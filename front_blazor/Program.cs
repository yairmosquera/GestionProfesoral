using FrontGestion.Components;
using FrontGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONFIGURACIÓN DE BLAZOR
// =====================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Dirección de la API
var apiUrl = builder.Configuration["ApiUrl"]
    ?? "http://localhost:5000";

// =====================================================
// AUTENTICACIÓN - VERSIÓN 3
// =====================================================

// Cliente para autenticación. No incluye tokens por defecto.
builder.Services.AddHttpClient("ApiGestionSesion", cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// IMPORTANTE: la sesión debe ser Scoped para compartirse entre
// Login.razor y los servicios del MISMO circuito Blazor Server.
// AddHttpClient<ServicioSesionJwt> la registraría como transient.
builder.Services.AddScoped<ServicioSesionJwt>(sp =>
    new ServicioSesionJwt(
        sp.GetRequiredService<IHttpClientFactory>()
          .CreateClient("ApiGestionSesion")));

// =====================================================
// SERVICIOS DE LA VERSIÓN 1
// =====================================================

builder.Services.AddHttpClient<ServicioPrograma>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioAreaConocimiento>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioRed>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioLineaInvestigacion>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioTerminoClave>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Cliente HTTP general usado por otras partes del frontend.
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiUrl)
    });

// =====================================================
// SERVICIOS DE LA VERSIÓN 2
// =====================================================

builder.Services.AddHttpClient<ServicioDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioEstudioRealizado>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioEvaluacionDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioExperienciaDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioReconocimientoDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioRedDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioDocenteDepartamento>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioInteresFuturo>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioEstudioAc>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioApoyoProfesoral>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioBeca>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioDocenteMaestro>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient<ServicioDocenteMaestroEdicion>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// =====================================================
// CONFIGURACIÓN DEL FRONTEND
// =====================================================

builder.WebHost.UseUrls("http://localhost:5100");

var app = builder.Build();

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