namespace GF2Learn.Web.Services;

/// <summary>
/// Resolves scoped <see cref="IKnowledgeCheckProgressService"/> safely from Blazor SSR components.
/// </summary>
public sealed class KnowledgeCheckProgressScope(
    IServiceScopeFactory scopeFactory,
    ILogger<KnowledgeCheckProgressScope> logger)
{
    public async Task<T> TryExecuteAsync<T>(
        Func<IKnowledgeCheckProgressService, CancellationToken, Task<T>> action,
        T fallback,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetService<IKnowledgeCheckProgressService>();
        if (service is null)
            return fallback;

        try
        {
            return await action(service, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Knowledge check progress unavailable");
            return fallback;
        }
    }
}
