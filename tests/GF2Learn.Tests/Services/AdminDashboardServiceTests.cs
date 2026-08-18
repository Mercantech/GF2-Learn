using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace GF2Learn.Tests.Services;

public sealed class AdminDashboardServiceTests
{
    [Fact]
    public async Task Dashboard_search_and_detail_use_trimmed_auth_name_with_fallback()
    {
        var options = new DbContextOptionsBuilder<Gf2LearnDbContext>()
            .UseInMemoryDatabase($"admin-dashboard-{Guid.NewGuid():N}")
            .Options;
        var factory = new TestDbContextFactory(options);
        var now = DateTimeOffset.UtcNow;
        var namedStudent = new AppUser
        {
            Id = Guid.NewGuid(),
            UserSub = "f7259867-auth-sub",
            AuthDisplayName = "  Mathias Gaardsdal Steenberg  ",
            FirstSeenAt = now,
            LastLoginAt = now
        };
        var unnamedStudent = new AppUser
        {
            Id = Guid.NewGuid(),
            UserSub = "4690afd9-auth-sub",
            AuthDisplayName = "   ",
            FirstSeenAt = now,
            LastLoginAt = now
        };

        await using (var setupDb = factory.CreateDbContext())
        {
            await setupDb.Database.EnsureCreatedAsync();
            setupDb.AppUsers.AddRange(namedStudent, unnamedStudent);
            await setupDb.SaveChangesAsync();
        }

        var service = new AdminDashboardService(
            factory,
            CreateContentService(),
            new ExerciseCatalog(),
            new KnowledgeCheckCatalog());

        var searched = await service.GetDashboardAsync(
            new AdminDashboardQuery(Search: "gAaRdSdAl"));
        var namedRow = Assert.Single(searched.Students);
        Assert.Equal(namedStudent.Id, namedRow.UserId);
        Assert.Equal("Mathias Gaardsdal Steenberg", namedRow.StudentLabel);

        var allStudents = await service.GetDashboardAsync(new AdminDashboardQuery());
        Assert.Equal(
            "Elev 4690AFD9",
            Assert.Single(allStudents.Students, student => student.UserId == unnamedStudent.Id).StudentLabel);

        var detail = Assert.IsType<AdminStudentDetailDto>(
            await service.GetStudentAsync(namedStudent.Id));
        Assert.Equal("Mathias Gaardsdal Steenberg", detail.StudentLabel);
        Assert.Equal("Mathias Gaardsdal Steenberg", detail.AuthDisplayName);
    }

    private static ContentService CreateContentService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ContentPath"] = Path.Combine(Path.GetTempPath(), $"gf2learn-missing-{Guid.NewGuid():N}")
            })
            .Build();

        return new ContentService(
            new FakeWebHostEnvironment
            {
                ContentRootPath = Path.GetTempPath(),
                WebRootPath = Path.GetTempPath()
            },
            configuration);
    }

    private sealed class TestDbContextFactory(DbContextOptions<Gf2LearnDbContext> options)
        : IDbContextFactory<Gf2LearnDbContext>
    {
        public Gf2LearnDbContext CreateDbContext() => new(options);
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
