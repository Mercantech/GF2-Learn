namespace GF2Learn.Web.Services;

public static class ExerciseCategoryCatalog
{
    public sealed record CategoryDef(string Key, string Title, string Icon, int Order);

    public static readonly IReadOnlyList<CategoryDef> All =
    [
        new("grundlaeg", "Grundlæggende", "📘", 1),
        new("logik", "Logic and loops", "🔀", 2),
        new("samlinger", "Collections", "📦", 3),
        new("oop", "Metoder og OOP", "🏗️", 4),
        new("fejl", "Error handling", "🛡️", 5),
        new("strings", "Strings", "🔤", 6),
        new("projekter", "Mini projects", "🚀", 7),
    ];

    public static string? GetTitle(string? key) =>
        All.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.OrdinalIgnoreCase))?.Title;
}
