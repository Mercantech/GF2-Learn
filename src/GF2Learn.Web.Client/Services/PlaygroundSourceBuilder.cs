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

    public sealed record SimulatedStdinField(string Value, string? VariableName, string? Prompt);

    public static PreparedCode Prepare(string userCode)
    {
        var (setupLines, body) = ExtractSetup(userCode);
        var executable = setupLines.Count == 0
            ? body
            : string.Join("\n", setupLines) + "\n" + body;

        var (transformed, stdinLines, usesReadLine) = TransformUserCode(executable);
        return new PreparedCode(transformed, stdinLines, usesReadLine);
    }

    public static string BuildEntryPointSource(string userCode, bool showStdinTraceInOutput = true) =>
        BuildEntryPointSource(Prepare(userCode), showStdinTraceInOutput);

    public static string BuildEntryPointSource(PreparedCode prepared, bool showStdinTraceInOutput = true) =>
        BuildEntryPointSource(prepared.ExecutableCode, prepared.StdinLines, prepared.UsesReadLine, showStdinTraceInOutput);

    public static string BuildExerciseMethodEntryPointSource(string userCode, bool showStdinTraceInOutput = true)
    {
        var prepared = Prepare(userCode);
        var methodCall = ExtractExerciseMethodCall(prepared.ExecutableCode);
        if (methodCall is null)
            return BuildEntryPointSource(prepared, showStdinTraceInOutput);

        return BuildExerciseMethodEntryPointSource(
            prepared.ExecutableCode,
            methodCall,
            prepared.StdinLines,
            prepared.UsesReadLine,
            showStdinTraceInOutput);
    }

    internal static string BuildExerciseMethodEntryPointSource(
        string transformedCode,
        string methodCall,
        IReadOnlyList<string> stdinLines,
        bool usesReadLine,
        bool showStdinTraceInOutput = true)
    {
        var stdinInit = stdinLines.Count == 0
            ? "Array.Empty<string>()"
            : $"new string[] {{ {string.Join(", ", stdinLines.Select(l => $"\"{EscapeCSharpString(l)}\""))} }}";
        var stdinNote = usesReadLine && showStdinTraceInOutput
            ? """
                    if (__stdinIdx > 0)
                        __sb.AppendLine("(Simuleret input: " + string.Join(", ", __stdin.Take(__stdinIdx)) + ")");
"""
            : string.Empty;

        var classBody = string.Join("\n", transformedCode.Split('\n').Select(l => "        " + l));
        return $$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;

            public static class __ExerciseCode
            {
            {{classBody}}
            }

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
                        __ExerciseCode.{{methodCall}}
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

    private static string? ExtractExerciseMethodCall(string code)
    {
        var match = Regex.Match(code, @"public\s+static\s+void\s+(\w+)\s*\(");
        return match.Success ? $"{match.Groups[1].Value}();" : null;
    }

    public static IReadOnlyList<string> GetSimulatedStdinLines(string userCode) =>
        ParseSimulatedInputFields(userCode).Select(f => f.Value).ToList();

    public static IReadOnlyList<SimulatedStdinField> GetSimulatedStdinFields(string userCode)
    {
        var (_, body) = ExtractSetup(userCode);
        var promptsByVar = BuildPromptMap(body);
        return ParseSimulatedInputFields(userCode)
            .Select(f =>
            {
                if (f.VariableName is null || f.Prompt is not null)
                    return f;
                return promptsByVar.TryGetValue(f.VariableName, out var prompt)
                    ? f with { Prompt = prompt }
                    : f;
            })
            .ToList();
    }

    internal static string BuildEntryPointSource(
        string transformedCode,
        IReadOnlyList<string> stdinLines,
        bool usesReadLine,
        bool showStdinTraceInOutput = true)
    {
        var stdinInit = stdinLines.Count == 0
            ? "Array.Empty<string>()"
            : $"new string[] {{ {string.Join(", ", stdinLines.Select(l => $"\"{EscapeCSharpString(l)}\""))} }}";
        var stdinNote = usesReadLine && showStdinTraceInOutput
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
        code = Regex.Replace(
            code,
            @"([^()]+?)\.PadLeft\s*\(\s*(\d+)\s*\)",
            "__PadLeft($1, $2)");

        return (code, stdinLines, usesReadLine);
    }

    private static List<string> ParseSimulatedInput(string userCode) =>
        ParseSimulatedInputFields(userCode).Select(f => f.Value).ToList();

    private static List<SimulatedStdinField> ParseSimulatedInputFields(string userCode)
    {
        var fields = new List<SimulatedStdinField>();
        foreach (Match match in Regex.Matches(
                     userCode,
                     @"(?m)^\s*//\s*gf2-input:\s*(.+)$",
                     RegexOptions.IgnoreCase))
        {
            var raw = match.Groups[1].Value.Trim();
            if (raw.Length == 0)
                continue;

            var named = Regex.Match(raw, @"^(\w+)\s*:\s*(.+)$");
            if (named.Success)
            {
                fields.Add(new SimulatedStdinField(
                    named.Groups[2].Value.Trim().Trim('"', '\''),
                    named.Groups[1].Value.Trim(),
                    null));
                continue;
            }

            if (raw.Contains(','))
            {
                fields.AddRange(raw.Split(',')
                    .Select(s => s.Trim().Trim('"', '\''))
                    .Where(s => s.Length > 0)
                    .Select(v => new SimulatedStdinField(v, null, null)));
            }
            else
            {
                fields.Add(new SimulatedStdinField(raw.Trim('"', '\''), null, null));
            }
        }

        return fields;
    }

    private static Dictionary<string, string> BuildPromptMap(string body)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? pendingPrompt = null;

        foreach (var line in body.Replace("\r\n", "\n").Split('\n'))
        {
            if (Regex.IsMatch(line, @"^\s*//"))
                continue;

            var write = Regex.Match(line, @"Console\.Write\s*\(\s*""([^""]*)""\s*\)");
            if (write.Success)
                pendingPrompt = write.Groups[1].Value.Trim();

            var writeLine = Regex.Match(line, @"Console\.WriteLine\s*\(\s*""([^""]*)""\s*\)\s*;?");
            if (writeLine.Success)
                pendingPrompt = writeLine.Groups[1].Value.Trim();

            if (!Regex.IsMatch(line, @"Console\.ReadLine\s*\("))
                continue;

            var varRead = Regex.Match(line, @"var\s+(\w+)\s*=\s*Console\.ReadLine");
            if (!varRead.Success)
            {
                pendingPrompt = null;
                continue;
            }

            var name = varRead.Groups[1].Value;
            if (pendingPrompt is not null && !map.ContainsKey(name))
                map[name] = pendingPrompt;

            pendingPrompt = null;
        }

        return map;
    }

    private static string EscapeCSharpString(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
}
