using System.Security.Claims;
using GF2Learn.Web.Options;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;

namespace GF2Learn.Web.Auth;

public static class MercantecAuthExtensions
{
    private static readonly TimeSpan MaximumCookieLifetime = TimeSpan.FromHours(8);

    public static IServiceCollection AddMercantecAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MercantecAuthOptions>(configuration.GetSection(MercantecAuthOptions.SectionName));

        var authOptions = configuration.GetSection(MercantecAuthOptions.SectionName).Get<MercantecAuthOptions>()
            ?? new MercantecAuthOptions();

        services.AddCascadingAuthenticationState();
        services.AddHttpContextAccessor();
        services.AddSingleton<MercantecAccessTokenValidator>();

        var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // Cookie (ikke OAuth) ved manglende auth — API får 401 via OnRedirectToLogin.
                // /auth/login kalder ChallengeAsync(Mercantec) eksplicit.
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = authOptions.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = "/auth/login";
                options.SlidingExpiration = false;
                options.ExpireTimeSpan = MaximumCookieLifetime;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (IsApiRequest(context.Request))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (IsApiRequest(context.Request))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

        if (IsAuthConfigured(authOptions))
        {
            authBuilder.AddOAuth(MercantecAuthOptions.SchemeName, options =>
            {
                options.ClientId = authOptions.ClientId;
                options.ClientSecret = authOptions.ClientSecret;
                options.AuthorizationEndpoint = authOptions.AuthorizationEndpoint;
                options.TokenEndpoint = authOptions.TokenEndpoint;
                options.CallbackPath = authOptions.CallbackPath;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        if (string.IsNullOrEmpty(context.AccessToken))
                            return;

                        var validator = context.HttpContext.RequestServices
                            .GetRequiredService<MercantecAccessTokenValidator>();
                        var validation = await validator.ValidateForSignInAsync(
                            context.AccessToken,
                            context.Scheme.Name,
                            context.HttpContext.RequestAborted);
                        context.Principal = validation.Principal;

                        ConfigureAbsoluteCookieLifetime(
                            context.Properties,
                            validation.ExpiresAt,
                            DateTimeOffset.UtcNow);

                        try
                        {
                            var users = context.HttpContext.RequestServices.GetService<IAppUserService>();
                            if (users is not null)
                            {
                                await users.EnsureCurrentUserAsync(
                                    context.Principal,
                                    markLogin: true,
                                    cancellationToken: context.HttpContext.RequestAborted);
                            }
                        }
                        catch (OperationCanceledException)
                            when (context.HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            throw;
                        }
                        catch (Exception exception)
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("GF2Learn.Web.Auth.MercantecAuth");
                            logger.LogWarning(
                                exception,
                                "Could not synchronize the authenticated user. Sign-in will continue.");
                        }
                    },
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        services.AddSingleton(new MercantecAuthStatus(IsAuthConfigured(authOptions)));
        services.AddAuthorization();
        return services;
    }

    public static IEndpointRouteBuilder MapMercantecAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/auth/login", async (HttpContext context, string? returnUrl, MercantecAuthStatus status) =>
        {
            if (!status.IsConfigured)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(
                    "Mercantec Auth er ikke konfigureret. Sæt MercantecAuth__ClientSecret.");
                return;
            }

            returnUrl ??= context.Request.Query["ReturnUrl"].FirstOrDefault();
            var redirectUri = ResolvePostLoginRedirect(context, returnUrl);

            await context.ChallengeAsync(MercantecAuthOptions.SchemeName, new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        });

        endpoints.MapGet("/auth/logout", async (
            HttpContext context,
            IOptions<MercantecAuthOptions> authOptions,
            MercantecAuthStatus status) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!status.IsConfigured)
                return Results.Redirect("/");

            var request = context.Request;
            var appReturnUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/";
            var signOutUrl =
                $"{authOptions.Value.SignOutEndpoint}?returnUrl={Uri.EscapeDataString(appReturnUrl)}";

            return Results.Redirect(signOutUrl);
        });

        return endpoints;
    }

    private static bool IsAuthConfigured(MercantecAuthOptions options) =>
        !string.IsNullOrWhiteSpace(options.ClientId)
        && !string.IsNullOrWhiteSpace(options.ClientSecret);

    private static void ConfigureAbsoluteCookieLifetime(
        AuthenticationProperties properties,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset issuedAt)
    {
        var maximumExpiry = issuedAt.Add(MaximumCookieLifetime);

        properties.IsPersistent = false;
        properties.AllowRefresh = false;
        properties.IssuedUtc = issuedAt;
        properties.ExpiresUtc = accessTokenExpiresAt < maximumExpiry
            ? accessTokenExpiresAt
            : maximumExpiry;
    }

    /// <summary>Builds <c>/auth/login?returnUrl=…</c> for the current or given page.</summary>
    public static string LoginUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return "/auth/login";

        return $"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}";
    }

    private static string ResolvePostLoginRedirect(HttpContext context, string? returnUrl)
    {
        var fallback = $"{context.Request.PathBase}/";
        if (string.IsNullOrWhiteSpace(returnUrl))
            return fallback;

        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absolute))
        {
            var path = absolute.PathAndQuery + absolute.Fragment;
            return string.IsNullOrWhiteSpace(path) ? fallback : path;
        }

        if (!returnUrl.StartsWith('/'))
            return fallback;

        if (returnUrl.StartsWith("//", StringComparison.Ordinal))
            return fallback;

        return returnUrl;
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        var path = (request.PathBase.Value ?? string.Empty) + (request.Path.Value ?? string.Empty);
        return path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/api", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed record MercantecAuthStatus(bool IsConfigured);
