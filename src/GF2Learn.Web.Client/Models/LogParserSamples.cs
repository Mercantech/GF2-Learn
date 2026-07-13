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

    /// <summary>
    /// Indlejret indhold til Blazor-demos (kører på server uden WASM HttpClient).
    /// Matcher filerne under wwwroot/project-samples/projekt-log-parser/.
    /// </summary>
    public static string GetContent(string fileName) =>
        fileName.Trim().ToLowerInvariant() switch
        {
            "app.log.txt" => TxtContent,
            "app.log.json" => JsonContent,
            "app.log.csv" => CsvContent,
            _ => throw new KeyNotFoundException($"Ukendt eksempelfil: {fileName}")
        };

    private const string TxtContent = """
        [INFO] 2026-03-10 08:00:01 Server startet
        [WARN] 2026-03-10 08:15:22 Disk 80% fuld
        [ERROR] 2026-03-10 09:02:11 Login fejlede for bruger elev1
        [INFO] 2026-03-10 09:05:00 Bruger logget ind
        [ERROR] 2026-03-10 10:30:44 Timeout mod database
        [WARN] 2026-03-10 11:00:00 Cache næsten fuld
        [INFO] 2026-03-10 12:00:00 Backup fuldført
        [ERROR] 2026-03-10 14:22:18 Database forbindelse afbrudt
        """;

    private const string JsonContent = """
        [
          { "level": "INFO", "timestamp": "2026-03-10T08:00:01", "message": "Server startet" },
          { "level": "WARN", "timestamp": "2026-03-10T08:15:22", "message": "Disk 80% fuld" },
          { "level": "ERROR", "timestamp": "2026-03-10T09:02:11", "message": "Login fejlede for bruger elev1" },
          { "level": "INFO", "timestamp": "2026-03-10T09:05:00", "message": "Bruger logget ind" },
          { "level": "ERROR", "timestamp": "2026-03-10T10:30:44", "message": "Timeout mod database" },
          { "level": "WARN", "timestamp": "2026-03-10T11:00:00", "message": "Cache næsten fuld" },
          { "level": "INFO", "timestamp": "2026-03-10T12:00:00", "message": "Backup fuldført" },
          { "level": "ERROR", "timestamp": "2026-03-10T14:22:18", "message": "Database forbindelse afbrudt" }
        ]
        """;

    private const string CsvContent = """
        level,timestamp,message
        INFO,2026-03-10 08:00:01,Server startet
        WARN,2026-03-10 08:15:22,Disk 80% fuld
        ERROR,2026-03-10 09:02:11,Login fejlede for bruger elev1
        INFO,2026-03-10 09:05:00,Bruger logget ind
        ERROR,2026-03-10 10:30:44,Timeout mod database
        WARN,2026-03-10 11:00:00,Cache næsten fuld
        INFO,2026-03-10 12:00:00,Backup fuldført
        ERROR,2026-03-10 14:22:18,Database forbindelse afbrudt
        """;
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
