using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class TerminoClaveDto
{
    public string Termino { get; set; } = string.Empty;
    public string? TerminoIngles { get; set; }
    public bool Activo { get; set; }
}

public class TerminoClaveCrearDto
{
    public string Termino { get; set; } = string.Empty;
    public string? TerminoIngles { get; set; }
}

public class TerminoClaveActualizarDto
{
    public string? TerminoIngles { get; set; }
}

public class ServicioTerminoClave
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioTerminoClave(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // Cada petición lleva el JWT del usuario que inició sesión.
    // No configuramos DefaultRequestHeaders para evitar mezclar usuarios.
    private HttpRequestMessage Preparar(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException("Inicia sesión para usar Términos Clave.");

        var mensaje = new HttpRequestMessage(metodo, ruta);
        mensaje.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return mensaje;
    }

    private static string MensajeHttp(HttpStatusCode estado)
    {
        return estado switch
        {
            HttpStatusCode.Unauthorized => "Sesión no válida o expirada. Inicia sesión nuevamente.",
            HttpStatusCode.Forbidden => "No tienes permiso para realizar esta operación.",
            _ => $"La API devolvió HTTP {(int)estado}."
        };
    }

    public async Task<List<TerminoClaveDto>> ObtenerTodosAsync()
    {
        using var solicitud = Preparar(HttpMethod.Get, "/api/terminos-clave");
        using var respuesta = await _http.SendAsync(solicitud);
        if (!respuesta.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"No fue posible cargar los términos clave: {MensajeHttp(respuesta.StatusCode)}",
                null, respuesta.StatusCode);

        return await respuesta.Content.ReadFromJsonAsync<List<TerminoClaveDto>>()
            ?? new List<TerminoClaveDto>();
    }

    public async Task<TerminoClaveDto?> ObtenerPorTerminoAsync(string termino)
    {
        var ruta = $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";
        using var solicitud = Preparar(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);
        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;
        if (!respuesta.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"No fue posible consultar el término: {MensajeHttp(respuesta.StatusCode)}",
                null, respuesta.StatusCode);

        return await respuesta.Content.ReadFromJsonAsync<TerminoClaveDto>();
    }

    public Task<Resultado<TerminoClaveDto>> CrearAsync(TerminoClaveCrearDto peticion)
    {
        return EnviarConDtoAsync(HttpMethod.Post, "/api/terminos-clave", peticion);
    }

    public Task<Resultado<TerminoClaveDto>> ActualizarAsync(
        string termino, TerminoClaveActualizarDto peticion)
    {
        var ruta = $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";
        return EnviarConDtoAsync(HttpMethod.Put, ruta, peticion);
    }

    private async Task<Resultado<TerminoClaveDto>> EnviarConDtoAsync(
        HttpMethod metodo, string ruta, object cuerpo)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return Resultado<TerminoClaveDto>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<TerminoClaveDto>.Mal("No tienes permiso para modificar términos clave.");

        try
        {
            using var solicitud = Preparar(metodo, ruta);
            solicitud.Content = JsonContent.Create(cuerpo);
            using var respuesta = await _http.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<TerminoClaveDto>.Mal(
                    $"No fue posible completar la operación. {MensajeHttp(respuesta.StatusCode)}");

            var datos = await respuesta.Content.ReadFromJsonAsync<TerminoClaveDto>();
            return datos is null
                ? Resultado<TerminoClaveDto>.Mal("La API no devolvió el término clave.")
                : Resultado<TerminoClaveDto>.Bien(datos);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<TerminoClaveDto>.Mal($"No fue posible comunicarse con la API: {ex.Message}");
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(string termino)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return Resultado<bool>.Mal("Debes iniciar sesión.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<bool>.Mal("No tienes permiso para retirar términos clave.");

        try
        {
            var ruta = $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";
            using var solicitud = Preparar(HttpMethod.Delete, ruta);
            using var respuesta = await _http.SendAsync(solicitud);
            return respuesta.IsSuccessStatusCode
                ? Resultado<bool>.Bien(true)
                : Resultado<bool>.Mal($"No fue posible retirar el término clave. {MensajeHttp(respuesta.StatusCode)}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<bool>.Mal($"No fue posible comunicarse con la API: {ex.Message}");
        }
    }
}