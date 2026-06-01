using System.Security.Claims;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Components;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;
    // Cloudflare Tunnel (cloudflared) → localhost — trust proxy headers
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddSingleton<ContentPreprocessor>();
builder.Services.AddSingleton<KnowledgeCheckCatalog>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<ContentService>();
builder.Services.AddSingleton<NavigationService>();
builder.Services.AddSingleton<PlaygroundParser>();
builder.Services.AddSingleton<RunnableCodeParser>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<Gf2LearnDbContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddScoped<IKnowledgeCheckProgressService, KnowledgeCheckProgressService>();
    builder.Services.AddSingleton<KnowledgeCheckProgressScope>();
}

builder.Services.AddMercantecAuth(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);

if (!string.IsNullOrWhiteSpace(connectionString))
{
    const int maxAttempts = 10;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Gf2LearnDbContext>();
            db.Database.Migrate();
            app.Logger.LogInformation("Database migration completed.");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(ex, "Database migration failed, retrying ({Attempt}/{MaxAttempts})...", attempt, maxAttempts);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Database migration failed — knowledge-check progress will not be saved.");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

var behindReverseProxy = builder.Configuration.GetValue("GF2_BEHIND_REVERSE_PROXY", false);
if (!behindReverseProxy)
    app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapMercantecAuthEndpoints();

if (!string.IsNullOrWhiteSpace(connectionString))
{
    var progress = app.MapGroup("/api/progress").RequireAuthorization();

    progress.MapGet("/knowledge-check/{contentSlug}", async (
        string contentSlug,
        ClaimsPrincipal user,
        IKnowledgeCheckProgressService progressService,
        CancellationToken cancellationToken) =>
    {
        var userSub = GetUserSub(user);
        if (userSub is null)
            return Results.Unauthorized();

        var answers = await progressService.GetAnswersAsync(userSub, contentSlug, cancellationToken);
        return Results.Ok(answers);
    });

    progress.MapPost("/knowledge-check", async (
        SaveKnowledgeCheckAnswerRequest request,
        ClaimsPrincipal user,
        IKnowledgeCheckProgressService progressService,
        CancellationToken cancellationToken) =>
    {
        var userSub = GetUserSub(user);
        if (userSub is null)
            return Results.Unauthorized();

        await progressService.SaveAnswerAsync(
            userSub,
            request.ContentSlug,
            request.QuestionIndex,
            request.SelectedIndex,
            request.IsCorrect,
            cancellationToken);

        return Results.NoContent();
    }).DisableAntiforgery();
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GF2Learn.Web.Client._Imports).Assembly);

app.Run();

static string? GetUserSub(ClaimsPrincipal user) =>
    user.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? user.FindFirstValue("sub");
