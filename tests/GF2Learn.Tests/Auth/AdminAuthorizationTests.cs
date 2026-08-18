using System.Security.Claims;
using GF2Learn.Web.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GF2Learn.Tests.Auth;

public sealed class AdminAuthorizationTests
{
    [Fact]
    public async Task Unauthenticated_user_is_denied_even_with_an_educator_role()
    {
        using var services = CreateServices();
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, "teacher")]));

        var result = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);

        Assert.False(result.Succeeded);
    }

    [Theory]
    [InlineData("teacher")]
    [InlineData("admin")]
    public async Task Default_educator_roles_are_granted_educator_access(string role)
    {
        using var services = CreateServices();
        var user = AuthenticatedUser(new Claim(ClaimTypes.Role, role));

        var result = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Superadmin_has_both_superadmin_and_educator_access()
    {
        using var services = CreateServices();
        var user = AuthenticatedUser(new Claim("role", "superadmin"));

        var educator = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);
        var superAdmin = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.SuperAdmin);

        Assert.True(educator.Succeeded);
        Assert.True(superAdmin.Succeeded);
    }

    [Fact]
    public async Task Ordinary_student_is_denied_both_admin_policies()
    {
        using var services = CreateServices();
        var user = AuthenticatedUser(new Claim(ClaimTypes.Role, "student"));

        var educator = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);
        var superAdmin = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.SuperAdmin);

        Assert.False(educator.Succeeded);
        Assert.False(superAdmin.Succeeded);
    }

    [Fact]
    public async Task Role_claim_type_and_comma_separated_values_are_case_insensitive()
    {
        using var services = CreateServices();
        var user = AuthenticatedUser(new Claim("RoLeS", "student, TEACHER, viewer"));

        var result = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Configured_educator_subject_is_granted_only_educator_access()
    {
        using var services = CreateServices(new Dictionary<string, string?>
        {
            ["AdminAccess:EducatorSubjects"] = "educator-one, educator-two"
        });
        var user = AuthenticatedUser(new Claim("sub", "EDUCATOR-TWO"));

        var educator = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);
        var superAdmin = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.SuperAdmin);

        Assert.True(educator.Succeeded);
        Assert.False(superAdmin.Succeeded);
    }

    [Fact]
    public async Task Configured_superadmin_subject_is_granted_both_policies()
    {
        using var services = CreateServices(new Dictionary<string, string?>
        {
            ["AdminAccess:SuperAdminSubjects:0"] = "super-user"
        });
        var user = AuthenticatedUser(new Claim(ClaimTypes.NameIdentifier, "SUPER-USER"));

        var educator = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.Educator);
        var superAdmin = await AuthorizeAsync(services, user, AdminAuthorizationPolicies.SuperAdmin);

        Assert.True(educator.Succeeded);
        Assert.True(superAdmin.Succeeded);
    }

    [Fact]
    public async Task Scalar_role_configuration_replaces_a_lower_priority_array()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminAccess:EducatorRoles:0"] = "teacher",
                ["AdminAccess:EducatorRoles:1"] = "admin"
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminAccess:EducatorRoles"] = "faculty"
            })
            .Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddAdminAuthorization(configuration);
        using var services = serviceCollection.BuildServiceProvider();

        var configuredRole = await AuthorizeAsync(
            services,
            AuthenticatedUser(new Claim(ClaimTypes.Role, "faculty")),
            AdminAuthorizationPolicies.Educator);
        var replacedDefault = await AuthorizeAsync(
            services,
            AuthenticatedUser(new Claim(ClaimTypes.Role, "teacher")),
            AdminAuthorizationPolicies.Educator);

        Assert.True(configuredRole.Succeeded);
        Assert.False(replacedDefault.Succeeded);
    }

    private static ServiceProvider CreateServices(
        IReadOnlyDictionary<string, string?>? settings = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdminAuthorization(configuration);
        return services.BuildServiceProvider();
    }

    private static ClaimsPrincipal AuthenticatedUser(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "TestAuthentication", ClaimTypes.Name, ClaimTypes.Role));

    private static Task<AuthorizationResult> AuthorizeAsync(
        IServiceProvider services,
        ClaimsPrincipal user,
        string policy) =>
        services.GetRequiredService<IAuthorizationService>()
            .AuthorizeAsync(user, resource: null, policy);
}
