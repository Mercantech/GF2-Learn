using System.Text.RegularExpressions;

namespace GF2Learn.Web.Services;

public sealed partial class KnowledgeCheckCatalog
{
    [GeneratedRegex(@"^:::(?<name>[a-z-]+)(?:[ \t]+(?<args>[^\r\n]*))?\r?\n(?<body>[\s\S]*?)^:::", RegexOptions.Multiline)]
    private static partial Regex DirectiveRegex();

    [GeneratedRegex(@"^q:", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex QuestionLineRegex();

    public int CountQuestions(string markdown)
    {
        var count = 0;
        foreach (Match match in DirectiveRegex().Matches(markdown))
        {
            if (!match.Groups["name"].Value.Equals("knowledge-check", StringComparison.OrdinalIgnoreCase))
                continue;

            count += QuestionLineRegex().Matches(match.Groups["body"].Value).Count;
        }

        return count;
    }
}
