using GF2Learn.Web.Auth;
using GF2Learn.Web.Components;
using GF2Learn.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ContentPreprocessor>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<ContentService>();
builder.Services.AddSingleton<NavigationService>();
builder.Services.AddSingleton<PlaygroundParser>();

builder.Services.AddMercantecAuth(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapMercantecAuthEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GF2Learn.Web.Client._Imports).Assembly);

app.Run();
