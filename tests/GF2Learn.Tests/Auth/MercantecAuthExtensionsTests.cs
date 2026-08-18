using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Models;
using GF2Learn.Web.Options;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace GF2Learn.Tests.Auth;

public sealed class MercantecAuthExtensionsTests
{
    private const string Issuer = "https://identity.example.test";
    private const string Audience = "gf2-learn-tests";

    [Fact]
    public void Cookie_authentication_is_absolute_and_non_sliding()
    {
        using var provider = CreateProvider();

        var options = provider
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(CookieAuthenticationDefaults.AuthenticationScheme);

        Assert.False(options.SlidingExpiration);
        Assert.Equal(TimeSpan.FromHours(8), options.ExpireTimeSpan);
    }

    [Fact]
    public async Task Short_access_token_bounds_the_cookie_expiration()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = new RsaSecurityKey(rsa) { KeyId = "short-token-key" };
        var validator = CreateValidator(signingKey);
        using var provider = CreateProvider(validator);
        var expires = DateTime.UtcNow.AddMinutes(5);
        var token = CreateToken(signingKey, expires);

        var ticket = await CreateTicketAsync(provider, token);

        Assert.False(ticket.Properties.IsPersistent);
        Assert.Equal(false, ticket.Properties.AllowRefresh);
        Assert.Equal(
            new DateTimeOffset(expires).ToUnixTimeSeconds(),
            ticket.Properties.ExpiresUtc?.ToUnixTimeSeconds());
    }

    [Fact]
    public async Task Long_access_token_gets_absolute_cap_and_user_sync_failure_does_not_fail_login()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = new RsaSecurityKey(rsa) { KeyId = "long-token-key" };
        var validator = CreateValidator(signingKey);
        var users = new ThrowingAppUserService();
        using var provider = CreateProvider(validator, users);
        var token = CreateToken(signingKey, DateTime.UtcNow.AddDays(1));
        var before = DateTimeOffset.UtcNow;

        var ticket = await CreateTicketAsync(provider, token);

        var after = DateTimeOffset.UtcNow;
        var principal = Assert.IsType<ClaimsPrincipal>(ticket.Principal);
        Assert.True(principal.Identity?.IsAuthenticated);
        Assert.Equal("student-1", principal.FindFirstValue("sub"));
        Assert.Equal(1, users.EnsureCalls);
        Assert.InRange(
            ticket.Properties.ExpiresUtc ?? DateTimeOffset.MinValue,
            before.AddHours(8).AddSeconds(-1),
            after.AddHours(8));
    }

    private static ServiceProvider CreateProvider(
        MercantecAccessTokenValidator? validator = null,
        IAppUserService? users = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MercantecAuth:ClientId"] = "test-client",
                ["MercantecAuth:ClientSecret"] = "test-secret"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMercantecAuth(configuration);

        if (validator is not null)
            services.AddSingleton(validator);
        if (users is not null)
            services.AddSingleton(users);

        return services.BuildServiceProvider();
    }

    private static MercantecAccessTokenValidator CreateValidator(SecurityKey signingKey)
    {
        var configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
        configuration.SigningKeys.Add(signingKey);

        return new MercantecAccessTokenValidator(
            Options.Create(new MercantecAuthOptions
            {
                Issuer = Issuer,
                Audience = Audience
            }),
            new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration));
    }

    private static string CreateToken(SecurityKey signingKey, DateTime expires)
    {
        var now = DateTime.UtcNow;
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim("sub", "student-1"),
                new Claim("role", "teacher")
            ]),
            NotBefore = now.AddMinutes(-1),
            IssuedAt = now.AddMinutes(-1),
            Expires = expires,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        });
    }

    private static async Task<OAuthCreatingTicketContext> CreateTicketAsync(
        IServiceProvider provider,
        string accessToken)
    {
        var options = provider
            .GetRequiredService<IOptionsMonitor<OAuthOptions>>()
            .Get(MercantecAuthOptions.SchemeName);
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var properties = new AuthenticationProperties();
        var scheme = new AuthenticationScheme(
            MercantecAuthOptions.SchemeName,
            displayName: null,
            typeof(OAuthHandler<OAuthOptions>));
        using var backchannel = new HttpClient();
        using var tokenResponse = OAuthTokenResponse.Success(JsonDocument.Parse(
            JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["access_token"] = accessToken,
                ["token_type"] = "Bearer"
            })));
        using var user = JsonDocument.Parse("{}");
        var ticket = new OAuthCreatingTicketContext(
            new ClaimsPrincipal(new ClaimsIdentity()),
            properties,
            httpContext,
            scheme,
            options,
            backchannel,
            tokenResponse,
            user.RootElement);

        await options.Events.CreatingTicket(ticket);
        return ticket;
    }

    private sealed class ThrowingAppUserService : IAppUserService
    {
        public int EnsureCalls { get; private set; }

        public Task<AppUser?> EnsureCurrentUserAsync(
            ClaimsPrincipal principal,
            bool markLogin = false,
            CancellationToken cancellationToken = default)
        {
            EnsureCalls++;
            throw new InvalidOperationException("Simulated database outage.");
        }

        public Task TouchActivityAsync(
            string userSub,
            DateTimeOffset occurredAt,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
