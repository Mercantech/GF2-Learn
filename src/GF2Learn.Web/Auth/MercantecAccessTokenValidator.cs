using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using GF2Learn.Web.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace GF2Learn.Web.Auth;

/// <summary>
/// Validates Mercantec access tokens against the provider's cached OpenID Connect
/// configuration before creating the application's cookie principal.
/// </summary>
public sealed class MercantecAccessTokenValidator
{
    private static readonly TimeSpan AllowedClockSkew = TimeSpan.FromMinutes(1);
    private static readonly string[] AllowedAlgorithms = [SecurityAlgorithms.RsaSha256];

    private readonly MercantecAuthOptions _options;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly JsonWebTokenHandler _tokenHandler = new()
    {
        MapInboundClaims = false
    };

    /// <summary>
    /// Creates a singleton-friendly validator. Supplying a configuration manager is
    /// optional so tests can use <see cref="StaticConfigurationManager{T}"/> without
    /// performing network requests.
    /// </summary>
    public MercantecAccessTokenValidator(
        IOptions<MercantecAuthOptions> options,
        IConfigurationManager<OpenIdConnectConfiguration>? configurationManager = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        ValidateOptions(_options);
        _configurationManager = configurationManager ?? CreateConfigurationManager(_options.Issuer);
    }

    /// <summary>
    /// Validates an access token and maps only the identity claims consumed by the app.
    /// Invalid tokens result in a <see cref="SecurityTokenException"/>.
    /// </summary>
    public async Task<ClaimsPrincipal> ValidateAsync(
        string accessToken,
        string authenticationScheme,
        CancellationToken cancellationToken = default) =>
        (await ValidateForSignInAsync(accessToken, authenticationScheme, cancellationToken)
            .ConfigureAwait(false)).Principal;

    /// <summary>
    /// Validates an access token and returns both the application principal and the
    /// authoritative token expiration used to bound the authentication cookie.
    /// </summary>
    public async Task<MercantecAccessTokenValidation> ValidateForSignInAsync(
        string accessToken,
        string authenticationScheme,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationScheme);

        var configuration = await _configurationManager
            .GetConfigurationAsync(cancellationToken)
            .ConfigureAwait(false);

        EnsureExpectedDiscoveryIssuer(configuration);

        var validationResult = await ValidateTokenAsync(accessToken, configuration, cancellationToken)
            .ConfigureAwait(false);

        // A rotated signing key can appear before the regular refresh interval. Ask the
        // OIDC manager for fresh discovery/JWKS data and retry validation once.
        if (!validationResult.IsValid
            && validationResult.Exception is SecurityTokenSignatureKeyNotFoundException)
        {
            _configurationManager.RequestRefresh();
            configuration = await _configurationManager
                .GetConfigurationAsync(cancellationToken)
                .ConfigureAwait(false);

            EnsureExpectedDiscoveryIssuer(configuration);
            validationResult = await ValidateTokenAsync(accessToken, configuration, cancellationToken)
                .ConfigureAwait(false);
        }

        if (!validationResult.IsValid)
            ThrowValidationException(validationResult);

        if (validationResult.SecurityToken is not JsonWebToken validatedToken)
            throw new SecurityTokenValidationException("Mercantec access token validation returned an unexpected token type.");

        var principal = CreatePrincipal(validatedToken, authenticationScheme);
        var issuedAt = new DateTimeOffset(
            DateTime.SpecifyKind(validatedToken.IssuedAt, DateTimeKind.Utc));
        var expiresAt = new DateTimeOffset(
            DateTime.SpecifyKind(validatedToken.ValidTo, DateTimeKind.Utc));

        return new MercantecAccessTokenValidation(principal, issuedAt, expiresAt);
    }

    private async Task<TokenValidationResult> ValidateTokenAsync(
        string accessToken,
        OpenIdConnectConfiguration configuration,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationParameters = new TokenValidationParameters
        {
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = configuration.SigningKeys,
            ValidAlgorithms = AllowedAlgorithms,

            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,

            ValidateAudience = true,
            ValidAudience = _options.Audience,

            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = AllowedClockSkew,

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        return await _tokenHandler
            .ValidateTokenAsync(accessToken, validationParameters)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private void EnsureExpectedDiscoveryIssuer(OpenIdConnectConfiguration configuration)
    {
        if (!string.Equals(configuration.Issuer, _options.Issuer, StringComparison.Ordinal))
        {
            throw new SecurityTokenInvalidIssuerException(
                $"Mercantec discovery issuer '{configuration.Issuer}' does not match the configured issuer.");
        }
    }

    private static ClaimsPrincipal CreatePrincipal(JsonWebToken token, string authenticationScheme)
    {
        if (string.IsNullOrWhiteSpace(token.Subject))
            throw new SecurityTokenValidationException("Mercantec access token is missing the required sub claim.");

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, token.Subject));
        claims.Add(new Claim("sub", token.Subject));

        AddClaimIfPresent(claims, token, "name", ClaimTypes.Name);
        AddClaimIfPresent(claims, token, "email", ClaimTypes.Email);
        AddClaimIfPresent(claims, token, "login_method", "login_method");

        foreach (var role in token.Claims.Where(claim => claim.Type == "role"))
        {
            if (!string.IsNullOrWhiteSpace(role.Value))
                claims.Add(new Claim(ClaimTypes.Role, role.Value));
        }

        var identity = new ClaimsIdentity(
            claims,
            authenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);

        return new ClaimsPrincipal(identity);
    }

    private static void AddClaimIfPresent(
        ICollection<Claim> claims,
        JsonWebToken token,
        string tokenClaimType,
        string applicationClaimType)
    {
        if (token.TryGetClaim(tokenClaimType, out var claim)
            && !string.IsNullOrWhiteSpace(claim.Value))
        {
            claims.Add(new Claim(applicationClaimType, claim.Value));
        }
    }

    private static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(string issuer)
    {
        var metadataAddress = $"{issuer.TrimEnd('/')}/.well-known/openid-configuration";
        var documentRetriever = new HttpDocumentRetriever
        {
            RequireHttps = true
        };

        return new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            documentRetriever);
    }

    private static void ValidateOptions(MercantecAuthOptions options)
    {
        if (!Uri.TryCreate(options.Issuer, UriKind.Absolute, out var issuer)
            || !string.Equals(issuer.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrEmpty(issuer.Query)
            || !string.IsNullOrEmpty(issuer.Fragment))
        {
            throw new OptionsValidationException(
                MercantecAuthOptions.SectionName,
                typeof(MercantecAuthOptions),
                ["Issuer must be an absolute HTTPS URL without query string or fragment."]);
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new OptionsValidationException(
                MercantecAuthOptions.SectionName,
                typeof(MercantecAuthOptions),
                ["Audience must be configured."]);
        }
    }

    [DoesNotReturn]
    private static void ThrowValidationException(TokenValidationResult validationResult)
    {
        if (validationResult.Exception is not null)
            ExceptionDispatchInfo.Capture(validationResult.Exception).Throw();

        throw new SecurityTokenValidationException("Mercantec access token validation failed.");
    }
}

public sealed record MercantecAccessTokenValidation(
    ClaimsPrincipal Principal,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt);
