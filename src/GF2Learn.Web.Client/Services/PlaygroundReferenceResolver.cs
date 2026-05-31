using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace GF2Learn.Web.Client.Services;

public sealed class PlaygroundReferenceResolver(IHttpClientFactory httpClientFactory)
{
    private static readonly string[] DefaultImports =
    [
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Text",
        "System.Threading.Tasks"
    ];

    /// <summary>
    /// Portable DLLs under wwwroot/playground/bcl (metadata for Roslyn only).
    /// WASM assemblies have no on-disk location, so we cannot use host file paths.
    /// </summary>
    private static readonly string[] CoreBclAssemblies =
    [
        "System.Private.CoreLib",
        "System.Runtime",
        "System.Console",
        "System.Collections",
        "System.Linq",
        "System.Runtime.Extensions",
        "System.Text.Json",
        "Microsoft.CSharp",
        "System.IO",
        "System.Threading"
    ];

    private ImmutableArray<MetadataReference>? _cachedCore;
    private Dictionary<string, string>? _whitelist;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public IReadOnlyList<string> DefaultNamespaces => DefaultImports;

    public async Task<ImmutableArray<MetadataReference>> GetCoreReferencesAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cachedCore is { } cached && !cached.IsDefaultOrEmpty)
            return cached;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cachedCore is { } hit && !hit.IsDefaultOrEmpty)
                return hit;

            _cachedCore = await LoadBclReferencesAsync(cancellationToken);
            return _cachedCore.Value;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<ImmutableArray<MetadataReference>> LoadBclReferencesAsync(
        CancellationToken cancellationToken)
    {
        var http = httpClientFactory.CreateClient("PlaygroundRefs");
        var builder = ImmutableArray.CreateBuilder<MetadataReference>();

        foreach (var name in CoreBclAssemblies)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var bytes = await http.GetByteArrayAsync($"playground/bcl/{name}.dll", cancellationToken);
                builder.Add(MetadataReference.CreateFromImage(bytes));
            }
            catch
            {
                // Missing or unreachable BCL assembly — continue with others
            }
        }

        return builder.ToImmutable();
    }

    public async Task<IReadOnlyList<MetadataReference>> GetExtraReferencesAsync(
        IReadOnlyList<string> extraRefs,
        CancellationToken cancellationToken = default)
    {
        if (extraRefs.Count == 0)
            return [];

        await _gate.WaitAsync(cancellationToken);
        try
        {
            _whitelist ??= await LoadWhitelistMapAsync(cancellationToken);
            if (_whitelist.Count == 0)
                return [];

            var http = httpClientFactory.CreateClient("PlaygroundRefs");
            var list = new List<MetadataReference>();

            foreach (var name in extraRefs)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!_whitelist.TryGetValue(name, out var path))
                    continue;

                try
                {
                    var bytes = await http.GetByteArrayAsync(path, cancellationToken);
                    list.Add(MetadataReference.CreateFromImage(bytes));
                }
                catch
                {
                    // Skip unavailable whitelist assemblies
                }
            }

            return list;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<Dictionary<string, string>> LoadWhitelistMapAsync(CancellationToken cancellationToken)
    {
        try
        {
            var http = httpClientFactory.CreateClient("PlaygroundRefs");
            await using var stream = await http.GetStreamAsync("playground/playground.json", cancellationToken);
            var doc = await JsonSerializer.DeserializeAsync<PlaygroundManifest>(stream, cancellationToken: cancellationToken);
            return doc?.Packages ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private sealed class PlaygroundManifest
    {
        public Dictionary<string, string> Packages { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
