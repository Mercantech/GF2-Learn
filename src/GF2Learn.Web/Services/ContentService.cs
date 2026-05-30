using System.Text.RegularExpressions;
using GF2Learn.Web.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GF2Learn.Web.Services;

public sealed class ContentService(IWebHostEnvironment env, IConfiguration config)
{
    private readonly IDeserializer _yaml = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private string ContentRoot
    {
        get
        {
            var configured = config["ContentPath"];
            if (!string.IsNullOrWhiteSpace(configured) && Directory.Exists(configured))
                return Path.GetFullPath(configured);

            var candidates = new[]
            {
                Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "content")),
                Path.GetFullPath(Path.Combine(env.ContentRootPath, "content")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "content"))
            };

            return candidates.FirstOrDefault(Directory.Exists) ?? candidates[0];
        }
    }

    public IReadOnlyList<ContentItem> GetAll()
    {
        var items = new List<ContentItem>();
        items.AddRange(LoadSection(ContentSectionType.Curriculum, Path.Combine(ContentRoot, "pensum")));
        items.AddRange(LoadExercises(Path.Combine(ContentRoot, "opgaver")));
        items.AddRange(LoadProjects(Path.Combine(ContentRoot, "projekter")));
        return items.OrderBy(i => i.Section).ThenBy(i => i.Order).ThenBy(i => i.Title).ToList();
    }

    public ContentItem? GetCurriculum(string slug) =>
        GetAll().FirstOrDefault(i => i.Section == ContentSectionType.Curriculum && i.Slug == slug);

    public ContentItem? GetExercise(string slug) =>
        GetAll().FirstOrDefault(i => i.Section == ContentSectionType.Exercises && i.Slug == slug);

    public ContentItem? GetProjectOverview(string projectSlug) =>
        GetAll().FirstOrDefault(i => i.Section == ContentSectionType.Projects && i.Slug == projectSlug && i.IsOverview);

    public ContentItem? GetProjectDay(string projectSlug, string daySlug) =>
        GetAll().FirstOrDefault(i => i.Section == ContentSectionType.Projects && i.ProjectSlug == projectSlug && i.Slug == daySlug);

    private IEnumerable<ContentItem> LoadSection(ContentSectionType section, string dir)
    {
        if (!Directory.Exists(dir)) yield break;
        foreach (var file in Directory.GetFiles(dir, "*.md", SearchOption.TopDirectoryOnly).OrderBy(f => f))
        {
            var item = ParseFile(file, section);
            if (item != null) yield return item;
        }
    }

    private IEnumerable<ContentItem> LoadExercises(string dir)
    {
        if (!Directory.Exists(dir)) yield break;
        foreach (var file in Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories).OrderBy(f => f))
        {
            var item = ParseFile(file, ContentSectionType.Exercises);
            if (item != null) yield return item;
        }
    }

    private IEnumerable<ContentItem> LoadProjects(string dir)
    {
        if (!Directory.Exists(dir)) yield break;
        foreach (var projectDir in Directory.GetDirectories(dir).OrderBy(d => d))
        {
            var projectSlug = Path.GetFileName(projectDir);
            foreach (var file in Directory.GetFiles(projectDir, "*.md").OrderBy(f => f))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var isOverview = fileName.Equals("overview", StringComparison.OrdinalIgnoreCase);
                var item = ParseFile(file, ContentSectionType.Projects, projectSlug, isOverview ? projectSlug : fileName, isOverview);
                if (item != null) yield return item;
            }
        }
    }

    private ContentItem? ParseFile(string path, ContentSectionType section, string? projectSlug = null, string? slugOverride = null, bool isOverview = false)
    {
        var raw = File.ReadAllText(path);
        var (frontmatter, body) = SplitFrontmatter(raw);
        var data = string.IsNullOrWhiteSpace(frontmatter)
            ? new Dictionary<string, object?>()
            : _yaml.Deserialize<Dictionary<string, object?>>(frontmatter);

        var slug = slugOverride ?? Path.GetFileNameWithoutExtension(path);
        var title = GetString(data, "title") ?? slug;
        var order = GetInt(data, "order");

        return new ContentItem
        {
            Slug = slug,
            Title = title,
            Section = section,
            Order = order,
            Difficulty = GetString(data, "difficulty"),
            ProjectSlug = projectSlug,
            IsOverview = isOverview,
            Topics = GetStringList(data, "topics"),
            Kompetencemaal = GetStringList(data, "kompetencemaal"),
            RelatedPensum = GetStringList(data, "related_pensum"),
            Timer = GetInt(data, "timer"),
            YoutubeId = GetString(data, "youtube_id"),
            Body = body.Trim(),
            FilePath = path
        };
    }

    private static (string Frontmatter, string Body) SplitFrontmatter(string raw)
    {
        var match = Regex.Match(raw, @"^---\r?\n([\s\S]*?)\r?\n---\r?\n([\s\S]*)$");
        return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : (string.Empty, raw);
    }

    private static string? GetString(Dictionary<string, object?> data, string key)
        => data.TryGetValue(key, out var v) ? v?.ToString() : null;

    private static int GetInt(Dictionary<string, object?> data, string key)
        => data.TryGetValue(key, out var v) && int.TryParse(v?.ToString(), out var n) ? n : 0;

    private static List<string> GetStringList(Dictionary<string, object?> data, string key)
    {
        if (!data.TryGetValue(key, out var v) || v is null) return [];
        if (v is IEnumerable<object> list) return list.Select(x => x.ToString() ?? "").Where(x => x.Length > 0).ToList();
        return [];
    }
}
