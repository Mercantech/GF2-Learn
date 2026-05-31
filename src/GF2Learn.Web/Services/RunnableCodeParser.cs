using System.Net;
using System.Text.RegularExpressions;
using GF2Learn.Web.Client.Services;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed partial class RunnableCodeParser
{
    [GeneratedRegex(
        @"<pre><code(?:\s[^>]*)?class=""[^""]*\blanguage-(?:csharp|cs)\b[^""]*""(?:\s[^>]*)?>([\s\S]*?)</code></pre>",
        RegexOptions.IgnoreCase)]
    private static partial Regex CSharpPreCodeRegex();

    [GeneratedRegex(@"<gf2-runnable-snippet\s+data-index=""(\d+)""\s*/>", RegexOptions.IgnoreCase)]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"\bclass\s+\w+[\s\S]*\bstatic\s+void\s+Main\s*\(", RegexOptions.IgnoreCase)]
    private static partial Regex FullProgramRegex();

    public IReadOnlyList<ContentSegment> BuildSegments(string markdownHtml)
    {
        var snippets = new List<RunnableSnippetBlock>();
        var index = 0;

        var withPlaceholders = CSharpPreCodeRegex().Replace(markdownHtml, match =>
        {
            var raw = WebUtility.HtmlDecode(match.Groups[1].Value);
            var code = raw.Trim();

            if (!IsRunnableSnippet(code))
                return match.Value;

            snippets.Add(new RunnableSnippetBlock { Code = code });
            return $"<gf2-runnable-snippet data-index=\"{index++}\" />";
        });

        return SplitRenderedHtml(withPlaceholders, snippets);
    }

    public static bool IsRunnableSnippet(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        if (!code.Contains("Console.", StringComparison.Ordinal))
            return false;

        var trimmed = code.TrimStart();
        if (trimmed.StartsWith("namespace ", StringComparison.Ordinal))
            return false;

        if (FullProgramRegex().IsMatch(code))
            return false;

        return PlaygroundCompileChecker.CanCompile(code);
    }

    private static IReadOnlyList<ContentSegment> SplitRenderedHtml(
        string html,
        IReadOnlyList<RunnableSnippetBlock> snippets)
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

            if (int.TryParse(match.Groups[1].Value, out var idx) && idx >= 0 && idx < snippets.Count)
                segments.Add(new ContentSegment { RunnableSnippet = snippets[idx] });

            lastIndex = match.Index + match.Length;
        }

        var tail = html[lastIndex..].Trim();
        if (tail.Length > 0)
            segments.Add(new ContentSegment { Html = tail });

        if (segments.Count == 0 && html.Trim().Length > 0)
            segments.Add(new ContentSegment { Html = html.Trim() });

        return segments;
    }
}
