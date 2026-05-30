namespace GF2Learn.Web.Models;

public enum ContentSectionType
{
    Curriculum,
    Exercises,
    Projects
}

public sealed class ContentItem
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public ContentSectionType Section { get; init; }
    public int Order { get; init; }
    public string? Difficulty { get; init; }
    public string? ProjectSlug { get; init; }
    public bool IsOverview { get; init; }
    public List<string> Topics { get; init; } = [];
    public List<string> Kompetencemaal { get; init; } = [];
    public List<string> RelatedPensum { get; init; } = [];
    public int Timer { get; init; }
    public string? YoutubeId { get; init; }
    public string Body { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;

    public string Href => Section switch
    {
        ContentSectionType.Curriculum => $"/curriculum/{Slug}",
        ContentSectionType.Exercises => $"/exercises/{Slug}",
        ContentSectionType.Projects when !string.IsNullOrEmpty(ProjectSlug) && !IsOverview
            => $"/projects/{ProjectSlug}/{Slug}",
        ContentSectionType.Projects => $"/projects/{Slug}",
        _ => "/"
    };
}

public sealed class NavGroup
{
    public required string Title { get; init; }
    public List<ContentItem> Items { get; init; } = [];
}

public sealed class SectionNav
{
    public ContentSectionType Section { get; init; }
    public List<NavGroup> Groups { get; init; } = [];
}
