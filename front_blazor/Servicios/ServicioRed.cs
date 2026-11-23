using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class RedDto
{
    public int Idr { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class RedCrearDto
{
    public int Idr { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}

public class RedActualizarDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}

public class ServicioRed
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioRed(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // El JWT se incorpora a CADA solicitud sin compartir cabeceras entre sesiones.
    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para acceder al módulo Redes.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private static void ValidarAcceso(HttpResponseMessage respuesta)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó el JWT (401). Vuelve a iniciar sesión.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "Tu usuario no tiene permisos para consultar Redes (403).");
    }

    public async Task<List<RedDto>> ObtenerTodosAsync()
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, "/api/redes");
        using var respuesta = await _http.SendAsync(solicitud);
        ValidarAcceso(respuesta);
        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<List<RedDto>>() ?? new();
    }

    public async Task<RedDto?> ObtenerPorIdAsync(int id)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, $"/api/redes/{id}");
        using var respuesta = await _http.SendAsync(solicitud);
        ValidarAcceso(respuesta);
        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;
        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<RedDto>();
    }

    public Task<Resultado<RedDto>> CrearAsync(RedCrearDto peticion)
        => EnviarConRespuesta(HttpMethod.Post, "/api/redes", peticion, "crear");

    public Task<Resultado<RedDto>> ActualizarAsync(int id, RedActualizarDto peticion)
        => EnviarConRespuesta(HttpMethod.Put, $"/api/redes/{id}", peticion, "actualizar");

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        if (!_sesion.Autenticado)
            return Resultado<bool>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<bool>.Mal("No tienes permiso para retirar redes.");

        try
        {
            using var solicitud = CrearSolicitud(HttpMethod.Delete, $"/api/redes/{id}");
            using var respuesta = await _http.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<bool>.Mal(MensajeError(respuesta, "retirar"));
            return Resultado<bool>.Bien(true);
        }
        catch (HttpRequestException)
        {
            return Resultado<bool>.Mal("No fue posible comunicarse con la API.");
        }
        catch (TaskCanceledException)
        {
            return Resultado<bool>.Mal("La solicitud agotó el tiempo de espera.");
        }
    }

    private async Task<Resultado<RedDto>> EnviarConRespuesta(
        HttpMethod metodo, string ruta, object peticion, string operacion)
    {
        if (!_sesion.Autenticado)
            return Resultado<RedDto>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<RedDto>.Mal("No tienes permiso para modificar redes.");

        try
        {
            using var solicitud = CrearSolicitud(metodo, ruta);
            solicitud.Content = JsonContent.Create(peticion);
            using var respuesta = await _http.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<RedDto>.Mal(MensajeError(respuesta, operacion));

            var red = await respuesta.Content.ReadFromJsonAsync<RedDto>();
            return red is null
                ? Resultado<RedDto>.Mal("La API no devolvió la red registrada.")
                : Resultado<RedDto>.Bien(red);
        }
        catch (HttpRequestException)
        {
            return Resultado<RedDto>.Mal("No fue posible comunicarse con la API.");
        }
        catch (TaskCanceledException)
        {
            return Resultado<RedDto>.Mal("La solicitud agotó el tiempo de espera.");
        }
    }

    private static string MensajeError(HttpResponseMessage respuesta, string operacion)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return "Sesión no válida o expirada (401). Inicia sesión nuevamente.";
        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return "No tienes permiso para realizar esta operación (403).";
        return $"No fue posible {operacion} la red. Código: {(int)respuesta.StatusCode}.";
    }
}