using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public static class CurriculumCategoryCatalog
{
    public sealed record CategoryDef(string Key, string Title, string Icon, int Order);

    public static readonly IReadOnlyList<CategoryDef> All =
    [
        new("csharp", "C#", "💻", 1),
        new("kodeprincipper", "Kode-principper", "✨", 2),
        new("git", "Git", "🌿", 3),
    ];

    public static string ResolveCategory(ContentItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Category))
            return item.Category.Trim().ToLowerInvariant();

        if (item.Topics.Contains("git", StringComparer.OrdinalIgnoreCase))
            return "git";

        return "csharp";
    }

    public static string? GetTitle(string? key) =>
        All.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.OrdinalIgnoreCase))?.Title;
}
