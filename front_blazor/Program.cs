using FrontGestion.Components;
using FrontGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Servicios de Blazor
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Dirección de la API
var apiUrl = builder.Configuration["ApiUrl"]
    ?? "http://localhost:5000";

builder.Services.AddHttpClient<ServicioPrograma>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioAreaConocimiento>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.WebHost.UseUrls("http://localhost:5100");
var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();