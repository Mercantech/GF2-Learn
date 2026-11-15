using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GF2Learn.Web.Auth;

public sealed class LocalAccount : IdentityUser
{
    public const string SubjectPrefix = "local:";
    public const int MinimumPasswordLength = 12;
    public const int MaximumPasswordLength = 128;

    public LocalAccount() => Id = $"{SubjectPrefix}{Guid.NewGuid():D}";

    public required string DisplayName { get; set; }

    internal static bool IsLocalSubject(string subject) =>
        subject.StartsWith(SubjectPrefix, StringComparison.OrdinalIgnoreCase);
}

internal sealed class LocalAccountClaimsFactory(
    UserManager<LocalAccount> userManager,
    IOptions<IdentityOptions> identityOptions)
    : UserClaimsPrincipalFactory<LocalAccount>(userManager, identityOptions)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(LocalAccount user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (identity.FindFirst(ClaimTypes.Name) is { } name)
            identity.RemoveClaim(name);

        identity.AddClaims([
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim("sub", user.Id),
            new Claim("login_method", "Lokal konto")
        ]);
        return identity;
    }
}

internal sealed class LocalAccountRevalidation(
    ILoggerFactory loggerFactory,
    IServiceScopeFactory scopeFactory)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var signIn = scope.ServiceProvider.GetRequiredService<SignInManager<LocalAccount>>();
        return await signIn.ValidateSecurityStampAsync(authenticationState.User) is not null;
    }
}
