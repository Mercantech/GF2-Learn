using System.Security.Claims;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using GF2Learn.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Tests.Services;

public sealed class LearningGroupServiceTests
{
    [Fact]
    [Trait("Category", "PostgreSQL")]
    public async Task PostgreSql_parallel_rotations_and_unlimited_redemptions_are_serialized()
    {
        var connectionString = Environment.GetEnvironmentVariable("GF2LEARN_TEST_POSTGRES");
        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        var options = new DbContextOptionsBuilder<Gf2LearnDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        var factory = new TestDbContextFactory(options);
        var now = DateTimeOffset.UtcNow;
        var groupId = Guid.NewGuid();
        var students = Enumerable.Range(1, 8)
            .Select(index => new AppUser
            {
                Id = Guid.NewGuid(),
                UserSub = $"postgres-student-{index}",
                FirstSeenAt = now,
                LastLoginAt = now
            })
            .ToList();

        await using (var setupDb = factory.CreateDbContext())
        {
            await setupDb.Database.MigrateAsync();
            setupDb.LearningGroups.Add(new LearningGroup
            {
                Id = groupId,
                Name = "PostgreSQL concurrency",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBySub = "superadmin"
            });
            setupDb.AppUsers.AddRange(students);
            await setupDb.SaveChangesAsync();
        }

        var users = new FakeAppUserService(students);
        var service = new LearningGroupService(factory, users);
        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ =>
            service.GenerateJoinCodeAsync(groupId, "superadmin")));

        var finalCode = await service.GenerateJoinCodeAsync(groupId, "superadmin");
        var joins = await Task.WhenAll(students.Select(student =>
            service.RedeemCodeAsync(TestPrincipal(student.UserSub), finalCode.Value)));

        Assert.All(joins, result => Assert.True(result.Success));

        await using var verificationDb = factory.CreateDbContext();
        var codes = await verificationDb.LearningGroupAccessTokens
            .AsNoTracking()
            .Where(token => token.GroupId == groupId
                            && token.Kind == GroupAccessTokenKind.JoinCode)
            .ToListAsync();
        Assert.Equal(9, codes.Count);
        var active = Assert.Single(codes, token => token.RevokedAt is null);
        Assert.Equal(0, active.UseCount);
        Assert.Equal(
            students.Count,
            await verificationDb.LearningGroupMembers.CountAsync(member => member.GroupId == groupId));
    }

    [Fact]
    public async Task Rotating_code_revokes_the_previous_code_and_keeps_one_active()
    {
        await using var fixture = await TestFixture.CreateAsync(studentCount: 1);

        var previous = await fixture.Service.GenerateJoinCodeAsync(
            fixture.GroupId,
            "superadmin");
        var current = await fixture.Service.GenerateJoinCodeAsync(
            fixture.GroupId,
            "superadmin");

        await using var db = fixture.Factory.CreateDbContext();
        var codes = await db.LearningGroupAccessTokens
            .AsNoTracking()
            .Where(token => token.GroupId == fixture.GroupId
                            && token.Kind == GroupAccessTokenKind.JoinCode)
            .ToListAsync();

        Assert.Equal(2, codes.Count);
        Assert.Single(codes, token => token.RevokedAt is null);
        Assert.Single(codes, token => token.RevokedAt is not null);

        var oldResult = await fixture.Service.RedeemCodeAsync(
            TestPrincipal(fixture.Students[0].UserSub),
            previous.Value);
        var currentResult = await fixture.Service.RedeemCodeAsync(
            TestPrincipal(fixture.Students[0].UserSub),
            current.Value);

        Assert.False(oldResult.Success);
        Assert.True(currentResult.Success);
    }

    [Fact]
    public async Task Unlimited_code_does_not_update_use_count_for_multiple_students()
    {
        await using var fixture = await TestFixture.CreateAsync(studentCount: 3);
        var code = await fixture.Service.GenerateJoinCodeAsync(
            fixture.GroupId,
            "superadmin");

        foreach (var student in fixture.Students)
        {
            var result = await fixture.Service.RedeemCodeAsync(
                TestPrincipal(student.UserSub),
                code.Value);
            Assert.True(result.Success);
        }

        await using var db = fixture.Factory.CreateDbContext();
        var access = await db.LearningGroupAccessTokens
            .AsNoTracking()
            .SingleAsync(token => token.GroupId == fixture.GroupId
                                  && token.Kind == GroupAccessTokenKind.JoinCode
                                  && token.RevokedAt == null);

        Assert.Null(access.MaxUses);
        Assert.Equal(0, access.UseCount);
        Assert.Equal(
            fixture.Students.Count,
            await db.LearningGroupMembers.CountAsync(member => member.GroupId == fixture.GroupId));
    }

    [Fact]
    public async Task One_time_invite_is_consumed_only_once()
    {
        await using var fixture = await TestFixture.CreateAsync(studentCount: 2);
        var invite = await fixture.Service.GenerateInviteLinkAsync(
            fixture.GroupId,
            "superadmin");

        var first = await fixture.Service.RedeemInviteAsync(
            TestPrincipal(fixture.Students[0].UserSub),
            invite.Value);
        var second = await fixture.Service.RedeemInviteAsync(
            TestPrincipal(fixture.Students[1].UserSub),
            invite.Value);

        Assert.True(first.Success);
        Assert.False(second.Success);

        await using var db = fixture.Factory.CreateDbContext();
        var access = await db.LearningGroupAccessTokens
            .AsNoTracking()
            .SingleAsync(token => token.GroupId == fixture.GroupId
                                  && token.Kind == GroupAccessTokenKind.InviteLink);
        Assert.Equal(1, access.MaxUses);
        Assert.Equal(1, access.UseCount);
        Assert.Single(await db.LearningGroupMembers.ToListAsync());
    }

    [Fact]
    public async Task Archived_group_revokes_access_and_rejects_the_old_code()
    {
        await using var fixture = await TestFixture.CreateAsync(studentCount: 1);
        var code = await fixture.Service.GenerateJoinCodeAsync(
            fixture.GroupId,
            "superadmin");

        await fixture.Service.SetArchivedAsync(fixture.GroupId, isArchived: true);
        var result = await fixture.Service.RedeemCodeAsync(
            TestPrincipal(fixture.Students[0].UserSub),
            code.Value);

        Assert.False(result.Success);

        await using var db = fixture.Factory.CreateDbContext();
        Assert.NotNull((await db.LearningGroupAccessTokens.SingleAsync()).RevokedAt);
        Assert.Empty(await db.LearningGroupMembers.ToListAsync());
    }

    [Fact]
    public async Task Group_lists_and_search_use_auth_name_with_pseudonymous_fallback()
    {
        await using var fixture = await TestFixture.CreateAsync(studentCount: 2);
        await using (var db = fixture.Factory.CreateDbContext())
        {
            var students = await db.AppUsers
                .OrderBy(user => user.UserSub)
                .ToListAsync();
            students[0].AuthDisplayName = "  Mathias Gaardsdal Steenberg  ";
            students[1].AuthDisplayName = "   ";
            await db.SaveChangesAsync();
        }

        await fixture.Service.AddMemberAsync(
            fixture.GroupId,
            fixture.Students[0].Id,
            "superadmin");

        var group = Assert.IsType<LearningGroupDetailDto>(
            await fixture.Service.GetGroupAsync(fixture.GroupId));
        Assert.Equal("Mathias Gaardsdal Steenberg", Assert.Single(group.Members).StudentLabel);
        Assert.Equal(
            "Elev STUDENT-",
            Assert.Single(group.AvailableStudents, student => student.UserId == fixture.Students[1].Id).StudentLabel);

        var searchResult = Assert.IsType<LearningGroupDetailDto>(
            await fixture.Service.GetGroupAsync(fixture.GroupId, "gAaRdSdAl"));
        Assert.Equal(
            fixture.Students[0].Id,
            Assert.Single(searchResult.AvailableStudents).UserId);
    }

    private static ClaimsPrincipal TestPrincipal(string userSub) =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userSub)],
            "TestAuthentication"));

    private sealed class TestFixture : IAsyncDisposable
    {
        private readonly SqliteConnection connection;

        private TestFixture(
            SqliteConnection connection,
            TestDbContextFactory factory,
            FakeAppUserService users,
            LearningGroupService service,
            Guid groupId,
            IReadOnlyList<AppUser> students)
        {
            this.connection = connection;
            Factory = factory;
            Users = users;
            Service = service;
            GroupId = groupId;
            Students = students;
        }

        public TestDbContextFactory Factory { get; }
        public FakeAppUserService Users { get; }
        public LearningGroupService Service { get; }
        public Guid GroupId { get; }
        public IReadOnlyList<AppUser> Students { get; }

        public static async Task<TestFixture> CreateAsync(int studentCount)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<Gf2LearnDbContext>()
                .UseSqlite(connection)
                .Options;
            var factory = new TestDbContextFactory(options);

            var now = DateTimeOffset.UtcNow;
            var groupId = Guid.NewGuid();
            var students = Enumerable.Range(1, studentCount)
                .Select(index => new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserSub = $"student-{index}",
                    FirstSeenAt = now,
                    LastLoginAt = now
                })
                .ToList();

            await using (var db = factory.CreateDbContext())
            {
                await db.Database.EnsureCreatedAsync();
                db.LearningGroups.Add(new LearningGroup
                {
                    Id = groupId,
                    Name = "Testhold",
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBySub = "superadmin"
                });
                db.AppUsers.AddRange(students);
                await db.SaveChangesAsync();
            }

            var users = new FakeAppUserService(students);
            return new TestFixture(
                connection,
                factory,
                users,
                new LearningGroupService(factory, users),
                groupId,
                students);
        }

        public async ValueTask DisposeAsync() => await connection.DisposeAsync();
    }

    private sealed class TestDbContextFactory(DbContextOptions<Gf2LearnDbContext> options)
        : IDbContextFactory<Gf2LearnDbContext>
    {
        public Gf2LearnDbContext CreateDbContext() => new(options);
    }

    private sealed class FakeAppUserService(IEnumerable<AppUser> users) : IAppUserService
    {
        private readonly IReadOnlyDictionary<string, AppUser> usersBySub =
            users.ToDictionary(user => user.UserSub, StringComparer.Ordinal);

        public Task<AppUser?> EnsureCurrentUserAsync(
            ClaimsPrincipal principal,
            bool markLogin = false,
            CancellationToken cancellationToken = default)
        {
            var userSub = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Task.FromResult(
                userSub is not null && usersBySub.TryGetValue(userSub, out var user)
                    ? user
                    : null);
        }

        public Task TouchActivityAsync(
            string userSub,
            DateTimeOffset occurredAt,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
