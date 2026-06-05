namespace GF2Learn.Web.Services;

public static class ExerciseCategoryCatalog
{
    public sealed record CategoryDef(string Key, string Title, string Icon, int Order);

    public static readonly IReadOnlyList<CategoryDef> All =
    [
        new("grundlaeg", "Grundlæggende", "📘", 1),
        new("logik", "Logik og løkker", "🔀", 2),
        new("samlinger", "Samlinger", "📦", 3),
        new("oop", "Metoder og OOP", "🏗️", 4),
    ];

    public static string? GetTitle(string? key) =>
        All.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.OrdinalIgnoreCase))?.Title;
}
