using System.Text.RegularExpressions;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed partial class ExerciseAiContextService(ContentService content)
{
    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    public ExercisePartAiContext? GetPartContext(string contentSlug, int partIndex)
    {
        var item = content.GetExercise(contentSlug);
        if (item is null)
            return null;

        return ParseParts(item.Body).FirstOrDefault(p => p.PartIndex == partIndex);
    }

    public IReadOnlyList<ExercisePartAiContext> ParseParts(string markdown)
    {
        var matches = DirectiveRegex().Matches(markdown).Cast<Match>().ToList();
        var parts = new List<ExercisePartAiContext>();
        var index = 0;

        for (var i = 0; i < matches.Count; i++)
        {
            if (!matches[i].Groups["name"].Value.Equals("exercise", StringComparison.OrdinalIgnoreCase))
                continue;

            if (i + 1 >= matches.Count
                || !matches[i + 1].Groups["name"].Value.Equals("code-playground", StringComparison.OrdinalIgnoreCase))
                continue;

            var taskBody = matches[i].Groups["body"].Value.Trim();
            var starter = ExtractCode(matches[i + 1].Groups["body"].Value.Trim());

            string? solution = null;
            if (i + 2 < matches.Count
                && matches[i + 2].Groups["name"].Value.Equals("solution", StringComparison.OrdinalIgnoreCase))
            {
                solution = ExtractSolutionCode(matches[i + 2].Groups["body"].Value.Trim());
            }

            parts.Add(new ExercisePartAiContext(
                index,
                ToPlainTask(taskBody),
                starter,
                solution));

            index++;
        }

        return parts;
    }

    private static string ExtractCode(string body)
    {
        var match = Regex.Match(
            body,
            @"```(?:csharp|cs)?\s*\r?\n([\s\S]*?)\r?\n```",
            RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : body.Trim();
    }

    private static string? ExtractSolutionCode(string body)
    {
        var code = ExtractCode(body);
        return string.IsNullOrWhiteSpace(code) ? null : code;
    }

    private static string ToPlainTask(string markdown) =>
        Regex.Replace(markdown, @"\*\*([^*]+)\*\*", "$1")
            .Replace('`', ' ')
            .Trim();
}
