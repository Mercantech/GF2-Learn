namespace GF2Learn.Web.Options;

/// <summary>
/// Configures which authenticated users can access educator and super-admin features.
/// Values can be supplied as arrays or as comma-separated strings in the
/// <c>AdminAccess</c> configuration section.
/// </summary>
public sealed class AdminAccessOptions
{
    public const string SectionName = "AdminAccess";

    public string[] EducatorRoles { get; set; } =
    [
        "teacher",
        "underviser",
        "admin",
        "superadmin",
        "super_admin"
    ];

    public string[] SuperAdminRoles { get; set; } =
    [
        "superadmin",
        "super_admin"
    ];

    /// <summary>
    /// Account identifiers that should have educator access.
    /// </summary>
    public string[] EducatorSubjects { get; set; } = [];

    /// <summary>
    /// Account identifiers that should have super-admin access. A
    /// super-admin also automatically has educator access.
    /// </summary>
    public string[] SuperAdminSubjects { get; set; } = [];
}
