using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GF2Learn.Web.Auth;

public static class MercantecClaimMapper
{
    public static ClaimsPrincipal CreatePrincipal(string accessToken, string authenticationScheme)
    {
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(accessToken);

        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(jwt.Subject))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, jwt.Subject));
            claims.Add(new Claim("sub", jwt.Subject));
        }

        AddClaimIfPresent(claims, jwt, "name", ClaimTypes.Name);
        AddClaimIfPresent(claims, jwt, "email", ClaimTypes.Email);
        AddClaimIfPresent(claims, jwt, "login_method", "login_method");

        foreach (var role in jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value))
        {
            if (!string.IsNullOrWhiteSpace(role))
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme));
    }

    private static void AddClaimIfPresent(List<Claim> claims, JsonWebToken jwt, string jwtType, string claimType)
    {
        if (jwt.TryGetClaim(jwtType, out var claim) && !string.IsNullOrWhiteSpace(claim.Value))
            claims.Add(new Claim(claimType, claim.Value));
    }
}
