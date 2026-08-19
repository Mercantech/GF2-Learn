namespace GF2Learn.Web.Auth;

/// <summary>
/// Ufølsomme tidsstempler, som bruges til auth-diagnostik i browserkonsollen.
/// Tokenværdier må aldrig kopieres til claims eller browseren.
/// </summary>
public static class AuthSessionMetadataClaims
{
    public const string AccessTokenIssuedAt = "gf2:auth:access-issued-at";
    public const string AccessTokenExpiresAt = "gf2:auth:access-expires-at";
    public const string CookieIssuedAt = "gf2:auth:cookie-issued-at";
    public const string CookieExpiresAt = "gf2:auth:cookie-expires-at";
}
