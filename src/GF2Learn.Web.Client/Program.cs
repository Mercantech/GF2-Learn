using GF2Learn.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<CSharpRunnerService>();
builder.Services.AddScoped<PlaygroundReferenceResolver>();
builder.Services.AddHttpClient("PlaygroundRefs", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();
