
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
// SERVICIOS DE LA VERSIÓN 1
// =====================================================

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


builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiUrl)
    });


// =====================================================
// SERVICIOS DE LA VERSIÓN 2
// =====================================================

// Servicio de Docentes
builder.Services.AddHttpClient<ServicioDocente>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Estudios Realizados
builder.Services.AddHttpClient<ServicioEstudioRealizado>(cliente =>
{
    cliente.BaseAddress = new Uri(apiUrl);
});

// Evaluacion Docente
builder.Services.AddHttpClient<ServicioEvaluacionDocente>(
    cliente =>
    {
        cliente.BaseAddress = new Uri(apiUrl);
    });


builder.Services.AddHttpClient<ServicioExperienciaDocente>(
    cliente =>
    {
        cliente.BaseAddress = new Uri(apiUrl);
    });


builder.Services.AddHttpClient<ServicioReconocimientoDocente>(
    cliente =>
    {
        cliente.BaseAddress = new Uri(apiUrl);
    });


builder.Services.AddHttpClient<ServicioRedDocente>(
    cliente =>
    {
        cliente.BaseAddress = new Uri(apiUrl);
    });


builder.Services.AddHttpClient<ServicioDocenteDepartamento>(
    cliente =>
    {
        cliente.BaseAddress = new Uri(apiUrl);
    });



// =====================================================
// CONFIGURACIÓN DEL FRONTEND
// =====================================================

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

// Configuración de componentes interactivos
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ejecutar aplicación
app.Run();
