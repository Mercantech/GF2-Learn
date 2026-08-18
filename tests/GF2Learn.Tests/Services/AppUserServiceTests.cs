using System.Security.Claims;
using GF2Learn.Web.Data;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Tests.Services;

public sealed class AppUserServiceTests
{
    [Fact]
    public async Task New_user_persists_trimmed_auth_display_name()
    {
        await using var db = CreateDatabase();
        var service = new AppUserService(db, new DenyAllAuthorizationService());

        var user = await service.EnsureCurrentUserAsync(
            AuthenticatedPrincipal("student-1", "  Mathias Gaardsdal Steenberg  "));

        Assert.NotNull(user);
        Assert.Equal("Mathias Gaardsdal Steenberg", user.AuthDisplayName);
        Assert.Equal(
            "Mathias Gaardsdal Steenberg",
            (await db.AppUsers.AsNoTracking().SingleAsync()).AuthDisplayName);
    }

    [Fact]
    public async Task Existing_user_refreshes_changed_auth_display_name()
    {
        await using var db = CreateDatabase();
        var service = new AppUserService(db, new DenyAllAuthorizationService());
        await service.EnsureCurrentUserAsync(AuthenticatedPrincipal("student-1", "Old name"));

        await service.EnsureCurrentUserAsync(AuthenticatedPrincipal("student-1", "New name"));

        Assert.Equal(
            "New name",
            (await db.AppUsers.AsNoTracking().SingleAsync()).AuthDisplayName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("Invalid\nname")]
    public async Task Missing_or_invalid_name_does_not_erase_existing_value(string? name)
    {
        await using var db = CreateDatabase();
        var service = new AppUserService(db, new DenyAllAuthorizationService());
        await service.EnsureCurrentUserAsync(AuthenticatedPrincipal("student-1", "Existing name"));

        await service.EnsureCurrentUserAsync(AuthenticatedPrincipal("student-1", name));

        Assert.Equal(
            "Existing name",
            (await db.AppUsers.AsNoTracking().SingleAsync()).AuthDisplayName);
    }

    [Fact]
    public async Task Auth_display_name_is_capped_without_splitting_a_surrogate_pair()
    {
        await using var db = CreateDatabase();
        var service = new AppUserService(db, new DenyAllAuthorizationService());
        var name = new string('A', 255) + "🙂" + "B";

        var user = await service.EnsureCurrentUserAsync(AuthenticatedPrincipal("student-1", name));

        Assert.NotNull(user);
        Assert.Equal(255, user.AuthDisplayName?.Length);
        Assert.DoesNotContain(user.AuthDisplayName!, char.IsSurrogate);
    }

    private static Gf2LearnDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<Gf2LearnDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new Gf2LearnDbContext(options);
    }

    private static ClaimsPrincipal AuthenticatedPrincipal(string userSub, string? name)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userSub)
        };
        if (name is not null)
            claims.Add(new Claim(ClaimTypes.Name, name));

        return new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            "TestAuthentication",
            ClaimTypes.Name,
            ClaimTypes.Role));
    }

    private sealed class DenyAllAuthorizationService : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            IEnumerable<IAuthorizationRequirement> requirements) =>
            Task.FromResult(AuthorizationResult.Failed());

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            string policyName) =>
            Task.FromResult(AuthorizationResult.Failed());
    }
}
