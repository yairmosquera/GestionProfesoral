using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class EstudioAcDto
{
    public int Estudio { get; set; }
    public string AreaConocimiento { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioEstudioAc
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioEstudioAc(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException("Debes iniciar sesión para consultar los estudios académicos.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private async Task<List<EstudioAcDto>> ConsultarAsync(string ruta)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("La API rechazó la sesión JWT (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("No tienes permiso para consultar estudios académicos (403).");

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<List<EstudioAcDto>>() ?? new();
    }

    public Task<List<EstudioAcDto>> Listar() =>
        ConsultarAsync("/api/estudios-ac");

    public Task<List<EstudioAcDto>> AreasPorEstudio(int estudio) =>
        ConsultarAsync($"/api/estudios-ac/estudio/{estudio}");

    public async Task<(bool Ok, string Mensaje)> Guardar(
        EstudioAcDto relacion,
        bool editando,
        int estudioOriginal,
        string areaConocimientoOriginal)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para modificar asociaciones académicas.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para modificar asociaciones académicas.");

        string ruta = editando
            ? $"/api/estudios-ac/{estudioOriginal}/{Uri.EscapeDataString(areaConocimientoOriginal)}"
            : "/api/estudios-ac";

        using var solicitud = CrearSolicitud(editando ? HttpMethod.Put : HttpMethod.Post, ruta);
        solicitud.Content = JsonContent.Create(new { relacion.Estudio, relacion.AreaConocimiento });
        using var respuesta = await _http.SendAsync(solicitud);

        return await ProcesarRespuesta(respuesta,
            editando ? "Asociación de área actualizada correctamente." : "Área asociada al estudio correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int estudio, string areaConocimiento)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para retirar asociaciones académicas.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para retirar asociaciones académicas.");

        var ruta = $"/api/estudios-ac/{estudio}/{Uri.EscapeDataString(areaConocimiento)}";
        using var solicitud = CrearSolicitud(HttpMethod.Delete, ruta);
        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(respuesta, "Área retirada del estudio correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> ProcesarRespuesta(
        HttpResponseMessage respuesta, string mensajeExito)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API rechazó el JWT (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permiso para realizar esta operación (403).");

        return await ServicioRespuestaApi.ProcesarAsync(respuesta, mensajeExito);
    }
}