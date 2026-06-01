using System.Text.RegularExpressions;

namespace GF2Learn.Web.Services;

public sealed partial class ExerciseCatalog
{
    [GeneratedRegex(@"^:::\s*exercise\b", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex ExerciseDirectiveRegex();

    public int CountParts(string markdown) => ExerciseDirectiveRegex().Matches(markdown).Count;
}
