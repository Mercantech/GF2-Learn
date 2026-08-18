using System.Security.Claims;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace GF2Learn.Tests.Services;

public sealed class PageActivityServiceTests
{
    private const string KnownExerciseSlug = "01-variabler";
    private static readonly DateTimeOffset TestNow = new(2026, 8, 18, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Known_exercise_heartbeat_creates_session_gate_and_daily_activity()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var sessionId = Guid.NewGuid();

        var accepted = await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(sessionId, activeSeconds: 12, fixture.Clock));

        Assert.True(accepted);

        fixture.Db.ChangeTracker.Clear();
        var session = Assert.Single(await fixture.Db.PageActivitySessions.ToListAsync());
        Assert.Equal(sessionId, session.Id);
        Assert.Equal(fixture.UserId, session.UserId);
        Assert.Equal("exercise", session.ContentType);
        Assert.Equal(KnownExerciseSlug, session.ContentSlug);
        Assert.Equal(12, session.ReportedActiveSeconds);
        Assert.True(session.VisitCredited);

        var daily = Assert.Single(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Equal(12, daily.ActiveSeconds);
        Assert.Equal(1, daily.VisitCount);
        Assert.Equal(DateOnly.FromDateTime(TestNow.UtcDateTime), daily.ActivityDate);

        var gate = Assert.Single(await fixture.Db.PageActivityCreditGates.ToListAsync());
        Assert.Equal(3, gate.AvailableSeconds);
        Assert.Equal(TestNow, gate.LastRefillAt);
        Assert.Equal(TestNow, (await fixture.Db.AppUsers.SingleAsync()).LastActivityAt);
    }

    [Fact]
    public async Task Retried_cumulative_heartbeat_does_not_double_count_active_seconds()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var sessionId = Guid.NewGuid();
        var request = Heartbeat(sessionId, activeSeconds: 10, fixture.Clock);

        Assert.True(await fixture.Service.RecordHeartbeatAsync(TestPrincipal(), request));
        Assert.True(await fixture.Service.RecordHeartbeatAsync(TestPrincipal(), request));

        fixture.Db.ChangeTracker.Clear();
        var session = Assert.Single(await fixture.Db.PageActivitySessions.ToListAsync());
        var daily = Assert.Single(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Equal(10, session.ReportedActiveSeconds);
        Assert.Equal(10, daily.ActiveSeconds);
        Assert.Equal(1, daily.VisitCount);
    }

    [Fact]
    public async Task Higher_cumulative_heartbeat_adds_only_new_wall_clock_credited_delta()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var sessionId = Guid.NewGuid();

