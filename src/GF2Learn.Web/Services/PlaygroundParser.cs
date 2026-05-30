using System.Text.RegularExpressions;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed partial class PlaygroundParser
{
    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    [GeneratedRegex(@"<gf2-playground\s+data-index=""(\d+)""\s*/>", RegexOptions.IgnoreCase)]
    private static partial Regex PlaceholderRegex();

    /// <summary>
    /// Replaces code-playground directives with HTML comments, leaving other directives for the markdown pipeline.
    /// </summary>
    public (string Markdown, IReadOnlyList<PlaygroundBlock> Playgrounds) ExtractPlaygrounds(string markdown)
    {
        var playgrounds = new List<PlaygroundBlock>();
        var index = 0;

        var withPlaceholders = DirectiveRegex().Replace(markdown, match =>
        {
            if (!match.Groups["name"].Value.Equals("code-playground", StringComparison.OrdinalIgnoreCase))
                return match.Value;

            var body = match.Groups["body"].Value.Trim();
            playgrounds.Add(ParsePlayground(body));
            return $"\n\n<gf2-playground data-index=\"{index++}\" />\n\n";
        });

        return (withPlaceholders, playgrounds);
    }

    /// <summary>
    /// Splits rendered HTML at playground placeholder comments.
    /// </summary>
    public IReadOnlyList<ContentSegment> SplitRenderedHtml(string html, IReadOnlyList<PlaygroundBlock> playgrounds)
    {
        var segments = new List<ContentSegment>();
        var lastIndex = 0;

        foreach (Match match in PlaceholderRegex().Matches(html))
        {
            if (match.Index > lastIndex)
            {
                var chunk = html[lastIndex..match.Index].Trim();
                if (chunk.Length > 0)
                    segments.Add(new ContentSegment { Html = chunk });
            }

            if (int.TryParse(match.Groups[1].Value, out var idx) && idx >= 0 && idx < playgrounds.Count)
                segments.Add(new ContentSegment { Playground = playgrounds[idx] });

            lastIndex = match.Index + match.Length;
        }

        var tail = html[lastIndex..].Trim();
        if (tail.Length > 0)
            segments.Add(new ContentSegment { Html = tail });

        if (segments.Count == 0 && html.Trim().Length > 0)
            segments.Add(new ContentSegment { Html = html.Trim() });

        return segments;
    }

    public IReadOnlyList<ContentSegment> BuildSegments(string markdown, Func<string, string> renderMarkdown)
    {
        var (withPlaceholders, playgrounds) = ExtractPlaygrounds(markdown);
        var html = renderMarkdown(withPlaceholders);
        return SplitRenderedHtml(html, playgrounds);
    }

    private static PlaygroundBlock ParsePlayground(string body)
    {
        var (code, expected, refs) = ExtractPlaygroundBody(body);
        return new PlaygroundBlock { Code = code, Expected = expected, Refs = refs };
    }

    private static (string Code, string Expected, IReadOnlyList<string> Refs) ExtractPlaygroundBody(string body)
    {
        var refs = new List<string>();
        foreach (Match refsMatch in Regex.Matches(body, @"^refs:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            refs.AddRange(refsMatch.Groups[1].Value.Split(',', StringSplitOptions.TrimEntries)
                .Select(r => r.Trim()).Where(r => r.Length > 0));
        }

        var expectedMatch = Regex.Match(body, @"^expected:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        var expected = expectedMatch.Success ? expectedMatch.Groups[1].Value.Trim() : string.Empty;

        var codeMatch = Regex.Match(
            body,
            @"```(?:csharp|cs)?\s*\r?\n([\s\S]*?)\r?\n```",
            RegexOptions.IgnoreCase);
        var code = codeMatch.Success
            ? codeMatch.Groups[1].Value.Trim()
            : StripPlaygroundMetadata(body).Trim();

        return (code, expected, refs);
    }

    private static string StripPlaygroundMetadata(string body)
    {
        var stripped = Regex.Replace(body, @"^refs:\s*.+\r?$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        stripped = Regex.Replace(stripped, @"^expected:\s*.+\r?$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        stripped = Regex.Replace(stripped, @"^```(?:csharp|cs)?\s*\r?$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        stripped = Regex.Replace(stripped, @"^```\s*\r?$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return stripped.Trim();
    }
}
