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
    private static readonly AdhocWorkspace Workspace = new();

    public async Task<CSharpFormatResult> FormatAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return new CSharpFormatResult(false, null, "Ingen kode at formatere.");

        if (code.Length > MaxCodeLength)
            return new CSharpFormatResult(false, null, "Koden er for lang til formatering.");

        try
        {
            var project = Workspace.AddProject("GF2Format", LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var document = project.AddDocument("Code.cs", code);
            var formatted = await Formatter.FormatAsync(document, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var text = await formatted.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return new CSharpFormatResult(true, text.ToString(), null);
        }
        catch (Exception ex)
        {
            return new CSharpFormatResult(false, null, "Koden kunne ikke formateres: " + ex.Message);
        }
    }
}