        Assert.True(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(sessionId, activeSeconds: 15, fixture.Clock)));

        fixture.Clock.Advance(TimeSpan.FromSeconds(5));
        Assert.True(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(sessionId, activeSeconds: 20, fixture.Clock)));

        fixture.Db.ChangeTracker.Clear();
        var session = Assert.Single(await fixture.Db.PageActivitySessions.ToListAsync());
        var daily = Assert.Single(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Equal(20, session.ReportedActiveSeconds);
        Assert.Equal(20, daily.ActiveSeconds);
        Assert.Equal(1, daily.VisitCount);
    }

    [Fact]
    public async Task Many_new_session_ids_cannot_inflate_initial_credit_or_visits()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var services = await Task.WhenAll(
            Enumerable.Range(0, 8).Select(_ => fixture.CreateSiblingAsync()));

        try
        {
            var results = await Task.WhenAll(services.Select(item =>
                item.Service.RecordHeartbeatAsync(
                    TestPrincipal(),
                    Heartbeat(Guid.NewGuid(), activeSeconds: 15, fixture.Clock))));

            Assert.All(results, Assert.True);
        }
        finally
        {
            foreach (var service in services)
                await service.DisposeAsync();
        }

        fixture.Db.ChangeTracker.Clear();
        Assert.Equal(8, await fixture.Db.PageActivitySessions.CountAsync());
        var daily = Assert.Single(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Equal(15, daily.ActiveSeconds);
        Assert.Equal(1, daily.VisitCount);
        Assert.Equal(0, (await fixture.Db.PageActivityCreditGates.SingleAsync()).AvailableSeconds);
    }

    [Fact]
    public async Task Parallel_sessions_preserve_every_available_increment()
    {
        await using var fixture = await TestFixture.CreateAsync();
        Assert.True(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(Guid.NewGuid(), activeSeconds: 5, fixture.Clock)));
        fixture.Clock.Advance(TimeSpan.FromSeconds(20));

        await using var first = await fixture.CreateSiblingAsync();
        await using var second = await fixture.CreateSiblingAsync();
        var results = await Task.WhenAll(
            first.Service.RecordHeartbeatAsync(
                TestPrincipal(),
                Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock)),
            second.Service.RecordHeartbeatAsync(
                TestPrincipal(),
                Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock)));

        Assert.All(results, Assert.True);
        fixture.Db.ChangeTracker.Clear();
        var daily = Assert.Single(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Equal(25, daily.ActiveSeconds);
        Assert.Equal(3, daily.VisitCount);
        Assert.Equal(10, (await fixture.Db.PageActivityCreditGates.SingleAsync()).AvailableSeconds);
    }

    [Fact]
    public async Task Out_of_order_parallel_cumulative_heartbeats_do_not_lose_an_increment()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var sessionId = Guid.NewGuid();
        Assert.True(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(sessionId, activeSeconds: 15, fixture.Clock)));
        fixture.Clock.Advance(TimeSpan.FromSeconds(15));

        await using var first = await fixture.CreateSiblingAsync();
        await using var second = await fixture.CreateSiblingAsync();
        var results = await Task.WhenAll(
            first.Service.RecordHeartbeatAsync(
                TestPrincipal(),
                Heartbeat(sessionId, activeSeconds: 20, fixture.Clock)),
            second.Service.RecordHeartbeatAsync(
                TestPrincipal(),
                Heartbeat(sessionId, activeSeconds: 30, fixture.Clock)));

        Assert.All(results, Assert.True);
        fixture.Db.ChangeTracker.Clear();
        Assert.Equal(30, (await fixture.Db.PageActivitySessions.SingleAsync()).ReportedActiveSeconds);
        Assert.Equal(30, (await fixture.Db.PageActivityDaily.SingleAsync()).ActiveSeconds);
        Assert.Equal(1, (await fixture.Db.PageActivityDaily.SingleAsync()).VisitCount);
    }

    [Fact]
    public async Task Initial_heartbeat_is_bounded_by_server_credit_not_client_session_id()
    {
        await using var fixture = await TestFixture.CreateAsync();

        Assert.True(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(Guid.NewGuid(), activeSeconds: 300, fixture.Clock)));

        fixture.Db.ChangeTracker.Clear();
        Assert.Equal(300, (await fixture.Db.PageActivitySessions.SingleAsync()).ReportedActiveSeconds);
        Assert.Equal(15, (await fixture.Db.PageActivityDaily.SingleAsync()).ActiveSeconds);
        Assert.Equal(0, (await fixture.Db.PageActivityCreditGates.SingleAsync()).AvailableSeconds);
    }

    [Fact]
    public async Task Null_request_and_null_fields_are_rejected_before_user_lookup()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var valid = Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock);

        Assert.False(await fixture.Service.RecordHeartbeatAsync(null, valid));
        Assert.False(await fixture.Service.RecordHeartbeatAsync(TestPrincipal(), null));
        Assert.False(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            valid with { ContentType = null! }));
        Assert.False(await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            valid with { ContentSlug = null! }));

        Assert.Equal(0, fixture.Users.EnsureCurrentUserCalls);
        Assert.Empty(await fixture.Db.PageActivitySessions.ToListAsync());
    }

    [Fact]
    public async Task Unknown_content_slug_is_rejected_without_creating_activity()
    {
        await using var fixture = await TestFixture.CreateAsync();

        var accepted = await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock) with
            {
                ContentSlug = "does-not-exist"
            });

        Assert.False(accepted);
        Assert.Equal(0, fixture.Users.EnsureCurrentUserCalls);
        Assert.Empty(await fixture.Db.PageActivitySessions.ToListAsync());
        Assert.Empty(await fixture.Db.PageActivityDaily.ToListAsync());
    }

    [Fact]
    public async Task Educator_heartbeat_is_rejected_without_creating_activity()
    {
        await using var fixture = await TestFixture.CreateAsync(isEducator: true);

        var accepted = await fixture.Service.RecordHeartbeatAsync(
            TestPrincipal(),
            Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock));

        Assert.False(accepted);
        Assert.Equal(1, fixture.Users.EnsureCurrentUserCalls);
        Assert.Empty(await fixture.Db.PageActivitySessions.ToListAsync());
        Assert.Empty(await fixture.Db.PageActivityDaily.ToListAsync());
        Assert.Null((await fixture.Db.AppUsers.SingleAsync()).LastActivityAt);
    }

    [Fact]
    public async Task Generic_database_update_error_is_not_hidden_as_success()
    {
        await using var fixture = await TestFixture.CreateAsync(
            interceptor: new ThrowingSaveChangesInterceptor());

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            fixture.Service.RecordHeartbeatAsync(
                TestPrincipal(),
                Heartbeat(Guid.NewGuid(), activeSeconds: 10, fixture.Clock)));

        Assert.Equal("synthetic storage failure", exception.Message);
    }

    [PostgreSqlFact]
    public async Task PostgreSql_advisory_lock_serializes_parallel_initial_sessions()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            PostgreSqlFactAttribute.ConnectionStringVariable)!;
        var options = new DbContextOptionsBuilder<Gf2LearnDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        var userId = Guid.NewGuid();

        await using (var setup = new Gf2LearnDbContext(options))
        {
            await setup.Database.EnsureDeletedAsync();
            await setup.Database.MigrateAsync();
            setup.AppUsers.Add(new AppUser
            {
                Id = userId,
                UserSub = "postgres-student",
                FirstSeenAt = TestNow,
                LastLoginAt = TestNow
            });
            await setup.SaveChangesAsync();
        }

        var content = CreateContentService();
        var clock = new ManualTimeProvider(TestNow);
        var serviceContexts = new List<ServiceContext>();

        try
        {
            for (var index = 0; index < 8; index++)
            {
                var db = new Gf2LearnDbContext(options);
                var user = new AppUser
                {
                    Id = userId,
                    UserSub = "postgres-student",
                    FirstSeenAt = TestNow,
                    LastLoginAt = TestNow
                };
                serviceContexts.Add(new ServiceContext(
                    db,
                    new PageActivityService(db, new FakeAppUserService(user), content, clock)));
            }

            var results = await Task.WhenAll(serviceContexts.Select(item =>
                item.Service.RecordHeartbeatAsync(
                    TestPrincipal(),
                    Heartbeat(Guid.NewGuid(), activeSeconds: 15, clock))));
            Assert.All(results, Assert.True);

            await using var verify = new Gf2LearnDbContext(options);
            Assert.Equal(8, await verify.PageActivitySessions.CountAsync());
            var daily = await verify.PageActivityDaily.SingleAsync();
            Assert.Equal(15, daily.ActiveSeconds);
            Assert.Equal(1, daily.VisitCount);
            Assert.Equal(0, (await verify.PageActivityCreditGates.SingleAsync()).AvailableSeconds);
        }
        finally
        {
            foreach (var context in serviceContexts)
                await context.DisposeAsync();

            await using var cleanup = new Gf2LearnDbContext(options);
            await cleanup.Database.EnsureDeletedAsync();
        }
    }

    private static PageActivityHeartbeatRequest Heartbeat(
        Guid sessionId,
        int activeSeconds,
        ManualTimeProvider clock) =>
        new(
            sessionId,
            "exercise",
            KnownExerciseSlug,
            activeSeconds,
            clock.GetUtcNow().AddMinutes(-1));

    private static ClaimsPrincipal TestPrincipal() =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "student-1")],
            "TestAuthentication"));

    private sealed class TestFixture : IAsyncDisposable
    {
        private readonly string databaseName;
        private readonly InMemoryDatabaseRoot databaseRoot;
        private readonly ContentService content;

        private TestFixture(
            string databaseName,
            InMemoryDatabaseRoot databaseRoot,
            Gf2LearnDbContext db,
            Guid userId,
            FakeAppUserService users,
            ContentService content,
            ManualTimeProvider clock,
            PageActivityService service)
        {
            this.databaseName = databaseName;
            this.databaseRoot = databaseRoot;
            this.content = content;
            Db = db;
            UserId = userId;
            Users = users;
            Clock = clock;
            Service = service;
        }

        public Gf2LearnDbContext Db { get; }
        public Guid UserId { get; }
        public FakeAppUserService Users { get; }
        public ManualTimeProvider Clock { get; }
        public PageActivityService Service { get; }

        public static async Task<TestFixture> CreateAsync(
            bool isEducator = false,
            SaveChangesInterceptor? interceptor = null)
        {
            var databaseName = $"page-activity-{Guid.NewGuid():N}";
            var databaseRoot = new InMemoryDatabaseRoot();
            var seedOptions = CreateOptions(databaseName, databaseRoot);
            var userId = Guid.NewGuid();

            await using (var seedDb = new Gf2LearnDbContext(seedOptions))
            {
                seedDb.AppUsers.Add(new AppUser
                {
                    Id = userId,
                    UserSub = "student-1",
                    IsEducator = isEducator,
                    FirstSeenAt = TestNow,
                    LastLoginAt = TestNow
                });
                await seedDb.SaveChangesAsync();
            }

            var serviceOptions = CreateOptions(databaseName, databaseRoot, interceptor);
            var db = new Gf2LearnDbContext(serviceOptions);
            var user = await db.AppUsers.AsNoTracking().SingleAsync();
            var users = new FakeAppUserService(user);
            var content = CreateContentService();
            var clock = new ManualTimeProvider(TestNow);
            var service = new PageActivityService(db, users, content, clock);
            return new TestFixture(
                databaseName,
                databaseRoot,
                db,
                userId,
                users,
                content,
                clock,
                service);
        }

        public async Task<ServiceContext> CreateSiblingAsync()
        {
            var db = new Gf2LearnDbContext(CreateOptions(databaseName, databaseRoot));
            var user = await db.AppUsers.AsNoTracking().SingleAsync();
            return new ServiceContext(
                db,
                new PageActivityService(db, new FakeAppUserService(user), content, Clock));
        }

        public ValueTask DisposeAsync() => Db.DisposeAsync();

        private static DbContextOptions<Gf2LearnDbContext> CreateOptions(
            string databaseName,
            InMemoryDatabaseRoot databaseRoot,
            SaveChangesInterceptor? interceptor = null)
        {
            var builder = new DbContextOptionsBuilder<Gf2LearnDbContext>()
                .UseInMemoryDatabase(databaseName, databaseRoot);
            if (interceptor is not null)
                builder.AddInterceptors(interceptor);

            return builder.Options;
        }
    }

    private sealed class ServiceContext(
        Gf2LearnDbContext db,
        PageActivityService service) : IAsyncDisposable
    {
        public PageActivityService Service { get; } = service;
        public ValueTask DisposeAsync() => db.DisposeAsync();
    }

    private sealed class FakeAppUserService(AppUser? user) : IAppUserService
    {
        public int EnsureCurrentUserCalls { get; private set; }

        public Task<AppUser?> EnsureCurrentUserAsync(
            ClaimsPrincipal principal,
            bool markLogin = false,
            CancellationToken cancellationToken = default)
        {
            EnsureCurrentUserCalls++;
            return Task.FromResult(user);
        }

        public Task TouchActivityAsync(
            string userSub,
            DateTimeOffset occurredAt,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private readonly Lock sync = new();
        private DateTimeOffset utcNow = utcNow;

        public override DateTimeOffset GetUtcNow()
        {
            lock (sync)
                return utcNow;
        }

        public void Advance(TimeSpan amount)
        {
            lock (sync)
                utcNow = utcNow.Add(amount);
        }
    }

    private sealed class ThrowingSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromException<InterceptionResult<int>>(
                new DbUpdateException("synthetic storage failure"));
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PostgreSqlFactAttribute : FactAttribute
    {
        public const string ConnectionStringVariable = "GF2LEARN_ACTIVITY_TEST_POSTGRES";

        public PostgreSqlFactAttribute()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionStringVariable)))
                Skip = $"Set {ConnectionStringVariable} to run the PostgreSQL integration test.";
        }
    }

    private static ContentService CreateContentService()
    {
        var contentRoot = FindRepositoryContentDirectory();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ContentPath"] = contentRoot
            })
            .Build();

        return new ContentService(
            new FakeWebHostEnvironment
            {
                ContentRootPath = Directory.GetParent(contentRoot)!.FullName,
                WebRootPath = Path.Combine(Directory.GetParent(contentRoot)!.FullName, "wwwroot")
            },
            configuration);
    }

    private static string FindRepositoryContentDirectory()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "content");
            if (File.Exists(Path.Combine(
                    candidate,
                    "opgaver",
                    "begynder",
                    $"{KnownExerciseSlug}.md")))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the repository content directory.");
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "GF2Learn.Tests";
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
