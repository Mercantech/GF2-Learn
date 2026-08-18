using System.Collections.Concurrent;
using System.Data;
using System.Security.Claims;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IPageActivityService
{
    Task<bool> RecordHeartbeatAsync(
        ClaimsPrincipal? principal,
        PageActivityHeartbeatRequest? request,
        CancellationToken cancellationToken = default);
}

public sealed class PageActivityService : IPageActivityService
{
    private const int MaxReportedSeconds = 24 * 60 * 60;
    private const int MaxAcceptedSessionDeltaSeconds = 5 * 60;
    private const int InitialCreditSeconds = 15;
    private const int MaxStoredCreditSeconds = 60;
    private const int MaxConcurrencyAttempts = 4;

    // Production uses a PostgreSQL transaction-scoped advisory lock, which also
    // coordinates multiple app instances. This fallback gives non-relational test
    // providers the same per-user serialization semantics.
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> LocalUserLocks = new();

    private readonly Gf2LearnDbContext db;
    private readonly IAppUserService users;
    private readonly ContentService content;
    private readonly TimeProvider timeProvider;

    public PageActivityService(
        Gf2LearnDbContext db,
        IAppUserService users,
        ContentService content)
        : this(db, users, content, TimeProvider.System)
    {
    }

    public PageActivityService(
        Gf2LearnDbContext db,
        IAppUserService users,
        ContentService content,
        TimeProvider timeProvider)
    {
        this.db = db;
        this.users = users;
        this.content = content;
        this.timeProvider = timeProvider;
    }

    public async Task<bool> RecordHeartbeatAsync(
        ClaimsPrincipal? principal,
        PageActivityHeartbeatRequest? request,
        CancellationToken cancellationToken = default)
    {
        if (principal is null
            || request is null
            || request.SessionId == Guid.Empty
            || request.ActiveSeconds <= 0
            || request.ActiveSeconds > MaxReportedSeconds
            || string.IsNullOrWhiteSpace(request.ContentType)
            || string.IsNullOrWhiteSpace(request.ContentSlug))
        {
            return false;
        }

        var contentType = request.ContentType.Trim().ToLowerInvariant();
        var contentSlug = request.ContentSlug.Trim();
        if (!IsKnownContent(contentType, contentSlug))
            return false;

        var currentUser = await users.EnsureCurrentUserAsync(
            principal,
            cancellationToken: cancellationToken);
        if (currentUser is null || currentUser.IsEducator)
            return false;

        var userId = currentUser.Id;
        var usesRelationalDatabase = db.Database.IsRelational();
        SemaphoreSlim? localUserLock = null;

        if (!usesRelationalDatabase)
        {
            localUserLock = LocalUserLocks.GetOrAdd(userId, static _ => new SemaphoreSlim(1, 1));
            await localUserLock.WaitAsync(cancellationToken);
        }

        try
        {
            for (var attempt = 1; attempt <= MaxConcurrencyAttempts; attempt++)
            {
                db.ChangeTracker.Clear();

                try
                {
                    return await RecordSerializedAsync(
                        userId,
                        contentType,
                        contentSlug,
                        request,
                        usesRelationalDatabase,
                        cancellationToken);
                }
                catch (DbUpdateConcurrencyException) when (attempt < MaxConcurrencyAttempts)
                {
                    // A bounded retry handles deletes or writes from maintenance and
                    // non-PostgreSQL providers. Generic DbUpdateException instances are
                    // deliberately not swallowed: schema/FK/storage failures must surface.
                }
            }

            throw new DbUpdateConcurrencyException(
                $"Aktivitet for bruger {userId} kunne ikke gemmes efter {MaxConcurrencyAttempts} forsøg.");
        }
        finally
        {
            localUserLock?.Release();
        }
    }

