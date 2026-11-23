using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class LineaInvestigacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class LineaInvestigacionCrearDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class LineaInvestigacionActualizarDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class ServicioLineaInvestigacion
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioLineaInvestigacion(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // El JWT se añade a cada petición usando la sesión del circuito
    // actual de Blazor Server. Nunca se guarda en encabezados globales.
    private HttpRequestMessage NuevaPeticion(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para acceder a Líneas de Investigación.");

        var peticion = new HttpRequestMessage(metodo, ruta);
        peticion.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return peticion;
    }

    private static string MensajeError(HttpStatusCode codigo)
    {
        return codigo switch
        {
            HttpStatusCode.Unauthorized =>
                "La API rechazó la sesión (401). Inicia sesión nuevamente y verifica la configuración JWT.",
            HttpStatusCode.Forbidden =>
                "No tienes permisos para realizar esta operación (403).",
            HttpStatusCode.NotFound =>
                "No se encontró el recurso solicitado (404).",
            _ => $"La API devolvió HTTP {(int)codigo}."
        };
    }

    public async Task<List<LineaInvestigacionDto>> ObtenerTodosAsync()
    {
        using var peticion = NuevaPeticion(
            HttpMethod.Get, "/api/lineas-investigacion");
        using var respuesta = await _http.SendAsync(peticion);

        if (!respuesta.IsSuccessStatusCode)
            throw new HttpRequestException(MensajeError(respuesta.StatusCode));

        return await respuesta.Content
            .ReadFromJsonAsync<List<LineaInvestigacionDto>>() ?? new();
    }

    public async Task<LineaInvestigacionDto?> ObtenerPorIdAsync(int id)
    {
        using var peticion = NuevaPeticion(
            HttpMethod.Get, $"/api/lineas-investigacion/{id}");
        using var respuesta = await _http.SendAsync(peticion);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!respuesta.IsSuccessStatusCode)
            throw new HttpRequestException(MensajeError(respuesta.StatusCode));

        return await respuesta.Content.ReadFromJsonAsync<LineaInvestigacionDto>();
    }

    public async Task<Resultado<LineaInvestigacionDto>> CrearAsync(
        LineaInvestigacionCrearDto peticion)
    {
        if (!_sesion.Autenticado)
            return Resultado<LineaInvestigacionDto>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<LineaInvestigacionDto>.Mal(
                "No tienes permisos para crear líneas de investigación.");

        try
        {
            using var solicitud = NuevaPeticion(
                HttpMethod.Post, "/api/lineas-investigacion");
            solicitud.Content = JsonContent.Create(peticion);
            using var respuesta = await _http.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
                return Resultado<LineaInvestigacionDto>.Mal(
                    MensajeError(respuesta.StatusCode));

            var linea = await respuesta.Content
                .ReadFromJsonAsync<LineaInvestigacionDto>();
            return linea is null
                ? Resultado<LineaInvestigacionDto>.Mal("La API no devolvió la línea creada.")
                : Resultado<LineaInvestigacionDto>.Bien(linea);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<LineaInvestigacionDto>.Mal(
                $"No fue posible comunicarse con la API: {ex.Message}");
        }
    }

    public async Task<Resultado<LineaInvestigacionDto>> ActualizarAsync(
        int id, LineaInvestigacionActualizarDto peticion)
    {
        if (!_sesion.Autenticado)
            return Resultado<LineaInvestigacionDto>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<LineaInvestigacionDto>.Mal(
                "No tienes permisos para modificar líneas de investigación.");

        try
        {
            using var solicitud = NuevaPeticion(
                HttpMethod.Put, $"/api/lineas-investigacion/{id}");
            solicitud.Content = JsonContent.Create(peticion);
            using var respuesta = await _http.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
                return Resultado<LineaInvestigacionDto>.Mal(
                    MensajeError(respuesta.StatusCode));

            var linea = await respuesta.Content
                .ReadFromJsonAsync<LineaInvestigacionDto>();
            return linea is null
                ? Resultado<LineaInvestigacionDto>.Mal("La API no devolvió la línea actualizada.")
                : Resultado<LineaInvestigacionDto>.Bien(linea);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<LineaInvestigacionDto>.Mal(
                $"No fue posible comunicarse con la API: {ex.Message}");
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        if (!_sesion.Autenticado)
            return Resultado<bool>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<bool>.Mal(
                "No tienes permisos para retirar líneas de investigación.");

        try
        {
            using var solicitud = NuevaPeticion(
                HttpMethod.Delete, $"/api/lineas-investigacion/{id}");
            using var respuesta = await _http.SendAsync(solicitud);

            return respuesta.IsSuccessStatusCode
                ? Resultado<bool>.Bien(true)
                : Resultado<bool>.Mal(MensajeError(respuesta.StatusCode));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<bool>.Mal(
                $"No fue posible comunicarse con la API: {ex.Message}");
        }
    }
}