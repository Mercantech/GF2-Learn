using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace GF2Learn.Web.Services;

public interface ILearningGroupService
{
    Task<IReadOnlyList<LearningGroupSummaryDto>> GetGroupsAsync(
        bool includeArchived = true,
        CancellationToken cancellationToken = default);

    Task<LearningGroupDetailDto?> GetGroupAsync(
        Guid groupId,
        string? studentSearch = null,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateGroupAsync(
        string name,
        string createdBySub,
        CancellationToken cancellationToken = default);

    Task SetArchivedAsync(
        Guid groupId,
        bool isArchived,
        CancellationToken cancellationToken = default);

    Task AddMemberAsync(
        Guid groupId,
        Guid userId,
        string addedBySub,
        CancellationToken cancellationToken = default);

    Task RemoveMemberAsync(
        Guid groupId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<GeneratedGroupAccessDto> GenerateInviteLinkAsync(
        Guid groupId,
        string createdBySub,
        CancellationToken cancellationToken = default);

    Task<GeneratedGroupAccessDto> GenerateJoinCodeAsync(
        Guid groupId,
        string createdBySub,
        CancellationToken cancellationToken = default);

    Task<JoinGroupResult> RedeemInviteAsync(
        ClaimsPrincipal principal,
        string token,
        CancellationToken cancellationToken = default);

    Task<JoinGroupResult> RedeemCodeAsync(
        ClaimsPrincipal principal,
        string code,
        CancellationToken cancellationToken = default);
}

public sealed class LearningGroupService(
    IDbContextFactory<Gf2LearnDbContext> dbFactory,
    IAppUserService users) : ILearningGroupService
{
    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int AccessGenerationAttempts = 5;
    private const string TokenHashIndex = "IX_learning_group_access_tokens_TokenHash";
    private const string ActiveJoinCodeIndex = "UX_learning_group_access_tokens_active_join_code";

    public async Task<IReadOnlyList<LearningGroupSummaryDto>> GetGroupsAsync(
        bool includeArchived = true,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var query = db.LearningGroups.AsNoTracking();
        if (!includeArchived)
            query = query.Where(group => !group.IsArchived);

        return await query
            .OrderBy(group => group.IsArchived)
            .ThenBy(group => group.Name)
            .Select(group => new LearningGroupSummaryDto(
                group.Id,
                group.Name,
                group.IsArchived,
                group.Members.Count(member => !member.User.IsEducator),
                group.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<LearningGroupDetailDto?> GetGroupAsync(
        Guid groupId,
        string? studentSearch = null,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var group = await db.LearningGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == groupId, cancellationToken);
        if (group is null)
            return null;

        var members = await db.LearningGroupMembers
            .AsNoTracking()
            .Where(member => member.GroupId == groupId && !member.User.IsEducator)
            .OrderBy(member => member.User.AuthDisplayName)
            .ThenBy(member => member.User.UserSub)
            .Select(member => new LearningGroupMemberDto(
                member.UserId,
                StudentLabel(member.User.AuthDisplayName, member.User.UserSub, member.User.Id),
                member.User.AdminMetadata == null ? null : member.User.AdminMetadata.Nickname,
                member.JoinedAt,
                member.Source.ToString()))
            .ToListAsync(cancellationToken);

        var normalizedSearch = studentSearch?.Trim();
        var studentsQuery = db.AppUsers
            .AsNoTracking()
            .Where(user => !user.IsEducator);

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            var normalizedSearchLower = normalizedSearch.ToLowerInvariant();
            studentsQuery = studentsQuery.Where(user =>
                (user.AdminMetadata != null
                    && user.AdminMetadata.Nickname != null
                    && user.AdminMetadata.Nickname.ToLower().Contains(normalizedSearchLower))
                || (user.AuthDisplayName != null
                    && user.AuthDisplayName.ToLower().Contains(normalizedSearchLower))
                || user.UserSub.ToLower().Contains(normalizedSearchLower));
        }

        var available = await studentsQuery
            .OrderBy(user => user.AuthDisplayName)
            .ThenBy(user => user.UserSub)
            .Take(100)
            .Select(user => new AvailableStudentDto(
                user.Id,
                StudentLabel(user.AuthDisplayName, user.UserSub, user.Id),
                user.AdminMetadata == null ? null : user.AdminMetadata.Nickname,
                user.GroupMemberships.Any(member => member.GroupId == groupId)))
            .ToListAsync(cancellationToken);

        return new LearningGroupDetailDto(
            group.Id,
            group.Name,
            group.IsArchived,
            members,
            available);
    }

    public async Task<Guid> CreateGroupAsync(
        string name,
        string createdBySub,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var cleanedName = CleanRequired(name, 160, "Holdet skal have et navn.");
        var now = DateTimeOffset.UtcNow;
        var group = new LearningGroup
        {
            Id = Guid.NewGuid(),
            Name = cleanedName,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBySub = CleanRequired(createdBySub, 128, "Superadmin mangler et bruger-id.")
        };

        db.LearningGroups.Add(group);
        await db.SaveChangesAsync(cancellationToken);
        return group.Id;
    }

    public async Task SetArchivedAsync(
        Guid groupId,
        bool isArchived,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
        var group = await LockGroupAsync(db, groupId, cancellationToken)
            ?? throw new InvalidOperationException("Holdet findes ikke.");

        var now = DateTimeOffset.UtcNow;
        group.IsArchived = isArchived;
        group.UpdatedAt = now;

        if (isArchived)
        {
            await db.LearningGroupAccessTokens
                .Where(token => token.GroupId == groupId && token.RevokedAt == null)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(token => token.RevokedAt, now),
                    cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        await CommitAsync(transaction, cancellationToken);
    }

    public async Task AddMemberAsync(
        Guid groupId,
        Guid userId,
        string addedBySub,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
        var group = await LockGroupAsync(db, groupId, cancellationToken);
        var userExists = await db.AppUsers
            .AnyAsync(user => user.Id == userId && !user.IsEducator, cancellationToken);

        if (group is null || group.IsArchived || !userExists)
            throw new InvalidOperationException("Holdet eller eleven findes ikke.");

        if (await db.LearningGroupMembers.AnyAsync(
                member => member.GroupId == groupId && member.UserId == userId,
                cancellationToken))
        {
            await CommitAsync(transaction, cancellationToken);
            return;
        }

        db.LearningGroupMembers.Add(new LearningGroupMember
        {
            GroupId = groupId,
            UserId = userId,
            JoinedAt = DateTimeOffset.UtcNow,
            Source = GroupMembershipSource.Manual,
            AddedBySub = CleanRequired(addedBySub, 128, "Superadmin mangler et bruger-id.")
        });
        await db.SaveChangesAsync(cancellationToken);
        await CommitAsync(transaction, cancellationToken);
    }

    public async Task RemoveMemberAsync(
        Guid groupId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
        if (await LockGroupAsync(db, groupId, cancellationToken) is null)
        {
            await CommitAsync(transaction, cancellationToken);
            return;
        }

        var membership = await db.LearningGroupMembers
            .FirstOrDefaultAsync(
                member => member.GroupId == groupId && member.UserId == userId,
                cancellationToken);
        if (membership is not null)
        {
            db.LearningGroupMembers.Remove(membership);
            await db.SaveChangesAsync(cancellationToken);
        }

        await CommitAsync(transaction, cancellationToken);
    }

    public async Task<GeneratedGroupAccessDto> GenerateInviteLinkAsync(
        Guid groupId,
        string createdBySub,
        CancellationToken cancellationToken = default)
    {
        var actorSub = CleanRequired(createdBySub, 128, "Superadmin mangler et bruger-id.");

        for (var attempt = 1; attempt <= AccessGenerationAttempts; attempt++)
        {
            var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            var expiresAt = DateTimeOffset.UtcNow.AddDays(14);

            try
            {
                await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
                await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
                await EnsureActiveLockedGroupAsync(db, groupId, cancellationToken);

                db.LearningGroupAccessTokens.Add(NewAccessToken(
                    groupId,
                    GroupAccessTokenKind.InviteLink,
                    rawToken,
                    expiresAt,
                    maxUses: 1,
                    actorSub));
                await db.SaveChangesAsync(cancellationToken);
                await CommitAsync(transaction, cancellationToken);

                return new GeneratedGroupAccessDto(
                    rawToken,
                    $"/join/{rawToken}",
                    expiresAt,
                    "Invitation");
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, TokenHashIndex))
            {
                // A cryptographic collision is extraordinarily unlikely, but retrying keeps
                // the operation correct if one is ever observed (or forced in a test).
                if (attempt == AccessGenerationAttempts)
                    break;
            }
        }

        throw new InvalidOperationException("Kunne ikke generere en unik invitation. Prøv igen.");
    }

    public async Task<GeneratedGroupAccessDto> GenerateJoinCodeAsync(
        Guid groupId,
        string createdBySub,
        CancellationToken cancellationToken = default)
    {
        var actorSub = CleanRequired(createdBySub, 128, "Superadmin mangler et bruger-id.");

        for (var attempt = 1; attempt <= AccessGenerationAttempts; attempt++)
        {
            var now = DateTimeOffset.UtcNow;
            var normalizedCode = GenerateCode(8);
            var displayCode = normalizedCode.Insert(4, "-");
            var expiresAt = now.AddDays(90);

            try
            {
                await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
                await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
                await EnsureActiveLockedGroupAsync(db, groupId, cancellationToken);

                await db.LearningGroupAccessTokens
                    .Where(token => token.GroupId == groupId
                                    && token.Kind == GroupAccessTokenKind.JoinCode
                                    && token.RevokedAt == null)
                    .ExecuteUpdateAsync(
                        setters => setters.SetProperty(token => token.RevokedAt, now),
                        cancellationToken);

                db.LearningGroupAccessTokens.Add(NewAccessToken(
                    groupId,
                    GroupAccessTokenKind.JoinCode,
                    normalizedCode,
                    expiresAt,
                    maxUses: null,
                    actorSub));
                await db.SaveChangesAsync(cancellationToken);
                await CommitAsync(transaction, cancellationToken);

                return new GeneratedGroupAccessDto(
                    displayCode,
                    "/join",
                    expiresAt,
                    "Holdkode");
            }
            catch (DbUpdateException ex) when (
                IsUniqueViolation(ex, TokenHashIndex, ActiveJoinCodeIndex))
            {
                // Retry the complete transaction. A failed PostgreSQL transaction cannot
                // safely be reused, and rolling back also preserves the previous active code.
                if (attempt == AccessGenerationAttempts)
                    break;
            }
        }

        throw new InvalidOperationException("Kunne ikke generere en unik holdkode. Prøv igen.");
    }

    public Task<JoinGroupResult> RedeemInviteAsync(
        ClaimsPrincipal principal,
        string token,
        CancellationToken cancellationToken = default) =>
        RedeemAsync(principal, token.Trim(), GroupAccessTokenKind.InviteLink, cancellationToken);

    public Task<JoinGroupResult> RedeemCodeAsync(
        ClaimsPrincipal principal,
        string code,
        CancellationToken cancellationToken = default) =>
        RedeemAsync(principal, NormalizeCode(code), GroupAccessTokenKind.JoinCode, cancellationToken);

    private async Task<JoinGroupResult> RedeemAsync(
        ClaimsPrincipal principal,
        string rawValue,
        GroupAccessTokenKind kind,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return InvalidAccessResult(kind);

        var ensuredUser = await users.EnsureCurrentUserAsync(
            principal,
            cancellationToken: cancellationToken);
        if (ensuredUser is null || ensuredUser.IsEducator)
            return new JoinGroupResult(false, "Kun elever kan tilmelde sig et hold.");

        var hash = Hash(rawValue);
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        // This first lookup only discovers the group whose row must be locked first. The
        // token itself is queried again under that lock, so a revoked tracked entity can
        // never leak across a long-lived Blazor circuit or win a rotation race.
        var candidate = await db.LearningGroupAccessTokens
            .AsNoTracking()
            .Where(token => token.TokenHash == hash && token.Kind == kind)
            .Select(token => new { token.Id, token.GroupId })
            .FirstOrDefaultAsync(cancellationToken);
        if (candidate is null)
            return InvalidAccessResult(kind);

        await using var transaction = await BeginWriteTransactionAsync(db, cancellationToken);
        var group = await LockGroupAsync(db, candidate.GroupId, cancellationToken);
        var access = await LockAccessTokenAsync(db, candidate.Id, cancellationToken);
        var now = DateTimeOffset.UtcNow;

        if (group is null
            || access is null
            || access.GroupId != candidate.GroupId
            || access.Kind != kind
            || !string.Equals(access.TokenHash, hash, StringComparison.Ordinal)
            || group.IsArchived
            || access.RevokedAt is not null
            || access.ExpiresAt <= now
            || (access.MaxUses is not null && access.UseCount >= access.MaxUses))
        {
            return InvalidAccessResult(kind);
        }

        var alreadyMember = await db.LearningGroupMembers.AnyAsync(
            member => member.GroupId == access.GroupId && member.UserId == ensuredUser.Id,
            cancellationToken);
        if (alreadyMember)
        {
            await CommitAsync(transaction, cancellationToken);
            return new JoinGroupResult(
                true,
                $"Du er allerede med på holdet {group.Name}.",
                group.Name);
        }

        db.LearningGroupMembers.Add(new LearningGroupMember
        {
            GroupId = access.GroupId,
            UserId = ensuredUser.Id,
            JoinedAt = now,
            Source = kind == GroupAccessTokenKind.JoinCode
                ? GroupMembershipSource.Code
                : GroupMembershipSource.Invite,
            AddedBySub = null
        });

        // Unlimited hold codes are deliberately read-only. Updating their shared UseCount
        // would make unrelated students contend on an unnecessary concurrency token.
        if (access.MaxUses is not null)
            access.UseCount++;

        try
        {
            await db.SaveChangesAsync(cancellationToken);
            await CommitAsync(transaction, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await RollbackAsync(transaction, cancellationToken);
            if (await MembershipExistsFreshAsync(access.GroupId, ensuredUser.Id, cancellationToken))
            {
                return new JoinGroupResult(
                    true,
                    $"Du er nu med på holdet {group.Name}.",
                    group.Name);
            }

            return new JoinGroupResult(
                false,
                "Invitationen eller holdkoden blev netop brugt eller ændret. Prøv igen.");
        }
        catch (DbUpdateException)
        {
            await RollbackAsync(transaction, cancellationToken);
            if (await MembershipExistsFreshAsync(access.GroupId, ensuredUser.Id, cancellationToken))
            {
                return new JoinGroupResult(
                    true,
                    $"Du er nu med på holdet {group.Name}.",
                    group.Name);
            }

            throw;
        }

        return new JoinGroupResult(
            true,
            $"Du er nu med på holdet {group.Name}.",
            group.Name);
    }

    private static LearningGroupAccessToken NewAccessToken(
        Guid groupId,
        GroupAccessTokenKind kind,
        string rawValue,
        DateTimeOffset? expiresAt,
        int? maxUses,
        string createdBySub) => new()
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            Kind = kind,
            TokenHash = Hash(rawValue),
            DisplaySuffix = rawValue.Length <= 4 ? rawValue : rawValue[^4..],
            ExpiresAt = expiresAt,
            MaxUses = maxUses,
            UseCount = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBySub = createdBySub
        };

    private static async Task<LearningGroup?> LockGroupAsync(
        Gf2LearnDbContext db,
        Guid groupId,
        CancellationToken cancellationToken)
    {
        if (IsNpgsql(db))
        {
            return await db.LearningGroups
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM learning_groups
                    WHERE "Id" = {groupId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return await db.LearningGroups
            .SingleOrDefaultAsync(group => group.Id == groupId, cancellationToken);
    }

    private static async Task<LearningGroupAccessToken?> LockAccessTokenAsync(
        Gf2LearnDbContext db,
        Guid tokenId,
        CancellationToken cancellationToken)
    {
        if (IsNpgsql(db))
        {
            return await db.LearningGroupAccessTokens
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM learning_group_access_tokens
                    WHERE "Id" = {tokenId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return await db.LearningGroupAccessTokens
            .SingleOrDefaultAsync(token => token.Id == tokenId, cancellationToken);
    }

    private static async Task EnsureActiveLockedGroupAsync(
        Gf2LearnDbContext db,
        Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await LockGroupAsync(db, groupId, cancellationToken);
        if (group is null || group.IsArchived)
            throw new InvalidOperationException("Holdet findes ikke eller er arkiveret.");
    }

    private async Task<bool> MembershipExistsFreshAsync(
        Guid groupId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await using var verificationDb = await dbFactory.CreateDbContextAsync(cancellationToken);
        return await verificationDb.LearningGroupMembers
            .AsNoTracking()
            .AnyAsync(
                member => member.GroupId == groupId && member.UserId == userId,
                cancellationToken);
    }

    private static async Task<IDbContextTransaction?> BeginWriteTransactionAsync(
        Gf2LearnDbContext db,
        CancellationToken cancellationToken)
    {
        if (!db.Database.IsRelational())
            return null;

        return await db.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);
    }

    private static Task CommitAsync(
        IDbContextTransaction? transaction,
        CancellationToken cancellationToken) =>
        transaction is null
            ? Task.CompletedTask
            : transaction.CommitAsync(cancellationToken);

    private static Task RollbackAsync(
        IDbContextTransaction? transaction,
        CancellationToken cancellationToken) =>
        transaction is null
            ? Task.CompletedTask
            : transaction.RollbackAsync(cancellationToken);

    private static bool IsNpgsql(Gf2LearnDbContext db) =>
        string.Equals(
            db.Database.ProviderName,
            "Npgsql.EntityFrameworkCore.PostgreSQL",
            StringComparison.Ordinal);

    private static bool IsUniqueViolation(DbUpdateException exception, params string[] constraints) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: { } constraintName
        }
        && constraints.Contains(constraintName, StringComparer.Ordinal);

    private static JoinGroupResult InvalidAccessResult(GroupAccessTokenKind kind) =>
        new(false, kind == GroupAccessTokenKind.JoinCode
            ? "Holdkoden er ugyldig eller udløbet."
            : "Invitationen er ugyldig, brugt eller udløbet.");

    private static string GenerateCode(int length)
    {
        Span<char> chars = stackalloc char[length];
        for (var index = 0; index < length; index++)
            chars[index] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];

        return new string(chars);
    }

    private static string NormalizeCode(string code) =>
        new(code.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static string CleanRequired(string value, int maxLength, string error)
    {
        var cleaned = value.Trim();
        if (cleaned.Length == 0)
            throw new InvalidOperationException(error);

        return cleaned.Length <= maxLength ? cleaned : cleaned[..maxLength];
    }

    private static string StudentLabel(string? authDisplayName, string userSub, Guid id)
    {
        var cleanedDisplayName = authDisplayName?.Trim();
        if (!string.IsNullOrWhiteSpace(cleanedDisplayName))
            return cleanedDisplayName;

        var normalized = userSub.Trim();
        var identifier = normalized.Length == 0
            ? id.ToString("N")[..8]
            : normalized[..Math.Min(normalized.Length, 8)];
        return $"Elev {identifier.ToUpperInvariant()}";
    }
}
