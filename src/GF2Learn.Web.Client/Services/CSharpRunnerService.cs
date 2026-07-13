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

    public async Task<RunResult> RunInteractiveConsoleAsync(
        string code,
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
            if (coreRefs.IsDefaultOrEmpty)
            {
                return new RunResult
                {
                    Success = false,
                    Error = "Kunne ikke indlæse C#-runtime. Genindlæs siden og prøv igen.",
                    Elapsed = sw.Elapsed
                };
            }

            var prepared = PlaygroundSourceBuilder.Prepare(code);
            PlaygroundStdinBridge.Reset();
            PlaygroundStdoutBridge.Reset();

            var entrySource = PlaygroundSourceBuilder.BuildInteractiveEntryPointSource(prepared);
            var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);
            var tree = CSharpSyntaxTree.ParseText(entrySource, parseOptions, path: "Program.cs");

            var compilation = CSharpCompilation.Create(
                $"PlaygroundConsole_{Guid.NewGuid():N}",
                [tree],
                coreRefs,
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
            WireStdinHook(assembly);
            WireStdoutHook(assembly);

            var type = assembly.GetType("__PlaygroundEntry")
                ?? throw new InvalidOperationException("Kunne ikke finde entry point.");

            var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException("Kunne ikke finde Run-metoden.");

            using var runCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            runCts.CancelAfter(TimeSpan.FromMinutes(2));
            var output = await Task.Run(() => InvokeRun(method), runCts.Token);
            sw.Stop();

            return new RunResult
            {
                Success = true,
                Output = NormalizeOutput(output),
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
                    ? "Kompilering tog for lang tid."
                    : "Koden kørte for længe.",
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

    public async Task<RunResult> RunProjectAsync(
        IReadOnlyList<ProjectSourceFile> files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0)
        {
            return new RunResult
            {
                Success = false,
                Error = "Projektet har ingen filer."
            };
        }

        var entry = files.FirstOrDefault(f =>
            f.FileName.Equals("Program.cs", StringComparison.OrdinalIgnoreCase))
            ?? files[0];

        var sw = Stopwatch.StartNew();
        using var compileCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        compileCts.CancelAfter(CompileTimeoutMs);

        try
        {
            var coreRefs = await referenceResolver.GetCoreReferencesAsync(compileCts.Token);
            if (coreRefs.IsDefaultOrEmpty)
            {
                return new RunResult
                {
                    Success = false,
                    Error = "Kunne ikke indlæse C#-runtime. Genindlæs siden og prøv igen.",
                    Elapsed = sw.Elapsed
                };
            }

            var prepared = PlaygroundSourceBuilder.Prepare(entry.Content);
            PlaygroundStdinBridge.Reset();
            foreach (var line in prepared.StdinLines)
                PlaygroundStdinBridge.Enqueue(line);

            var entrySource = PlaygroundSourceBuilder.BuildInteractiveEntryPointSource(prepared);
            var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);
            var trees = new List<SyntaxTree>
            {
                CSharpSyntaxTree.ParseText(entrySource, parseOptions, path: entry.FileName)
            };

            foreach (var file in files.Where(f => !ReferenceEquals(f, entry)))
            {
                if (string.IsNullOrWhiteSpace(file.Content))
                    continue;

                trees.Add(CSharpSyntaxTree.ParseText(file.Content, parseOptions, path: file.FileName));
            }

            var compilation = CSharpCompilation.Create(
                $"PlaygroundProject_{Guid.NewGuid():N}",
                trees,
                coreRefs,
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
            WireStdinHook(assembly);

            var type = assembly.GetType("__PlaygroundEntry")
                ?? throw new InvalidOperationException("Kunne ikke finde entry point.");

            var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException("Kunne ikke finde Run-metoden.");

            using var runCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            runCts.CancelAfter(TimeSpan.FromMinutes(2));
            var output = await Task.Run(() => InvokeRun(method), runCts.Token);
            sw.Stop();

            return new RunResult
            {
                Success = true,
                Output = NormalizeOutput(output),
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
                    ? "Kompilering tog for lang tid."
                    : "Koden kørte for længe.",
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

    private static void WireStdinHook(Assembly assembly)
    {
        var hook = assembly.GetType("__StdinHook");
        if (hook is null)
            return;

        var field = hook.GetField("Provider", BindingFlags.Public | BindingFlags.Static);
        field?.SetValue(null, (Func<string?>)PlaygroundStdinBridge.ReadLineSync);
    }

    private static void WireStdoutHook(Assembly assembly)
    {
        var hook = assembly.GetType("__StdoutHook");
        if (hook is null)
            return;

        hook.GetField("OnWrite", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, (Action<string>)PlaygroundStdoutBridge.RaiseWrite);
        hook.GetField("OnWriteLine", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, (Action<string>)PlaygroundStdoutBridge.RaiseWriteLine);
    }

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
