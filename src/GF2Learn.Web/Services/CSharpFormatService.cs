using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace GF2Learn.Web.Services;

public interface ICSharpFormatService
{
    Task<CSharpFormatResult> FormatAsync(string code, CancellationToken cancellationToken = default);
}

public sealed record CSharpFormatResult(bool Success, string? Formatted, string? Error);

public sealed class CSharpFormatService : ICSharpFormatService
{
    private const int MaxCodeLength = 64_000;

    private static readonly CSharpParseOptions ParseOptions =
        CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);

    private static readonly string FormatWorkDir = InitializeFormatWorkDir();
    private static readonly Lock WorkDirLock = new();

    public async Task<CSharpFormatResult> FormatAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return new CSharpFormatResult(false, null, "Ingen kode at formatere.");

        if (code.Length > MaxCodeLength)
            return new CSharpFormatResult(false, null, "Koden er for lang til formatering.");

        var syntaxError = await GetSyntaxErrorAsync(code, cancellationToken).ConfigureAwait(false);
        if (syntaxError is not null)
            return new CSharpFormatResult(false, null, syntaxError);

        try
        {
            EnsureEditorConfig();
            var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("GF2Format", LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithParseOptions(ParseOptions);

            string filePath;
            lock (WorkDirLock)
            {
                filePath = Path.Combine(FormatWorkDir, $"snippet_{Guid.NewGuid():N}.cs");
            }

            var document = project.AddDocument("Code.cs", code, filePath: filePath);
            var formatted = await Formatter.FormatAsync(document, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var text = await formatted.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return new CSharpFormatResult(true, NormalizeLineEndings(text.ToString()), null);
        }
        catch (Exception ex)
        {
            return new CSharpFormatResult(false, null, "Koden kunne ikke formateres: " + ex.Message);
        }
    }

    private static async Task<string?> GetSyntaxErrorAsync(string code, CancellationToken cancellationToken)
    {
        var tree = CSharpSyntaxTree.ParseText(code, ParseOptions, cancellationToken: cancellationToken);
        var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return "Koden kunne ikke læses.";

        var errors = root.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Take(3)
            .Select(d => d.GetMessage())
            .ToList();

        return errors.Count == 0
            ? null
            : "Ret syntaksfejl først: " + string.Join(" ", errors);
    }

    private static string InitializeFormatWorkDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gf2learn-csharp-format");
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void EnsureEditorConfig()
    {
        var source = Path.Combine(AppContext.BaseDirectory, "csharp-format.editorconfig");
        if (!File.Exists(source))
            return;

        lock (WorkDirLock)
        {
            File.Copy(source, Path.Combine(FormatWorkDir, ".editorconfig"), overwrite: true);
        }
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd() + "\n";
}
