using System.Security.Claims;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Components;
using GF2Learn.Web.Configuration;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using GF2Learn.Web.Options;
using GF2Learn.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

EnvFileLoader.TryLoadFromAncestors(AppContext.BaseDirectory);
if (EnvFileLoader.LoadedPath is null)
    EnvFileLoader.TryLoadFromAncestors(Directory.GetCurrentDirectory());

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
builder.Services.AddSingleton<ExerciseCatalog>();
builder.Services.AddSingleton<ExercisePageBuilder>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<ContentService>();
builder.Services.AddSingleton<NavigationService>();
builder.Services.AddSingleton<PlaygroundParser>();
builder.Services.AddSingleton<RunnableCodeParser>();
builder.Services.AddSingleton<ProjectSolutionCatalog>();
builder.Services.AddSingleton<ExerciseAiContextService>();

builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddHttpClient<IExerciseAiService, ExerciseAiService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.Timeout = TimeSpan.FromSeconds(90);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<Gf2LearnDbContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddScoped<IKnowledgeCheckProgressService, KnowledgeCheckProgressService>();
    builder.Services.AddSingleton<KnowledgeCheckProgressScope>();
    builder.Services.AddScoped<IExerciseProgressService, ExerciseProgressService>();
    builder.Services.AddSingleton<ExerciseProgressScope>();
}

var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"]
    ?? "/app/data-protection-keys";
Directory.CreateDirectory(dataProtectionKeysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("GF2Learn");

builder.Services.AddMercantecAuth(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddRequestTimeouts();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

var openAiKey = builder.Configuration["OpenAi:ApiKey"];
if (!string.IsNullOrWhiteSpace(openAiKey))
    app.Logger.LogInformation("OpenAI er konfigureret (model: {Model}).", builder.Configuration["OpenAi:Model"] ?? "gpt-4o-mini");
else
    app.Logger.LogWarning(
        "OpenAI er IKKE konfigureret. Sæt OpenAi__ApiKey i .env{EnvPath} (genstart appen).",
        EnvFileLoader.LoadedPath is { } p ? $" ({p})" : " i projektroden");

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
            var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
            if (pending.Count > 0)
                app.Logger.LogInformation("Anvender ventende migrationer: {Migrations}", string.Join(", ", pending));

            db.Database.Migrate();

            var applied = await db.Database.GetAppliedMigrationsAsync();
            app.Logger.LogInformation(
                "Database migration completed. Applied: {Migrations}",
                string.Join(", ", applied));
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
app.UseRequestTimeouts();
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

    progress.MapGet("/exercise/{contentSlug}", async (
        string contentSlug,
        ClaimsPrincipal user,
        IExerciseProgressService progressService,
        CancellationToken cancellationToken) =>
    {
        var userSub = GetUserSub(user);
        if (userSub is null)
            return Results.Unauthorized();

        var answers = await progressService.GetAnswersAsync(userSub, contentSlug, cancellationToken);
        return Results.Ok(answers);
    });

    progress.MapPost("/exercise", async (
        SaveExercisePartRequest request,
        ClaimsPrincipal user,
        IServiceProvider services,
        CancellationToken cancellationToken) =>
    {
        var userSub = GetUserSub(user);
        if (userSub is null)
            return Results.Unauthorized();

        if (request.PartIndex < 0)
            return Results.BadRequest();

        var progressService = services.GetService<IExerciseProgressService>();
        if (progressService is null)
        {
            return Results.Problem(
                detail: "Database er ikke konfigureret (ConnectionStrings:DefaultConnection).",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        try
        {
            await progressService.SavePartAsync(
                userSub,
                request.ContentSlug,
                request.PartIndex,
                request.AnswerText,
                cancellationToken);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: "Kunne ikke gemme opgavesvar: " + ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }).DisableAntiforgery();
}

app.MapGet("/api/exercise-ai/status", (IExerciseAiService ai) =>
    Results.Ok(new ExerciseAiStatusResponse(
        ai.IsConfigured,
        ai.IsConfigured ? null : "OpenAI er ikke konfigureret (sæt OpenAi__ApiKey i .env).")))
    .AllowAnonymous();

var exerciseAi = app.MapGroup("/api/exercise-ai").RequireAuthorization();

exerciseAi.MapPost("/hint", async (
    ExerciseAiRequest request,
    IExerciseAiService ai,
    ExerciseAiContextService contexts,
    ILogger<ExerciseAiService> logger,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("AI hint: {Slug} del {Part}", request.ContentSlug, request.PartIndex);

    if (!ai.IsConfigured)
    {
        return Results.Json(
            new ExerciseAiStatusResponse(false, "OpenAI er ikke konfigureret."),
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    if (string.IsNullOrWhiteSpace(request.StudentCode))
        return Results.BadRequest();

    var context = contexts.GetPartContext(request.ContentSlug, request.PartIndex);
    if (context is null)
        return Results.NotFound();

    try
    {
        var hint = await ai.GetHintAsync(
            context,
            request.StudentCode,
            request.ConsoleOutput,
            cancellationToken);
        return Results.Ok(hint);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "AI hint fejlede for {Slug} del {Part}", request.ContentSlug, request.PartIndex);
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status502BadGateway);
    }
})
.DisableAntiforgery()
.WithRequestTimeout(TimeSpan.FromMinutes(2));

exerciseAi.MapPost("/check", async (
    ExerciseAiRequest request,
    IExerciseAiService ai,
    ExerciseAiContextService contexts,
    ILogger<ExerciseAiService> logger,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("AI check: {Slug} del {Part}", request.ContentSlug, request.PartIndex);

    if (!ai.IsConfigured)
    {
        return Results.Json(
            new ExerciseAiStatusResponse(false, "OpenAI er ikke konfigureret."),
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    if (string.IsNullOrWhiteSpace(request.StudentCode))
        return Results.BadRequest();

    var context = contexts.GetPartContext(request.ContentSlug, request.PartIndex);
    if (context is null)
        return Results.NotFound();

    try
    {
        var check = await ai.CheckSolutionAsync(
            context,
            request.StudentCode,
            request.ConsoleOutput,
            cancellationToken);
        return Results.Ok(check);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "AI check fejlede for {Slug} del {Part}", request.ContentSlug, request.PartIndex);
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status502BadGateway);
    }
})
.DisableAntiforgery()
.WithRequestTimeout(TimeSpan.FromMinutes(2));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GF2Learn.Web.Client._Imports).Assembly);

app.Run();

static string? GetUserSub(ClaimsPrincipal user) =>
    user.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? user.FindFirstValue("sub");
