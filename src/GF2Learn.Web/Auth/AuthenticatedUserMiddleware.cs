using GF2Learn.Web.Services;

namespace GF2Learn.Web.Auth;

public sealed class AuthenticatedUserMiddleware(
    RequestDelegate next,
    ILogger<AuthenticatedUserMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAppUserService users)
    {
        if (context.User.Identity?.IsAuthenticated == true && ShouldSynchronize(context.Request.Path))
        {
            try
            {
                await users.EnsureCurrentUserAsync(context.User, cancellationToken: context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected.
            }
            catch (Exception ex)
            {
                // A database outage must not turn a valid login into a site-wide outage.
                logger.LogWarning(ex, "Kunne ikke synkronisere den loggede GF2 Learn-bruger.");
            }
        }

        await next(context);
    }

    private static bool ShouldSynchronize(PathString path)
    {
        var value = path.Value ?? string.Empty;
        if (value.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !Path.HasExtension(value);
    }
}
