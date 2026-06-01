namespace GF2Learn.Web.Services;

/// <summary>
/// Resolves scoped <see cref="IExerciseProgressService"/> safely from Blazor SSR components.
/// </summary>
public sealed class ExerciseProgressScope(
    IServiceScopeFactory scopeFactory,
    ILogger<ExerciseProgressScope> logger)
{
    public async Task<T> TryExecuteAsync<T>(
        Func<IExerciseProgressService, CancellationToken, Task<T>> action,
        T fallback,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetService<IExerciseProgressService>();
        if (service is null)
            return fallback;

        try
        {
            return await action(service, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exercise progress unavailable");
            return fallback;
        }
    }
}
