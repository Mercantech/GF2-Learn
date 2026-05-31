using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GF2Learn.Web.Client.Services;

/// <summary>
/// Compile check for playground snippets using the same wrapper as WASM execution.
/// </summary>
public static class PlaygroundCompileChecker
{
    private static readonly Lazy<IReadOnlyList<MetadataReference>> References = new(BuildReferences);

    public static bool CanCompile(string userCode) =>
        GetCompileErrors(userCode).Count == 0;

    public static IReadOnlyList<string> GetCompileErrors(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
            return ["Empty code."];

        try
        {
            var source = PlaygroundSourceBuilder.BuildEntryPointSource(userCode);
            var tree = CSharpSyntaxTree.ParseText(
                source,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12));

            var compilation = CSharpCompilation.Create(
                "PlaygroundCheck",
                [tree],
                References.Value,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation.GetDiagnostics()
                .Where(d => d.Severity >= DiagnosticSeverity.Error)
                .Select(d => d.ToString())
                .ToList();
        }
        catch (Exception ex)
        {
            return [ex.Message];
        }
    }

    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        var trusted = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)?
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (trusted is { Length: > 0 })
        {
            return trusted
                .Select(static path => MetadataReference.CreateFromFile(path))
                .Distinct(MetadataReferenceComparer.Instance)
                .ToList();
        }

        var assemblies = new[]
        {
            typeof(object).Assembly,
            typeof(Console).Assembly,
            typeof(IEnumerable<>).Assembly,
            typeof(IList<>).Assembly,
            typeof(Dictionary<,>).Assembly,
            typeof(DateTime).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Task).Assembly,
            typeof(Uri).Assembly,
        };

        return assemblies
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Distinct(MetadataReferenceComparer.Instance)
            .ToList();
    }

    private sealed class MetadataReferenceComparer : IEqualityComparer<MetadataReference>
    {
        public static MetadataReferenceComparer Instance { get; } = new();

        public bool Equals(MetadataReference? x, MetadataReference? y) =>
            x?.Display == y?.Display;

        public int GetHashCode(MetadataReference obj) =>
            obj.Display?.GetHashCode(StringComparison.Ordinal) ?? 0;
    }
}
