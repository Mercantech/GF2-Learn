using System.Security.Claims;
using GF2Learn.Web.Options;

namespace GF2Learn.Web.Auth;

/// <summary>Reusable policy names for admin endpoints and components.</summary>
public static class AdminAuthorizationPolicies
{
    public const string Educator = "Educator";
    public const string SuperAdmin = "SuperAdmin";
}

public static class AdminAuthorizationExtensions
{
    /// <summary>
    /// Registers educator and super-admin policies from the AdminAccess
    /// configuration section. Role and subject comparisons are case-insensitive.
    /// </summary>
    public static IServiceCollection AddAdminAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var access = ReadOptions(configuration.GetSection(AdminAccessOptions.SectionName));

        services.Configure<AdminAccessOptions>(options =>
        {
            options.EducatorRoles = [.. access.EducatorRoles];
            options.SuperAdminRoles = [.. access.SuperAdminRoles];
            options.EducatorSubjects = [.. access.EducatorSubjects];
            options.SuperAdminSubjects = [.. access.SuperAdminSubjects];
        });

        var educatorRoles = ToSet(access.EducatorRoles.Concat(access.SuperAdminRoles));
        var superAdminRoles = ToSet(access.SuperAdminRoles);
        var educatorSubjects = ToSet(access.EducatorSubjects.Concat(access.SuperAdminSubjects));
        var superAdminSubjects = ToSet(access.SuperAdminSubjects);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminAuthorizationPolicies.Educator, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    HasConfiguredRole(context.User, educatorRoles)
                    || HasConfiguredSubject(context.User, educatorSubjects));
            });

            options.AddPolicy(AdminAuthorizationPolicies.SuperAdmin, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    HasConfiguredRole(context.User, superAdminRoles)
                    || HasConfiguredSubject(context.User, superAdminSubjects));
            });
        });

        return services;
    }

    private static AdminAccessOptions ReadOptions(IConfigurationSection section)
    {
        var defaults = new AdminAccessOptions();

        return new AdminAccessOptions
        {
            EducatorRoles = ReadValues(
                section.GetSection(nameof(AdminAccessOptions.EducatorRoles)),
                defaults.EducatorRoles),
            SuperAdminRoles = ReadValues(
                section.GetSection(nameof(AdminAccessOptions.SuperAdminRoles)),
                defaults.SuperAdminRoles),
            EducatorSubjects = ReadValues(
                section.GetSection(nameof(AdminAccessOptions.EducatorSubjects)),
                defaults.EducatorSubjects),
            SuperAdminSubjects = ReadValues(
                section.GetSection(nameof(AdminAccessOptions.SuperAdminSubjects)),
                defaults.SuperAdminSubjects)
        };
    }

    private static string[] ReadValues(IConfigurationSection section, IEnumerable<string> defaults)
    {
        if (!section.Exists())
            return Normalize(defaults);

        // A scalar value (the normal environment-variable representation) must
        // replace any lower-priority JSON array. Otherwise an operator cannot
        // tighten the default role list in production because both values would
        // silently be combined.
        if (section.Value is not null)
            return Normalize([section.Value]);

        var values = section.GetChildren()
            .Select(child => child.Value)
            .Where(value => value is not null)
            .Select(value => value!);

        return Normalize(values);
    }

    private static string[] Normalize(IEnumerable<string> values) =>
        values
            .SelectMany(value => value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static HashSet<string> ToSet(IEnumerable<string> values) =>
        new(values, StringComparer.OrdinalIgnoreCase);

    private static bool HasConfiguredRole(ClaimsPrincipal user, HashSet<string> configuredRoles)
    {
        if (configuredRoles.Count == 0)
            return false;

        return user.Claims
            .Where(claim => IsRoleClaim(claim.Type))
            .SelectMany(claim => claim.Value.Split(
                ',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Any(configuredRoles.Contains);
    }

    private static bool HasConfiguredSubject(ClaimsPrincipal user, HashSet<string> configuredSubjects)
    {
        if (configuredSubjects.Count == 0)
            return false;

        return user.Claims.Any(claim =>
            IsSubjectClaim(claim.Type)
            && configuredSubjects.Contains(claim.Value.Trim()));
    }

    private static bool IsRoleClaim(string claimType) =>
        claimType.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
        || claimType.Equals("role", StringComparison.OrdinalIgnoreCase)
        || claimType.Equals("roles", StringComparison.OrdinalIgnoreCase);

    private static bool IsSubjectClaim(string claimType) =>
        claimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)
        || claimType.Equals("sub", StringComparison.OrdinalIgnoreCase);
}
