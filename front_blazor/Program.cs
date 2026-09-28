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

// Servicio de Programas
builder.Services.AddHttpClient<ServicioPrograma>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Servicio de Áreas de Conocimiento
builder.Services.AddHttpClient<ServicioAreaConocimiento>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Servicio de Redes
builder.Services.AddHttpClient<ServicioRed>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Servicio de Líneas de Investigación
builder.Services.AddHttpClient<ServicioLineaInvestigacion>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Servicio de Términos Clave
builder.Services.AddHttpClient<ServicioTerminoClave>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Dirección del frontend Blazor
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