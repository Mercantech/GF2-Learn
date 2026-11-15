using System.Security.Claims;
using GF2Learn.Web.Options;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace GF2Learn.Web.Auth;

public static class MercantecAuthExtensions
{
    public static IServiceCollection AddMercantecAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MercantecAuthOptions>(configuration.GetSection(MercantecAuthOptions.SectionName));

        var authOptions = configuration.GetSection(MercantecAuthOptions.SectionName).Get<MercantecAuthOptions>()
            ?? new MercantecAuthOptions();

        services.AddSingleton<MercantecAccessTokenValidator>();

        var authBuilder = services.AddAppCookie(CookieAuthenticationDefaults.AuthenticationScheme, authOptions.CookieName);

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

                        var cookieIssuedAt = DateTimeOffset.UtcNow;
                        var cookieExpiresAt = ConfigureAbsoluteCookieLifetime(
                            context.Properties,
                            validation.ExpiresAt,
                            cookieIssuedAt);
                        AddSessionMetadataClaims(
                            context.Principal,
                            validation.IssuedAt,
                            validation.ExpiresAt,
                            cookieIssuedAt,
                            cookieExpiresAt);

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

        services.AddSingleton(new AppAuthStatus(IsLocal: false, IsConfigured: IsAuthConfigured(authOptions)));
        return services;
    }

    public static IEndpointRouteBuilder MapMercantecAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/auth/login", async (HttpContext context, string? returnUrl, AppAuthStatus status) =>
        {
            if (!status.IsConfigured)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(
                    "Mercantec Auth er ikke konfigureret. Sæt MercantecAuth__ClientSecret.");
                return;
            }

            await context.ChallengeAsync(MercantecAuthOptions.SchemeName, new AuthenticationProperties
            {
                RedirectUri = AppAuthentication.ResolveReturnUrl(context.Request, returnUrl)
            });
        });

        return endpoints;
    }

    private static bool IsAuthConfigured(MercantecAuthOptions options) =>
        !string.IsNullOrWhiteSpace(options.ClientId)
        && !string.IsNullOrWhiteSpace(options.ClientSecret);

    private static DateTimeOffset ConfigureAbsoluteCookieLifetime(
        AuthenticationProperties properties,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset issuedAt)
    {
        var maximumExpiry = issuedAt.Add(AppAuthentication.MaximumCookieLifetime);

        properties.IsPersistent = false;
        properties.AllowRefresh = false;
        properties.IssuedUtc = issuedAt;
        properties.ExpiresUtc = accessTokenExpiresAt < maximumExpiry
            ? accessTokenExpiresAt
            : maximumExpiry;

        return properties.ExpiresUtc.Value;
    }

    private static void AddSessionMetadataClaims(
        ClaimsPrincipal principal,
        DateTimeOffset accessTokenIssuedAt,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset cookieIssuedAt,
        DateTimeOffset cookieExpiresAt)
    {
        if (principal.Identity is not ClaimsIdentity identity)
            return;

        identity.AddClaims(
        [
            UnixTimeClaim(AuthSessionMetadataClaims.AccessTokenIssuedAt, accessTokenIssuedAt),
            UnixTimeClaim(AuthSessionMetadataClaims.AccessTokenExpiresAt, accessTokenExpiresAt),
            UnixTimeClaim(AuthSessionMetadataClaims.CookieIssuedAt, cookieIssuedAt),
            UnixTimeClaim(AuthSessionMetadataClaims.CookieExpiresAt, cookieExpiresAt),
        ]);
    }

    private static Claim UnixTimeClaim(string type, DateTimeOffset value) =>
        new(type, value.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);

}
