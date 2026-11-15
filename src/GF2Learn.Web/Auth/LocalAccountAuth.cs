using System.Text;
using GF2Learn.Web.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GF2Learn.Web.Auth;

public static class LocalAccountAuth
{
    public static void AddLocalAccountAuth(this IServiceCollection services)
    {
        services.AddAppCookie(
            IdentityConstants.ApplicationScheme,
            ".gf2learn.local.auth",
            options => options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync);
        services.AddIdentityCore<LocalAccount>(options =>
            {
                options.Password.RequiredLength = LocalAccount.MinimumPasswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<Gf2LearnDbContext>()
            .AddClaimsPrincipalFactory<LocalAccountClaimsFactory>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<AuthenticationStateProvider, LocalAccountRevalidation>();
        services.AddSingleton(new AppAuthStatus(IsLocal: true, IsConfigured: true));
    }

    public static void MapLocalAccountLogin(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/auth/login", (HttpRequest request, string? returnUrl) =>
            Results.LocalRedirect(AppAuthentication.LocalAccountUrl(
                request, AppAuthentication.ResolveReturnUrl(request, returnUrl))));

    public static async Task<int> RunCommandAsync(IServiceProvider services, string[] args, bool useLocalAccounts)
    {
        if (!useLocalAccounts || args.Length != 3 || args[1] != "reset-password"
            || string.IsNullOrWhiteSpace(args[2]) || Console.IsInputRedirected)
        {
            Console.Error.WriteLine("Brug en interaktiv terminal i lokal login-tilstand: local-auth reset-password <brugernavn>");
            return 1;
        }

        await using var scope = services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<LocalAccount>>();
        var account = await users.FindByNameAsync(args[2].Trim());
        if (account is null)
        {
            Console.Error.WriteLine("Den lokale konto findes ikke.");
            return 1;
        }

        var password = ReadPassword("Ny adgangskode: ");
        var repeated = ReadPassword("Gentag adgangskode: ");
        if (password != repeated || password.Length is < LocalAccount.MinimumPasswordLength or > LocalAccount.MaximumPasswordLength)
        {
            Console.Error.WriteLine($"Adgangskoderne skal være ens og have {LocalAccount.MinimumPasswordLength}–{LocalAccount.MaximumPasswordLength} tegn.");
            return 1;
        }

        var token = await users.GeneratePasswordResetTokenAsync(account);
        var reset = await users.ResetPasswordAsync(account, token, password);
        if (!reset.Succeeded)
        {
            Console.Error.WriteLine("Adgangskoden kunne ikke nulstilles. Prøv igen.");
            return 1;
        }

        var unlocked = await users.SetLockoutEndDateAsync(account, null);
        var counterReset = await users.ResetAccessFailedCountAsync(account);
        if (!unlocked.Succeeded || !counterReset.Succeeded)
        {
            Console.Error.WriteLine("Adgangskoden er nulstillet. En eventuel loginspærring kan stadig være aktiv i op til 15 minutter.");
            return 1;
        }

        Console.WriteLine("Adgangskoden er nulstillet.");
        return 0;
    }

    private static string ReadPassword(string prompt)
    {
        Console.Write(prompt);
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return password.ToString();
            }

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                password.Length--;
            else if (!char.IsControl(key.KeyChar))
                password.Append(key.KeyChar);
        }
    }
}
