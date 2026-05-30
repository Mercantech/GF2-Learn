using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace GF2Learn.Web.Services;

public sealed partial class ContentPreprocessor
{
    private static readonly MarkdownPipeline InlinePipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    private static string RenderInlineMarkdown(string body) =>
        string.IsNullOrWhiteSpace(body) ? string.Empty : Markdown.ToHtml(body.Trim(), InlinePipeline);

    public string Process(string markdown)
    {
        return DirectiveRegex().Replace(markdown, match =>
        {
            var name = match.Groups["name"].Value;
            var args = match.Groups["args"].Value.Trim();
            var body = match.Groups["body"].Value.Trim();
            return name switch
            {
                "callout" => BuildCallout(args, body),
                "git-step" => BuildGitStep(body),
                "solution" => BuildSolution(body),
                "code-playground" => string.Empty,
                "related-pensum" => BuildRelatedPensum(body),
                "exercise" => BuildExercise(args, body),
                _ => match.Value
            };
        });
    }

    private static string BuildCallout(string args, string body)
    {
        var type = ParseArg(args, "type") ?? "info";
        return $"<div class=\"callout callout-{WebUtility.HtmlEncode(type)}\">{RenderInlineMarkdown(body)}</div>\n\n";
    }

    private static string BuildExercise(string args, string body)
    {
        var level = ParseArg(args, "level") ?? "begynder";
        return $"<div class=\"exercise-card\" data-level=\"{WebUtility.HtmlEncode(level)}\"><p class=\"exercise-label\">Opgave · {WebUtility.HtmlEncode(level)}</p>{RenderInlineMarkdown(body)}</div>\n\n";
    }

    private static string BuildGitStep(string body)
    {
        var commit = ParseLine(body, "commit");
        var branch = ParseLine(body, "branch") ?? "main";
        var sb = new StringBuilder();
        sb.Append("<div class=\"git-step\"><p class=\"git-step-label\">Git-trin</p><ul>");
        if (!string.IsNullOrWhiteSpace(branch))
            sb.Append($"<li><strong>Branch:</strong> <code>{WebUtility.HtmlEncode(branch)}</code></li>");
        if (!string.IsNullOrWhiteSpace(commit))
            sb.Append($"<li><strong>Commit:</strong> <code>{WebUtility.HtmlEncode(commit)}</code></li>");
        sb.Append("</ul>");
        var extra = body.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(l => !l.StartsWith("commit:", StringComparison.OrdinalIgnoreCase) && !l.StartsWith("branch:", StringComparison.OrdinalIgnoreCase));
        foreach (var line in extra)
            sb.Append($"<p>{WebUtility.HtmlEncode(line)}</p>");
        sb.Append("</div>\n\n");
        return sb.ToString();
    }

    private static string BuildSolution(string body)
        => $"<details class=\"solution\"><summary>Vis løsningsforslag</summary><div class=\"solution-body\">{RenderInlineMarkdown(body)}</div></details>\n\n";

    private static string BuildRelatedPensum(string body)
    {
        var links = body.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(l => l.TrimStart('-', ' ').Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l));
        var sb = new StringBuilder("<div class=\"related-pensum\"><p class=\"related-label\">Related curriculum</p><ul>");
        foreach (var slug in links)
            sb.Append($"<li><a href=\"/curriculum/{WebUtility.HtmlEncode(slug)}\">{WebUtility.HtmlEncode(slug)}</a></li>");
        sb.Append("</ul></div>\n\n");
        return sb.ToString();
    }

    private static string? ParseArg(string args, string key)
    {
        var match = Regex.Match(args, $@"{key}=""([^""]+)""|{key}=([^\s]+)");
        return match.Success ? (match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value) : null;
    }

    private static string? ParseLine(string body, string key)
    {
        var match = Regex.Match(body, $@"^{key}:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim().Trim('"') : null;
    }
}

