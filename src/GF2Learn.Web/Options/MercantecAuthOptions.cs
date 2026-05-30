namespace GF2Learn.Web.Options;

public sealed class MercantecAuthOptions
{
    public const string SectionName = "MercantecAuth";
    public const string SchemeName = "Mercantec";

    public string ClientId { get; set; } = "gf2-learn";
    public string ClientSecret { get; set; } = "";
    public string Issuer { get; set; } = "https://auth.mercantec.tech";
    public string Audience { get; set; } = "mercantec-apps";
    public string AuthorizationEndpoint { get; set; } = "https://auth.mercantec.tech/oauth/authorize";
    public string TokenEndpoint { get; set; } = "https://auth.mercantec.tech/oauth/token";
    public string SignOutEndpoint { get; set; } = "https://auth.mercantec.tech/signout";
    public string CallbackPath { get; set; } = "/signin-mercantec";
    public string CookieName { get; set; } = ".gf2learn.auth";
}
