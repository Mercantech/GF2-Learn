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
    /// <summary>First WASM compile loads BCL metadata over the network — needs more than a few seconds.</summary>
    private const int CompileTimeoutMs = 30_000;
    private const int RunTimeoutMs = 5_000;

    public Task<RunResult> RunAsync(
        string code,
        string? expectedOutput,
        IReadOnlyList<string> extraRefs,
        CancellationToken cancellationToken = default) =>
        RunAsync(code, expectedOutput, extraRefs, stdinOverride: null, showStdinTraceInOutput: true, wrapExerciseMethod: false, cancellationToken);

    public Task<RunResult> RunAsync(
        string code,
        string? expectedOutput,
        IReadOnlyList<string> extraRefs,
        IReadOnlyList<string>? stdinOverride,
        bool showStdinTraceInOutput,
        CancellationToken cancellationToken = default) =>
        RunAsync(code, expectedOutput, extraRefs, stdinOverride, showStdinTraceInOutput, wrapExerciseMethod: false, cancellationToken);

    public async Task<RunResult> RunAsync(
        string code,
        string? expectedOutput,
        IReadOnlyList<string> extraRefs,
        IReadOnlyList<string>? stdinOverride,
        bool showStdinTraceInOutput,
        bool wrapExerciseMethod,
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
        using var compileCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        compileCts.CancelAfter(CompileTimeoutMs);

        try
        {
            var coreRefs = await referenceResolver.GetCoreReferencesAsync(compileCts.Token);
            var extraMetadata = await referenceResolver.GetExtraReferencesAsync(extraRefs, compileCts.Token);

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
            var prepared = PlaygroundSourceBuilder.Prepare(code);
            var stdin = stdinOverride ?? prepared.StdinLines;
            var source = wrapExerciseMethod
                ? PlaygroundSourceBuilder.BuildExerciseMethodEntryPointSource(code, showStdinTraceInOutput)
                : PlaygroundSourceBuilder.BuildEntryPointSource(
                    prepared.ExecutableCode,
                    stdin,
                    prepared.UsesReadLine,
                    showStdinTraceInOutput);
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
            var emitResult = compilation.Emit(peStream, cancellationToken: compileCts.Token);

            if (!emitResult.Success)
            {
                sw.Stop();
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

            using var runCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            runCts.CancelAfter(RunTimeoutMs);
            var output = await Task.Run(() => InvokeRun(method), runCts.Token);
            sw.Stop();
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
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            sw.Stop();
            return new RunResult
            {
                Success = false,
                Error = compileCts.IsCancellationRequested
                    ? "Kompilering tog for lang tid (timeout efter 30 sekunder). Prøv igen — første kørsel kan være langsom."
                    : "Koden kørte for længe (timeout efter 5 sekunder).",
                Elapsed = sw.Elapsed
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new RunResult
            {
                Success = false,
                Error = FormatExceptionMessage(ex),
                Elapsed = sw.Elapsed
            };
        }
    }

    private static string InvokeRun(MethodInfo method)
    {
        try
        {
            return method.Invoke(null, null) as string ?? string.Empty;
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    private static string FormatExceptionMessage(Exception ex)
    {
        var current = ex;
        while (current is TargetInvocationException { InnerException: not null } tie)
            current = tie.InnerException;
        while (current is AggregateException { InnerException: not null } agg)
            current = agg.InnerException;

        return string.IsNullOrWhiteSpace(current?.Message)
            ? current?.GetType().Name ?? "Ukendt fejl"
            : current.Message;
    }

    private static string NormalizeOutput(string value) =>
        string.Join('\n', value.Replace("\r\n", "\n").Split('\n').Select(l => l.TrimEnd())).Trim();

    public Task WarmupAsync(CancellationToken cancellationToken = default) =>
        referenceResolver.GetCoreReferencesAsync(cancellationToken);

    private static string FormatDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        var sb = new StringBuilder();
        foreach (var diagnostic in diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Error))
            sb.AppendLine(diagnostic.ToString());
        return sb.Length > 0 ? sb.ToString().Trim() : "Ukendt kompileringsfejl.";
    }
}
