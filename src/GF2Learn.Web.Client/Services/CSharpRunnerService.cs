using System.Diagnostics;
using System.Reflection;
using System.Text;
using GF2Learn.Web.Client.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace GF2Learn.Web.Client.Services;

public sealed class CSharpRunnerService(PlaygroundReferenceResolver referenceResolver)
{
    private const int DefaultTimeoutMs = 3000;

    public async Task<RunResult> RunAsync(
        string code,
        string? expectedOutput,
        IReadOnlyList<string> extraRefs,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new RunResult
            {
                Success = false,
                Error = "Ingen kode at køre."
            };
        }

        var sw = Stopwatch.StartNew();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(DefaultTimeoutMs);

        try
        {
            var coreRefs = await referenceResolver.GetCoreReferencesAsync(timeoutCts.Token);
            var extraMetadata = await referenceResolver.GetExtraReferencesAsync(extraRefs, timeoutCts.Token);

            if (coreRefs.IsDefaultOrEmpty)
            {
                return new RunResult
                {
                    Success = false,
                    Error = "Kunne ikke indlæse C#-runtime. Genindlæs siden og prøv igen.",
                    Elapsed = sw.Elapsed
                };
            }

            var references = coreRefs.AddRange(extraMetadata);
            var source = BuildEntryPointSource(code);
            var tree = CSharpSyntaxTree.ParseText(
                source,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12));

            var compilation = CSharpCompilation.Create(
                $"Playground_{Guid.NewGuid():N}",
                [tree],
                references,
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    concurrentBuild: false,
                    optimizationLevel: OptimizationLevel.Release));

            await using var peStream = new MemoryStream();
            var emitResult = compilation.Emit(peStream, cancellationToken: timeoutCts.Token);
            sw.Stop();

            if (!emitResult.Success)
            {
                return new RunResult
                {
                    Success = false,
                    Error = FormatDiagnostics(emitResult.Diagnostics),
                    Elapsed = sw.Elapsed
                };
            }

            peStream.Position = 0;
            var assembly = Assembly.Load(peStream.ToArray());
            var type = assembly.GetType("__PlaygroundEntry")
                ?? throw new InvalidOperationException("Kunne ikke finde entry point.");

            var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException("Kunne ikke finde Run-metoden.");

            var output = method.Invoke(null, null) as string ?? string.Empty;
            var normalizedOutput = NormalizeOutput(output);
            bool? expectedMatch = string.IsNullOrWhiteSpace(expectedOutput)
                ? null
                : normalizedOutput == NormalizeOutput(expectedOutput);

            return new RunResult
            {
                Success = true,
                Output = normalizedOutput,
                ExpectedMatch = expectedMatch,
                Elapsed = sw.Elapsed
            };
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            sw.Stop();
            return new RunResult
            {
                Success = false,
                Error = "Koden kørte for længe (timeout efter 3 sekunder).",
                Elapsed = sw.Elapsed
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new RunResult
            {
                Success = false,
                Error = ex.Message,
                Elapsed = sw.Elapsed
            };
        }
    }

    private static string BuildEntryPointSource(string userCode)
    {
        var indented = string.Join("\n", userCode.Split('\n').Select(l => "            " + l));
        return $$"""
            using System;
            using System.IO;

            public static class __PlaygroundEntry
            {
                public static string Run()
                {
                    var __writer = new StringWriter();
                    var __stdout = Console.Out;
                    Console.SetOut(__writer);
                    try
                    {
            {{indented}}
                    }
                    finally
                    {
                        Console.SetOut(__stdout);
                    }
                    return __writer.ToString();
                }
            }
            """;
    }

    private static string NormalizeOutput(string value) =>
        string.Join('\n', value.Replace("\r\n", "\n").Split('\n').Select(l => l.TrimEnd())).Trim();

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        var sb = new StringBuilder();
        foreach (var diagnostic in diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Error))
            sb.AppendLine(diagnostic.ToString());
        return sb.Length > 0 ? sb.ToString().Trim() : "Ukendt kompileringsfejl.";
    }
}
