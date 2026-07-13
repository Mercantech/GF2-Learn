namespace GF2Learn.Web.Client.Models;

public static class LogParserSamples
{
    public const string BasePath = "/project-samples/projekt-log-parser/";

    public static readonly IReadOnlyList<LogParserSampleFile> Files =
    [
        new("app.log.txt", "Tekstlog", "txt"),
        new("app.log.json", "JSON-log", "json"),
        new("app.log.csv", "CSV-log", "csv")
    ];

    public static string Url(string fileName) => BasePath + fileName;
}

public sealed record LogParserSampleFile(string FileName, string Description, string Format);

public sealed record LogParserReport(
    int Info,
    int Warn,
    int Error,
    IReadOnlyList<string> ErrorLines,
    string Format,
    string SourceFile)
{
    public int Total => Info + Warn + Error;

    public string Summary => $"INFO: {Info}, WARN: {Warn}, ERROR: {Error}";

    public string ToDisplayReport()
    {
        var lines = new List<string>
        {
            $"Kilde: {SourceFile} ({Format})",
            Summary,
            $"Linjer i alt: {Total}"
        };

        if (ErrorLines.Count > 0)
        {
            lines.Add("");
            lines.Add("Seneste fejl:");
            lines.AddRange(ErrorLines);
        }

        return string.Join('\n', lines);
    }
}
