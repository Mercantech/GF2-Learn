using System.Text.RegularExpressions;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed partial class ExercisePageBuilder(ContentPreprocessor preprocessor)
{
    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    public IReadOnlyList<ExerciseBlockTransfer> BuildTransfers(string markdown, string contentSlug, Func<string, string> renderMarkdown) =>
        Build(markdown, contentSlug, renderMarkdown)
            .Select(ToTransfer)
            .ToList();

    public IReadOnlyList<ExercisePageBlock> Build(string markdown, string contentSlug, Func<string, string> renderMarkdown)
    {
        var blocks = new List<ExercisePageBlock>();
        var partIndex = 0;
        var lastIndex = 0;

        foreach (Match match in DirectiveRegex().Matches(markdown))
        {
            if (match.Index > lastIndex)
            {
                var text = markdown[lastIndex..match.Index].Trim();
                if (text.Length > 0)
                    AppendMarkdown(blocks, renderMarkdown(text));
            }

            var name = match.Groups["name"].Value;
            var args = match.Groups["args"].Value.Trim();
            var body = match.Groups["body"].Value.Trim();

            switch (name.ToLowerInvariant())
            {
                case "exercise":
                    blocks.Add(new ExerciseCardBlock(partIndex, ParseLevel(args), preprocessor.BuildExerciseCardHtml(args, body)));
                    break;
                case "code-playground":
                    blocks.Add(new ExerciseEditorBlock(partIndex, ExtractPlaygroundCode(body)));
                    partIndex++;
                    break;
                case "solution":
                    blocks.Add(new ExerciseSolutionBlock(preprocessor.BuildSolutionHtml(body)));
                    break;
                case "callout":
                case "git-step":
                case "related-pensum":
                    AppendMarkdown(blocks, preprocessor.BuildDirectiveHtml(name, args, body));
                    break;
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < markdown.Length)
        {
            var tail = markdown[lastIndex..].Trim();
            if (tail.Length > 0)
                AppendMarkdown(blocks, renderMarkdown(tail));
        }

        return blocks;
    }

    private static ExerciseBlockTransfer ToTransfer(ExercisePageBlock block) => block switch
    {
        ExerciseMarkdownBlock md => new("markdown", 0, md.Html),
        ExerciseCardBlock card => new("card", card.PartIndex, card.Html, card.Level),
        ExerciseEditorBlock editor => new("editor", editor.PartIndex, editor.Code),
        ExerciseSolutionBlock solution => new("solution", 0, solution.Html),
        _ => new("markdown", 0, string.Empty)
    };

    private static void AppendMarkdown(List<ExercisePageBlock> blocks, string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return;
        blocks.Add(new ExerciseMarkdownBlock(html));
    }

    private static string ParseLevel(string args)
    {
        var match = Regex.Match(args, @"level=""([^""]+)""|level=(\S+)");
        return match.Success
            ? (match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value)
            : "begynder";
    }

    private static string ExtractPlaygroundCode(string body)
    {
        var codeMatch = Regex.Match(
            body,
            @"```(?:csharp|cs)?\s*\r?\n([\s\S]*?)\r?\n```",
            RegexOptions.IgnoreCase);
        return codeMatch.Success ? codeMatch.Groups[1].Value.Trim() : body.Trim();
    }
}
