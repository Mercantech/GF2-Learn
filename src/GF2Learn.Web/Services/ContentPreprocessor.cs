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

    public string Process(string markdown, string? contentSlug = null)
    {
        var exercisePartIndex = 0;
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
                "exercise" => BuildExercise(args, body, contentSlug, exercisePartIndex++),
                "knowledge-check" => BuildKnowledgeCheck(body, contentSlug),
                _ => match.Value
            };
        });
    }

    private static string BuildCallout(string args, string body)
    {
        var type = ParseArg(args, "type") ?? "info";
        return $"<div class=\"callout callout-{WebUtility.HtmlEncode(type)}\">{RenderInlineMarkdown(body)}</div>\n\n";
    }

    private static string BuildExercise(string args, string body, string? contentSlug, int partIndex)
    {
        var level = ParseArg(args, "level") ?? "begynder";
        var slugAttr = string.IsNullOrWhiteSpace(contentSlug)
            ? string.Empty
            : $" data-content-slug=\"{WebUtility.HtmlEncode(contentSlug)}\"";
        var partNum = partIndex + 1;

        return $"""
            <section class="exercise-part"{slugAttr} data-part-index="{partIndex}">
            <div class="exercise-card" data-level="{WebUtility.HtmlEncode(level)}">
            <p class="exercise-label">Opgave · {WebUtility.HtmlEncode(level)} <span class="exercise-part-badge">del {partNum}</span></p>
            {RenderInlineMarkdown(body)}
            </div>
            <div class="exercise-save-panel">
            <label class="exercise-save-label" for="exercise-answer-{WebUtility.HtmlEncode(contentSlug ?? "x")}-{partIndex}">Din løsning</label>
            <textarea id="exercise-answer-{WebUtility.HtmlEncode(contentSlug ?? "x")}-{partIndex}" class="exercise-answer-input" rows="5" placeholder="Indsæt din kode eller kort beskrivelse af, hvad du har lavet…"></textarea>
            <div class="exercise-save-actions">
            <button type="button" class="btn btn-primary btn-sm exercise-save-btn">Gem som løst</button>
            <span class="exercise-saved-badge" hidden aria-live="polite">✓ Gemt</span>
            </div>
            </div>
            </section>

            """;
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

    private static string BuildKnowledgeCheck(string body, string? contentSlug)
    {
        var blocks = body.Split("---", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sb = new StringBuilder();
        var slugAttr = string.IsNullOrWhiteSpace(contentSlug)
            ? string.Empty
            : $" data-content-slug=\"{WebUtility.HtmlEncode(contentSlug)}\"";
        sb.Append($"<section class=\"knowledge-check\"{slugAttr}><h2 class=\"kc-heading\">Test din viden</h2>");
        sb.Append("<p class=\"kc-progress\" hidden></p>");

        var questionNumber = 0;
        foreach (var block in blocks)
        {
            var lines = block.Split('\n', StringSplitOptions.TrimEntries);
            string? question = null;
            var options = new List<string>();
            int? correct = null;
            var explainLines = new List<string>();
            var inExplanation = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("q:", StringComparison.OrdinalIgnoreCase))
                {
                    question = line[2..].Trim();
                    inExplanation = false;
                }
                else if (line.StartsWith("correct:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(line["correct:".Length..].Trim(), out var index))
                        correct = index;
                    inExplanation = false;
                }
                else if (line.StartsWith("explain:", StringComparison.OrdinalIgnoreCase))
                {
                    explainLines.Add(line["explain:".Length..].Trim());
                    inExplanation = true;
                }
                else if (inExplanation)
                {
                    explainLines.Add(line);
                }
                else if (line.StartsWith("- "))
                {
                    options.Add(line[2..].Trim());
                }
            }

            if (string.IsNullOrWhiteSpace(question) || options.Count == 0 || correct is null)
                continue;

            if (correct < 0 || correct >= options.Count)
                continue;

            questionNumber++;
            var questionIndex = questionNumber - 1;
            sb.Append($"<div class=\"kc-question\" data-correct=\"{correct.Value}\" data-question-index=\"{questionIndex}\">");
            sb.Append($"<p class=\"kc-number\">Spørgsmål {questionNumber}</p>");
            sb.Append($"<div class=\"kc-prompt\">{RenderInlineMarkdown(question)}</div>");
            sb.Append("<ul class=\"kc-options\">");
            for (var i = 0; i < options.Count; i++)
                sb.Append($"<li><button type=\"button\" class=\"kc-option\" data-original-index=\"{i}\">{RenderInlineMarkdown(options[i])}</button></li>");
            sb.Append("</ul>");
            var explanation = string.Join("\n", explainLines);
            sb.Append("<div class=\"kc-feedback\" hidden>");
            sb.Append("<p class=\"kc-verdict\"></p>");
            sb.Append($"<div class=\"kc-explanation\">{RenderInlineMarkdown(explanation)}</div>");
            sb.Append("</div></div>");
        }

        sb.Append("</section>\n\n");
        return sb.ToString();
    }
}

