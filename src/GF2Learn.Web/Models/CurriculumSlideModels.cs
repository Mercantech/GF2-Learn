namespace GF2Learn.Web.Models;

public enum CurriculumSlideKind
{
    Title,
    Content,
    CodeExample,
    Video,
    Quiz
}

public sealed class CurriculumSlide
{
    public required string Title { get; init; }
    public string? Subtitle { get; init; }
    public CurriculumSlideKind Kind { get; init; }
    public IReadOnlyList<ContentSegment> Segments { get; init; } = [];
    public string? YoutubeId { get; init; }
    public string? VideoTitle { get; init; }
    public string? QuizHtml { get; init; }
    public IReadOnlyList<string> LearningGoals { get; init; } = [];
}