    private async Task<bool> RecordSerializedAsync(
        Guid userId,
        string contentType,
        string contentSlug,
        PageActivityHeartbeatRequest request,
        bool usesRelationalDatabase,
        CancellationToken cancellationToken)
    {
        await using var transaction = usesRelationalDatabase
            ? await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
            : null;

        if (IsNpgsql())
        {
            // A single user can have many tabs and can hit different app instances.
            // The transaction-level lock serializes their shared credit bucket and all
            // dependent daily increments without retaining identifying raw events.
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended(CAST({userId} AS text), 0))",
                cancellationToken);
        }

        var now = timeProvider.GetUtcNow();
        var session = await db.PageActivitySessions
            .FirstOrDefaultAsync(item => item.Id == request.SessionId, cancellationToken);
        int candidateDelta;

        if (session is null)
        {
            var startedAt = request.StartedAt > now || request.StartedAt < now.AddDays(-1)
                ? now
                : request.StartedAt;
            candidateDelta = Math.Min(request.ActiveSeconds, MaxAcceptedSessionDeltaSeconds);
            session = new PageActivitySession
            {
                Id = request.SessionId,
                UserId = userId,
                ContentType = contentType,
                ContentSlug = contentSlug,
                ReportedActiveSeconds = request.ActiveSeconds,
                StartedAt = startedAt,
                LastHeartbeatAt = now
            };
            db.PageActivitySessions.Add(session);
        }
        else
        {
            if (session.UserId != userId
                || !string.Equals(session.ContentType, contentType, StringComparison.Ordinal)
                || !string.Equals(session.ContentSlug, contentSlug, StringComparison.Ordinal))
            {
                if (transaction is not null)
                    await transaction.RollbackAsync(cancellationToken);

                return false;
            }

            var reportedDelta = request.ActiveSeconds - session.ReportedActiveSeconds;
            candidateDelta = reportedDelta > 0
                ? Math.Min(reportedDelta, MaxAcceptedSessionDeltaSeconds)
                : 0;

            // Keep the highest cumulative client value even when server credit clamps
            // the delta. Retries remain idempotent and a forged jump cannot leak later.
            session.ReportedActiveSeconds = Math.Max(
                session.ReportedActiveSeconds,
                request.ActiveSeconds);
            session.LastHeartbeatAt = now;
            session.Version++;
        }

        var creditGate = await db.PageActivityCreditGates
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (creditGate is null)
        {
            creditGate = new PageActivityCreditGate
            {
                UserId = userId,
                AvailableSeconds = InitialCreditSeconds,
                LastRefillAt = now
            };
            db.PageActivityCreditGates.Add(creditGate);
        }
        else
        {
            RefillCredit(creditGate, now);
            creditGate.Version++;
        }

        var acceptedDelta = Math.Min(candidateDelta, creditGate.AvailableSeconds);
        creditGate.AvailableSeconds -= acceptedDelta;

        if (acceptedDelta > 0)
        {
            var persistedUser = await db.AppUsers
                .SingleAsync(user => user.Id == userId, cancellationToken);
            if (persistedUser.LastActivityAt is null || persistedUser.LastActivityAt < now)
                persistedUser.LastActivityAt = now;

            var activityDate = DateOnly.FromDateTime(now.UtcDateTime);
            var daily = await db.PageActivityDaily
                .FirstOrDefaultAsync(item => item.UserId == userId
                                             && item.ContentType == contentType
                                             && item.ContentSlug == contentSlug
                                             && item.ActivityDate == activityDate,
                    cancellationToken);
            var creditVisit = !session.VisitCredited;
            session.VisitCredited = true;

            if (daily is null)
            {
                daily = new PageActivityDaily
                {
                    UserId = userId,
                    ContentType = contentType,
                    ContentSlug = contentSlug,
                    ActivityDate = activityDate,
                    ActiveSeconds = acceptedDelta,
                    VisitCount = creditVisit ? 1 : 0,
                    LastSeenAt = now
                };
                db.PageActivityDaily.Add(daily);
            }
            else
            {
                daily.ActiveSeconds += acceptedDelta;
                if (creditVisit)
                    daily.VisitCount++;
                daily.LastSeenAt = now;
                daily.Version++;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        if (transaction is not null)
            await transaction.CommitAsync(cancellationToken);

        return true;
    }

    private static void RefillCredit(PageActivityCreditGate gate, DateTimeOffset now)
    {
        gate.AvailableSeconds = Math.Clamp(gate.AvailableSeconds, 0, MaxStoredCreditSeconds);
        if (gate.LastRefillAt > now)
        {
            gate.LastRefillAt = now;
            return;
        }

        var elapsedWholeSeconds = (int)Math.Min(
            int.MaxValue,
            Math.Floor((now - gate.LastRefillAt).TotalSeconds));
        if (elapsedWholeSeconds <= 0)
            return;

        var capacity = MaxStoredCreditSeconds - gate.AvailableSeconds;
        var granted = Math.Min(elapsedWholeSeconds, capacity);
        gate.AvailableSeconds += granted;

        // Once the bucket is full, discard excess idle time. Otherwise retain the
        // sub-second remainder so frequent heartbeats cannot lose earned time.
        gate.LastRefillAt = elapsedWholeSeconds >= capacity
            ? now
            : gate.LastRefillAt.AddSeconds(elapsedWholeSeconds);
    }

    private bool IsKnownContent(string contentType, string contentSlug) =>
        contentType switch
        {
            "curriculum" => content.GetCurriculum(contentSlug) is not null,
            "exercise" => content.GetExercise(contentSlug) is not null,
            _ => false
        };

    private bool IsNpgsql() => string.Equals(
        db.Database.ProviderName,
        "Npgsql.EntityFrameworkCore.PostgreSQL",
        StringComparison.Ordinal);
}
