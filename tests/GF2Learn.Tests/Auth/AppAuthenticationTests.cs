using System.Security.Claims;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Data;
using GF2Learn.Web.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GF2Learn.Tests.Auth;

public sealed class AppAuthenticationTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("central", false)]
    [InlineData(" Local ", true)]
    public void Login_mode_is_selected_from_configuration(string? mode, bool isLocal)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Auth:Mode"] = mode })
            .Build();

        Assert.Equal(isLocal, AppAuthentication.UseLocalAccounts(configuration));
    }

    [Fact]
    public void An_invalid_login_mode_reports_a_configuration_error()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Auth:Mode"] = "external" })
            .Build();

        Assert.Throws<InvalidOperationException>(() => AppAuthentication.UseLocalAccounts(configuration));
    }

    [Theory]
    [InlineData(null, "/")]
    [InlineData("/curriculum/05-loekker?step=2#foreach", "/curriculum/05-loekker?step=2#foreach")]
    [InlineData("https://learn.gf2.test/join/demo", "/join/demo")]
    [InlineData("https://example.com/join/demo", "/")]
    [InlineData("http://learn.gf2.test/join/demo", "/")]
    [InlineData("//example.com/", "/")]
    public void Login_returns_to_a_local_destination(string? returnUrl, string expected)
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("learn.gf2.test");

        Assert.Equal(expected, AppAuthentication.ResolveReturnUrl(context.Request, returnUrl));
    }

    [Fact]
    public async Task Local_account_uses_its_own_cookie_and_keeps_subject_after_reset()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContextFactory<Gf2LearnDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddLocalAccountAuth();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var scopedServices = scope.ServiceProvider;
        var schemes = scopedServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var users = scopedServices.GetRequiredService<UserManager<LocalAccount>>();
        var signIn = scopedServices.GetRequiredService<SignInManager<LocalAccount>>();
        var context = new DefaultHttpContext { RequestServices = scopedServices };
        signIn.Context = context;
        context.Request.Scheme = "https";

        Assert.Null(await schemes.GetSchemeAsync(MercantecAuthOptions.SchemeName));
        Assert.Equal(IdentityConstants.ApplicationScheme, (await schemes.GetDefaultAuthenticateSchemeAsync())?.Name);

        const string password = "synthetic passphrase for test";
        var user = new LocalAccount { UserName = "local.test", DisplayName = "Test Elev" };
        Assert.True((await users.CreateAsync(user, password)).Succeeded);
        Assert.NotEqual(password, user.PasswordHash);
        Assert.StartsWith("local:", user.Id);
        Assert.False((await users.CreateAsync(
            new LocalAccount { UserName = "LOCAL.TEST", DisplayName = "Another User" }, password)).Succeeded);

        for (var attempt = 0; attempt < 5; attempt++)
            Assert.False((await signIn.CheckPasswordSignInAsync(user, "incorrect", lockoutOnFailure: true)).Succeeded);
        Assert.True((await signIn.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)).IsLockedOut);
        Assert.True((await users.SetLockoutEndDateAsync(user, null)).Succeeded);

        Assert.True((await signIn.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true)).Succeeded);
        Assert.Equal(user.Id, context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal(user.Id, context.User.FindFirstValue("sub"));
        Assert.Equal("Test Elev", context.User.Identity?.Name);
        Assert.StartsWith(".gf2learn.local.auth=", Assert.Single(context.Response.Headers.SetCookie));
        Assert.Contains("secure", context.Response.Headers.SetCookie.ToString(), StringComparison.OrdinalIgnoreCase);

        var token = await users.GeneratePasswordResetTokenAsync(user);
        Assert.True((await users.ResetPasswordAsync(user, token, "another synthetic test passphrase")).Succeeded);
        Assert.False(await users.CheckPasswordAsync(user, password));
        Assert.Null(await signIn.ValidateSecurityStampAsync(context.User));
        Assert.Equal(user.Id, (await users.FindByNameAsync("local.test"))?.Id);
    }
}
