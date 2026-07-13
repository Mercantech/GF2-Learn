using System.Text.Json;
using GF2Learn.Web.Client.Models;

namespace GF2Learn.Web.Client.Services;

public static class LogParserLogic
{
    public static LogParserReport Parse(string content, string format, string sourceFile) =>
        format switch
        {
            "json" => ParseJson(content, sourceFile),
            "csv" => ParseCsv(content, sourceFile),
            _ => ParseTxt(content, sourceFile)
        };

    public static LogParserReport ParseTxt(string content, string sourceFile)
    {
        var info = 0;
        var warn = 0;
        var error = 0;
        var errorLines = new List<string>();

        foreach (var raw in content.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            if (line.Contains("[ERROR]", StringComparison.Ordinal))
            {
                error++;
                errorLines.Add(line);
            }
            else if (line.Contains("[WARN]", StringComparison.Ordinal))
            {
                warn++;
            }
            else if (line.Contains("[INFO]", StringComparison.Ordinal))
            {
                info++;
            }
        }

        return new LogParserReport(info, warn, error, TakeRecentErrors(errorLines), "txt", sourceFile);
    }

    public static LogParserReport ParseJson(string content, string sourceFile)
    {
        var info = 0;
        var warn = 0;
        var error = 0;
        var errorLines = new List<string>();

        using var doc = JsonDocument.Parse(content);
        foreach (var entry in doc.RootElement.EnumerateArray())
        {
            var level = entry.GetProperty("level").GetString() ?? "";
            var message = entry.TryGetProperty("message", out var msg) ? msg.GetString() ?? "" : "";
            var timestamp = entry.TryGetProperty("timestamp", out var ts) ? ts.GetString() ?? "" : "";
            var line = $"[{level}] {timestamp} {message}".Trim();

            switch (level)
            {
                case "ERROR":
                    error++;
                    errorLines.Add(line);
                    break;
                case "WARN":
                    warn++;
                    break;
                case "INFO":
                    info++;
                    break;
            }
        }

        return new LogParserReport(info, warn, error, TakeRecentErrors(errorLines), "json", sourceFile);
    }

    public static LogParserReport ParseCsv(string content, string sourceFile)
    {
        var info = 0;
        var warn = 0;
        var error = 0;
        var errorLines = new List<string>();
        var lines = content.Replace("\r\n", "\n").Split('\n');

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0)
                continue;

            var parts = line.Split(',', 3);
            if (parts.Length < 3)
                continue;

            var level = parts[0].Trim();
            var display = $"[{level}] {parts[1].Trim()} {parts[2].Trim()}";

            switch (level)
            {
                case "ERROR":
                    error++;
                    errorLines.Add(display);
                    break;
                case "WARN":
                    warn++;
                    break;
                case "INFO":
                    info++;
                    break;
            }
        }

        return new LogParserReport(info, warn, error, TakeRecentErrors(errorLines), "csv", sourceFile);
    }

    private static IReadOnlyList<string> TakeRecentErrors(List<string> errorLines) =>
        errorLines.TakeLast(5).ToList();
}
