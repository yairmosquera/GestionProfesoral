
using FrontGestion.Components;
using FrontGestion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONFIGURACIÓN DE BLAZOR SERVER
// =====================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// =====================================================
// CONFIGURACIÓN DE LA API
// =====================================================

var apiUrl = builder.Configuration["ApiUrl"];

if (string.IsNullOrWhiteSpace(apiUrl))
{
    if (builder.Environment.IsDevelopment())
    {
        apiUrl = "http://localhost:5000";
    }
    else
    {
        throw new InvalidOperationException(
            "Debe configurar ApiUrl en producción.");
    }
}

if (!Uri.TryCreate(
        apiUrl,
        UriKind.Absolute,
        out var apiUri) ||
    (apiUri.Scheme != Uri.UriSchemeHttp &&
     apiUri.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException(
        "ApiUrl debe ser una dirección HTTP o HTTPS válida.");
}

if (!builder.Environment.IsDevelopment() &&
    apiUri.Scheme != Uri.UriSchemeHttps)
{
    throw new InvalidOperationException(
        "En producción ApiUrl debe utilizar HTTPS.");
}

// =====================================================
// AUTENTICACIÓN JWT - VERSIÓN 3
// =====================================================

builder.Services.AddHttpClient(
    "ApiGestionSesion",
    cliente =>
    {
        cliente.BaseAddress = apiUri;
    });

// Servicio de sesión compartido por circuito Blazor.
// No registrar también como Transient o Singleton.

builder.Services.AddScoped<ServicioSesionJwt>(sp =>
    new ServicioSesionJwt(
        sp.GetRequiredService<IHttpClientFactory>()
          .CreateClient("ApiGestionSesion")));

// =====================================================
// SERVICIOS - VERSIÓN 1
// =====================================================

builder.Services.AddHttpClient<ServicioPrograma>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioAreaConocimiento>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioRed>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioLineaInvestigacion>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioTerminoClave>(
    cliente => cliente.BaseAddress = apiUri);

// Cliente HTTP general.

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = apiUri
    });

// =====================================================
// SERVICIOS - VERSIÓN 2
// =====================================================

builder.Services.AddHttpClient<ServicioDocente>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioEstudioRealizado>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioEvaluacionDocente>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioExperienciaDocente>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioReconocimientoDocente>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioRedDocente>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioDocenteDepartamento>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioInteresFuturo>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioEstudioAc>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioApoyoProfesoral>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioBeca>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioDocenteMaestro>(
    cliente => cliente.BaseAddress = apiUri);

builder.Services.AddHttpClient<ServicioDocenteMaestroEdicion>(
    cliente => cliente.BaseAddress = apiUri);

// =====================================================
// CONFIGURACIÓN LOCAL
// =====================================================

// El puerto 5100 solamente se utiliza en desarrollo.
// En MonsterASP.NET el hosting administra el puerto.

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5100");
}

// =====================================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// =====================================================

var app = builder.Build();

// =====================================================
// MIDDLEWARE
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

// =====================================================
// COMPONENTES BLAZOR SERVER
// =====================================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// =====================================================
// INICIAR APLICACIÓN
// =====================================================

app.Run();
