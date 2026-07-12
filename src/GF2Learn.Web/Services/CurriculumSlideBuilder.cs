using System.Text.RegularExpressions;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed partial class CurriculumSlideBuilder
{
    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    [GeneratedRegex(@"^##\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex SectionHeadingRegex();

    [GeneratedRegex(@"^###\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex SubsectionHeadingRegex();

    private readonly MarkdownService _markdown;
    private readonly RunnableCodeParser _runnableParser;
    private readonly MarkdownCodeHighlighter _highlighter;
    private readonly ContentPreprocessor _preprocessor;

    public CurriculumSlideBuilder(
        MarkdownService markdown,
        RunnableCodeParser runnableParser,
        MarkdownCodeHighlighter highlighter,
        ContentPreprocessor preprocessor)
    {
        _markdown = markdown;
        _runnableParser = runnableParser;
        _highlighter = highlighter;
        _preprocessor = preprocessor;
    }

    public IReadOnlyList<CurriculumSlide> Build(ContentItem item)
    {
        var slides = new List<CurriculumSlide>();
        var intro = ExtractIntro(item.Body);

        slides.Add(new CurriculumSlide
        {
            Kind = CurriculumSlideKind.Title,
            Title = item.Title,
            Subtitle = intro,
            LearningGoals = item.Kompetencemaal
        });

        if (!string.IsNullOrWhiteSpace(item.YoutubeId))
        {
            slides.Add(new CurriculumSlide
            {
                Kind = CurriculumSlideKind.Video,
                Title = item.Title,
                Subtitle = "Video",
                YoutubeId = item.YoutubeId,
                VideoTitle = item.Title
            });
        }

        foreach (var (sectionTitle, sectionBody) in SplitSections(item.Body))
            ProcessSection(sectionTitle, sectionBody, item.Slug, slides);

        return slides;
    }

    private static string? ExtractIntro(string body)
    {
        var firstSection = SectionHeadingRegex().Match(body);
        var introSource = firstSection.Success
            ? body[..firstSection.Index]
            : body;

        var lines = introSource
            .Split('\n', StringSplitOptions.TrimEntries)
            .Where(line => !line.StartsWith('#'))
            .ToList();

        if (lines.Count == 0)
            return null;

        return string.Join("\n\n", lines.Take(2));
    }

    private static IEnumerable<(string Title, string Body)> SplitSections(string body)
    {
        var matches = SectionHeadingRegex().Matches(body);
        if (matches.Count == 0)
            yield break;

        for (var i = 0; i < matches.Count; i++)
        {
            var title = matches[i].Groups[1].Value.Trim();
            var start = matches[i].Index + matches[i].Length;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : body.Length;
            var sectionBody = body[start..end].Trim();
            if (sectionBody.Length > 0)
                yield return (title, sectionBody);
        }
    }

    private void ProcessSection(string sectionTitle, string sectionBody, string slug, List<CurriculumSlide> slides)
    {
        var lastIndex = 0;

        foreach (Match match in DirectiveRegex().Matches(sectionBody))
        {
            if (match.Index > lastIndex)
            {
                var markdownChunk = sectionBody[lastIndex..match.Index].Trim();
                AddMarkdownSlides(sectionTitle, markdownChunk, slug, slides);
            }

            var name = match.Groups["name"].Value;
            var directiveBody = match.Groups["body"].Value.Trim();

            switch (name)
            {
                case "video-list":
                    AddVideoSlides(sectionTitle, directiveBody, slides);
                    break;
                case "knowledge-check":
                    AddQuizSlides(sectionTitle, directiveBody, slug, slides);
                    break;
                default:
                    var html = name switch
                    {
                        "callout" => _preprocessor.BuildDirectiveHtml("callout", match.Groups["args"].Value.Trim(), directiveBody),
                        "git-step" => _preprocessor.BuildDirectiveHtml("git-step", "", directiveBody),
                        "related-pensum" => _preprocessor.BuildDirectiveHtml("related-pensum", "", directiveBody),
                        _ => string.Empty
                    };

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        slides.Add(new CurriculumSlide
                        {
                            Kind = CurriculumSlideKind.Content,
                            Title = sectionTitle,
                            Segments = [new ContentSegment { Html = html }]
                        });
                    }
                    break;
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < sectionBody.Length)
        {
            var tail = sectionBody[lastIndex..].Trim();
            AddMarkdownSlides(sectionTitle, tail, slug, slides);
        }
    }

    private void AddMarkdownSlides(string sectionTitle, string markdown, string slug, List<CurriculumSlide> slides)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return;

        var chunks = SplitSubsections(markdown);
        foreach (var (subtitle, chunk) in chunks)
        {
            var html = _markdown.ToHtml(chunk, slug);
            var segments = HighlightSegments(_runnableParser.BuildSegments(html));
            var htmlBuffer = new List<ContentSegment>();

            foreach (var segment in segments)
            {
                if (segment.IsRunnableSnippet && segment.RunnableSnippet is not null)
                {
                    if (htmlBuffer.Count > 0)
                    {
                        slides.Add(CreateContentSlide(sectionTitle, subtitle, htmlBuffer));
                        htmlBuffer = [];
                    }

                    slides.Add(new CurriculumSlide
                    {
                        Kind = CurriculumSlideKind.CodeExample,
                        Title = sectionTitle,
                        Subtitle = subtitle ?? "Prøv koden",
                        Segments = [segment]
                    });
                }
                else if (!string.IsNullOrWhiteSpace(segment.Html))
                {
                    htmlBuffer.Add(segment);
                }
            }

            if (htmlBuffer.Count > 0)
                slides.Add(CreateContentSlide(sectionTitle, subtitle, htmlBuffer));
        }
    }

    private static IEnumerable<(string? Subtitle, string Markdown)> SplitSubsections(string markdown)
    {
        var matches = SubsectionHeadingRegex().Matches(markdown);
        if (matches.Count == 0)
        {
            yield return (null, markdown);
            yield break;
        }

        var preamble = markdown[..matches[0].Index].Trim();
        if (preamble.Length > 0)
            yield return (null, preamble);

        for (var i = 0; i < matches.Count; i++)
        {
            var subtitle = matches[i].Groups[1].Value.Trim();
            var start = matches[i].Index;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : markdown.Length;
            var chunk = markdown[start..end].Trim();
            if (chunk.Length > 0)
                yield return (subtitle, chunk);
        }
    }

    private void AddVideoSlides(string sectionTitle, string body, List<CurriculumSlide> slides)
    {
        foreach (var (title, id) in _preprocessor.ParseVideoList(body))
        {
            slides.Add(new CurriculumSlide
            {
                Kind = CurriculumSlideKind.Video,
                Title = sectionTitle,
                Subtitle = title,
                YoutubeId = id,
                VideoTitle = title
            });
        }
    }

    private void AddQuizSlides(string sectionTitle, string body, string slug, List<CurriculumSlide> slides)
    {
        foreach (var questionHtml in _preprocessor.BuildKnowledgeCheckQuestionSlides(body, slug))
        {
            slides.Add(new CurriculumSlide
            {
                Kind = CurriculumSlideKind.Quiz,
                Title = sectionTitle,
                Subtitle = "Test din viden",
                QuizHtml = questionHtml
            });
        }
    }

    private static CurriculumSlide CreateContentSlide(
        string sectionTitle,
        string? subtitle,
        IReadOnlyList<ContentSegment> segments) =>
        new()
        {
            Kind = CurriculumSlideKind.Content,
            Title = sectionTitle,
            Subtitle = subtitle,
            Segments = segments
        };

    private IReadOnlyList<ContentSegment> HighlightSegments(IReadOnlyList<ContentSegment> segments) =>
        segments.Select(segment =>
        {
            if (string.IsNullOrWhiteSpace(segment.Html))
                return segment;

            return new ContentSegment
            {
                Html = _highlighter.Apply(segment.Html),
                Playground = segment.Playground,
                RunnableSnippet = segment.RunnableSnippet
            };
        }).ToList();
}
