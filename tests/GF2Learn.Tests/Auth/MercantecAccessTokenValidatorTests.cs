using System.Security.Claims;
using System.Security.Cryptography;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace GF2Learn.Tests.Auth;

public sealed class MercantecAccessTokenValidatorTests
{
    private const string Issuer = "https://identity.example.test";
    private const string Audience = "gf2-learn-tests";
    private const string AuthenticationScheme = "MercantecTest";

    [Fact]
    public async Task Valid_token_maps_only_the_application_identity_claims()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = CreateKey(rsa, "valid-key");
        var validator = CreateValidator(signingKey);
        var token = CreateToken(
            signingKey,
            subject: "student-42",
            name: "Ada Lovelace",
            email: "ada@example.test",
            roles: ["student", "teacher"]);

        var principal = await validator.ValidateAsync(token, AuthenticationScheme);

        var identity = Assert.IsType<ClaimsIdentity>(principal.Identity);
        Assert.True(identity.IsAuthenticated);
        Assert.Equal(AuthenticationScheme, identity.AuthenticationType);
        Assert.Equal("student-42", principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("student-42", principal.FindFirstValue("sub"));
        Assert.Equal("Ada Lovelace", principal.FindFirstValue(ClaimTypes.Name));
        Assert.Equal("ada@example.test", principal.FindFirstValue(ClaimTypes.Email));
        Assert.True(principal.IsInRole("student"));
        Assert.True(principal.IsInRole("teacher"));
    }

    [Fact]
    public async Task Token_with_wrong_audience_is_rejected()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = CreateKey(rsa, "valid-key");
        var validator = CreateValidator(signingKey);
        var token = CreateToken(signingKey, audience: "some-other-application");

        await Assert.ThrowsAsync<SecurityTokenInvalidAudienceException>(
            () => validator.ValidateAsync(token, AuthenticationScheme));
    }

    [Fact]
    public async Task Token_with_wrong_signature_is_rejected()
    {
        using var trustedRsa = RSA.Create(2048);
        using var untrustedRsa = RSA.Create(2048);
        var trustedKey = CreateKey(trustedRsa, "trusted-key");
        var untrustedKey = CreateKey(untrustedRsa, "untrusted-key");
        var validator = CreateValidator(trustedKey);
        var token = CreateToken(untrustedKey);

        await Assert.ThrowsAnyAsync<SecurityTokenException>(
            () => validator.ValidateAsync(token, AuthenticationScheme));
    }

    [Fact]
    public async Task Expired_token_is_rejected()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = CreateKey(rsa, "valid-key");
        var validator = CreateValidator(signingKey);
        var token = CreateToken(
            signingKey,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: DateTime.UtcNow.AddMinutes(-2));

        await Assert.ThrowsAsync<SecurityTokenExpiredException>(
            () => validator.ValidateAsync(token, AuthenticationScheme));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Token_without_a_non_empty_subject_is_rejected(string? subject)
    {
        using var rsa = RSA.Create(2048);
        var signingKey = CreateKey(rsa, "valid-key");
        var validator = CreateValidator(signingKey);
        var token = CreateToken(signingKey, subject: subject);

        var exception = await Assert.ThrowsAsync<SecurityTokenValidationException>(
            () => validator.ValidateAsync(token, AuthenticationScheme));

        Assert.Contains("sub", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Sign_in_validation_returns_the_validated_token_expiration()
    {
        using var rsa = RSA.Create(2048);
        var signingKey = CreateKey(rsa, "valid-key");
        var validator = CreateValidator(signingKey);
        var expires = DateTime.UtcNow.AddMinutes(10);
        var token = CreateToken(signingKey, expires: expires);

        var validation = await validator.ValidateForSignInAsync(token, AuthenticationScheme);

        Assert.Equal(
            new DateTimeOffset(expires).ToUnixTimeSeconds(),
            validation.ExpiresAt.ToUnixTimeSeconds());
        Assert.Equal("student-1", validation.Principal.FindFirstValue("sub"));
    }

    private static MercantecAccessTokenValidator CreateValidator(SecurityKey signingKey)
    {
        var configuration = new OpenIdConnectConfiguration
        {
            Issuer = Issuer
        };
        configuration.SigningKeys.Add(signingKey);

        var configurationManager =
            new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);

        return new MercantecAccessTokenValidator(
            Options.Create(new MercantecAuthOptions
            {
                Issuer = Issuer,
                Audience = Audience
            }),
            configurationManager);
    }

    private static RsaSecurityKey CreateKey(RSA rsa, string keyId) =>
        new(rsa) { KeyId = keyId };

    private static string CreateToken(
        SecurityKey signingKey,
        string issuer = Issuer,
        string audience = Audience,
        string? subject = "student-1",
        string name = "Test Student",
        string email = "student@example.test",
        string[]? roles = null,
        DateTime? notBefore = null,
        DateTime? expires = null)
    {
        roles ??= ["student"];

        var claims = new List<Claim>
        {
            new("name", name),
            new("email", email)
        };
        if (subject is not null)
            claims.Add(new Claim("sub", subject));
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var now = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = notBefore ?? now.AddMinutes(-1),
            IssuedAt = now.AddMinutes(-1),
            Expires = expires ?? now.AddMinutes(5),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
