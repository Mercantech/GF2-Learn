using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GF2Learn.Web.Auth;

public sealed record AppAuthStatus(bool IsLocal, bool IsConfigured);

public static class AppAuthentication
{
    private const string LoginPath = "/auth/login";
    internal const string LocalAccountPath = "/auth/local";
    internal static readonly TimeSpan MaximumCookieLifetime = TimeSpan.FromHours(8);

    public static bool UseLocalAccounts(IConfiguration configuration) =>
        configuration["Auth:Mode"]?.Trim().ToLowerInvariant() switch
        {
            "local" => true,
            null or "central" => false,
            _ => throw new InvalidOperationException("Auth__Mode skal være local eller central.")
        };

    internal static AuthenticationBuilder AddAppCookie(
        this IServiceCollection services,
        string scheme,
        string cookieName,
        Action<CookieAuthenticationOptions>? configure = null)
    {
        services.AddCascadingAuthenticationState();
        services.AddHttpContextAccessor();
        services.AddAuthorization();

        return services.AddAuthentication(scheme).AddCookie(scheme, options =>
        {
            options.Cookie.Name = cookieName;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.LoginPath = LoginPath;
            options.SlidingExpiration = false;
            options.ExpireTimeSpan = MaximumCookieLifetime;
            options.Events.OnRedirectToLogin = context => RedirectOrStatus(context, 401);
            options.Events.OnRedirectToAccessDenied = context => RedirectOrStatus(context, 403);
            configure?.Invoke(options);
        });
    }

    public static void MapAppLogout(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/auth/logout", async (HttpContext context, IAntiforgery antiforgery) =>
        {
            if (!await antiforgery.IsRequestValidAsync(context))
                return Results.BadRequest();

            await context.SignOutAsync();
            return Results.LocalRedirect($"{context.Request.PathBase}/login-fordele");
        });

    public static string LoginUrl(string? returnUrl) =>
        string.IsNullOrWhiteSpace(returnUrl) ? LoginPath : LoginPath + QueryString.Create("returnUrl", returnUrl);

    internal static string LocalAccountUrl(HttpRequest request, string returnUrl, bool register = false) =>
        $"{request.PathBase}{LocalAccountPath}" + QueryString.Create("returnUrl", returnUrl)
        + (register ? "&register=true" : "");

    public static string ResolveReturnUrl(HttpRequest request, string? returnUrl)
    {
        var fallback = $"{request.PathBase}/";
        if (string.IsNullOrWhiteSpace(returnUrl))
            return fallback;

        if (!returnUrl.StartsWith('/'))
        {
            if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var absolute)
                || !Uri.TryCreate($"{request.Scheme}://{request.Host}", UriKind.Absolute, out var origin)
                || absolute.GetLeftPart(UriPartial.Authority) != origin.GetLeftPart(UriPartial.Authority))
            {
                return fallback;
            }

            returnUrl = absolute.PathAndQuery + absolute.Fragment;
        }

        if (!returnUrl.StartsWith('/') || !RedirectHttpResult.IsLocalUrl(returnUrl))
            return fallback;

        var path = PathString.FromUriComponent(returnUrl.Split('?', '#')[0]);
        if (request.PathBase.HasValue && !path.StartsWithSegments(request.PathBase))
            return fallback;

        return returnUrl;
    }

    private static Task RedirectOrStatus(RedirectContext<CookieAuthenticationOptions> context, int status)
    {
        if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            context.Response.StatusCode = status;
        else
            context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    }
}
