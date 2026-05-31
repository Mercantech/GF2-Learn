using System.Text;
using System.Text.RegularExpressions;

namespace GF2Learn.Web.Client.Services;

/// <summary>
/// Wraps user playground snippets in a compilable entry point.
/// Supports <c>// gf2-setup:</c> (prepended declarations) and <c>// gf2-input:</c> (simulated stdin).
/// </summary>
public static class PlaygroundSourceBuilder
{
    public sealed record PreparedCode(string ExecutableCode, IReadOnlyList<string> StdinLines, bool UsesReadLine);

    public static PreparedCode Prepare(string userCode)
    {
        var (setupLines, body) = ExtractSetup(userCode);
        var executable = setupLines.Count == 0
            ? body
            : string.Join("\n", setupLines) + "\n" + body;

        var (transformed, stdinLines, usesReadLine) = TransformUserCode(executable);
        return new PreparedCode(transformed, stdinLines, usesReadLine);
    }

    public static string BuildEntryPointSource(string userCode)
    {
        var prepared = Prepare(userCode);
        return BuildEntryPointSource(prepared.ExecutableCode, prepared.StdinLines, prepared.UsesReadLine);
    }

    internal static string BuildEntryPointSource(
        string transformedCode,
        IReadOnlyList<string> stdinLines,
        bool usesReadLine)
    {
        var stdinInit = stdinLines.Count == 0
            ? "Array.Empty<string>()"
            : $"new string[] {{ {string.Join(", ", stdinLines.Select(l => $"\"{EscapeCSharpString(l)}\""))} }}";
        var stdinNote = usesReadLine
            ? """
                    if (__stdinIdx > 0)
                        __sb.AppendLine("(Simuleret input: " + string.Join(", ", __stdin.Take(__stdinIdx)) + ")");
"""
            : string.Empty;

        var indented = string.Join("\n", transformedCode.Split('\n').Select(l => "            " + l));
        return $$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public static class __PlaygroundEntry
            {
                private static readonly StringBuilder __sb = new StringBuilder();
                private static readonly string[] __stdin = {{stdinInit}};
                private static int __stdinIdx;

                private static void __Emit(object? value) =>
                    __sb.Append(__Str(value));

                private static void __Emit(string format, params object?[] args) =>
                    __sb.Append(args.Length == 0 ? format : __Format(format, args));

                private static void __EmitLine() => __sb.AppendLine();

                private static void __EmitLine(object? value) =>
                    __sb.AppendLine(__Str(value));

                private static void __EmitLine(string format, params object?[] args) =>
                    __sb.AppendLine(args.Length == 0 ? format : __Format(format, args));

            {{PlaygroundRuntimeHelpers.Source}}

                private static string? __ReadLine()
                {
                    if (__stdinIdx >= __stdin.Length)
                        return string.Empty;
                    return __stdin[__stdinIdx++];
                }

                public static string Run()
                {
                    __sb.Clear();
                    __stdinIdx = 0;
                    try
                    {
            {{indented}}
                    }
                    catch (global::System.Exception __ex)
                    {
                        __sb.AppendLine("Fejl: " + __ex.Message);
                    }
            {{stdinNote}}
                    return __sb.ToString();
                }
            }
            """;
    }

    /// <summary>
    /// Strips playground directives and returns setup lines plus the visible snippet body.
    /// </summary>
    public static (IReadOnlyList<string> SetupLines, string Body) ExtractSetup(string userCode)
    {
        var setupLines = new List<string>();
        var bodyLines = new List<string>();

        foreach (var line in userCode.Replace("\r\n", "\n").Split('\n'))
        {
            var setupMatch = Regex.Match(line, @"^\s*//\s*gf2-setup:\s*(.*)$", RegexOptions.IgnoreCase);
            if (setupMatch.Success)
            {
                var setup = setupMatch.Groups[1].Value.Trim();
                if (setup.Length > 0)
                    setupLines.Add(setup);
                continue;
            }

            bodyLines.Add(line);
        }

        return (setupLines, string.Join("\n", bodyLines).Trim());
    }

    private static (string Code, IReadOnlyList<string> StdinLines, bool UsesReadLine) TransformUserCode(string userCode)
    {
        var stdinLines = ParseSimulatedInput(userCode);
        var usesReadLine = Regex.IsMatch(userCode, @"Console\.ReadLine\s*\(");

        if (usesReadLine && stdinLines.Count == 0)
            stdinLines = ["15"];

        var code = userCode;
        code = Regex.Replace(code, @"Console\.WriteLine", "__EmitLine");
        code = Regex.Replace(code, @"Console\.Write\s*\(", "__Emit(");
        code = Regex.Replace(code, @"Console\.ReadLine\s*\(\s*\)", "__ReadLine()");

        code = Regex.Replace(code, @"\bDateTime\.TryParse\s*\(\s*([^,]+)\s*,\s*out\s+DateTime\s+(\w+)\s*\)",
            "__TryParseDate($1, out __Date $2)");
        code = Regex.Replace(code, @"\bDateTime\.Parse\s*\(", "__ParseDate(");
        code = Regex.Replace(code, @"\bint\.TryParse\s*\(", "__TryParseInt(");
        code = Regex.Replace(code, @"\bint\.Parse\s*\(", "__ParseInt(");
        code = Regex.Replace(code, @"\bdouble\.TryParse\s*\(", "__TryParseDouble(");
        code = Regex.Replace(code, @"\bdouble\.Parse\s*\(", "__ParseDouble(");
        code = Regex.Replace(code, @"\bbool\.TryParse\s*\(", "__TryParseBool(");
        code = Regex.Replace(code, @"\bbool\.Parse\s*\(", "__ParseBool(");

        return (code, stdinLines, usesReadLine);
    }

    private static List<string> ParseSimulatedInput(string userCode)
    {
        var match = Regex.Match(
            userCode,
            @"//\s*gf2-input:\s*(.+)$",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (!match.Success)
            return [];

        return match.Groups[1].Value
            .Split(',')
            .Select(s => s.Trim().Trim('"', '\''))
            .Where(s => s.Length > 0)
            .ToList();
    }

    private static string EscapeCSharpString(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
}
