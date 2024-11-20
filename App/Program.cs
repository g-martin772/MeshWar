using App.Endpoints;
using App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<WarStateProvider>();
builder.Services.AddSingleton<WarMachine>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<WarMachine>());

var app = builder.Build();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapWarEndpoints();

app.Run("http://*:1337");