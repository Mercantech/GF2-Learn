using GF2Learn.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

/// <summary>
/// Raw heartbeat sessions only exist to make cumulative updates idempotent. Daily
/// aggregates are retained, while raw deduplication state is kept for at most 24 hours.
/// </summary>
public sealed class ActivitySessionCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ActivitySessionCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<Gf2LearnDbContext>();
                // Cleanup runs every six hours, so an 18-hour cutoff keeps the
                // worst-case lifetime just under the documented 24-hour ceiling.
                var cutoff = DateTimeOffset.UtcNow.AddHours(-18);
                var removed = await db.PageActivitySessions
                    .Where(session => session.LastHeartbeatAt < cutoff)
                    .ExecuteDeleteAsync(stoppingToken);

                if (removed > 0)
                    logger.LogInformation("Fjernede {Count} udløbne aktivitetssessioner.", removed);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Kunne ikke rydde gamle aktivitetssessioner.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
