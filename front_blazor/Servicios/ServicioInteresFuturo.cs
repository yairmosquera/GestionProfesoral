using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class InteresFuturoDto
{
    public int Docente { get; set; }
    public string TerminoClave { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioInteresFuturo
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioInteresFuturo(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para consultar los intereses futuros.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private async Task<List<InteresFuturoDto>> ConsultarAsync(string ruta)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó el JWT al consultar intereses futuros (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "No tienes permiso para consultar intereses futuros (403).");

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<List<InteresFuturoDto>>()
            ?? new List<InteresFuturoDto>();
    }

    public Task<List<InteresFuturoDto>> Listar()
        => ConsultarAsync("/api/intereses-futuros");

    public Task<List<InteresFuturoDto>> PorDocente(int docente)
        => ConsultarAsync($"/api/intereses-futuros/docente/{docente}");

    public async Task<(bool Ok, string Mensaje)> Guardar(
        InteresFuturoDto interes,
        bool editando,
        int docenteOriginal,
        string terminoClaveOriginal)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para modificar intereses futuros.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para modificar intereses futuros.");

        var ruta = editando
            ? $"/api/intereses-futuros/{docenteOriginal}/{Uri.EscapeDataString(terminoClaveOriginal)}"
            : "/api/intereses-futuros";

        using var solicitud = CrearSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post, ruta);

        // Conservar el mismo cuerpo que enviaba la versión anterior.
        solicitud.Content = JsonContent.Create(new
        {
            interes.Docente,
            interes.TerminoClave
        });

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuestaAsync(respuesta,
            editando
                ? "Interés futuro actualizado correctamente."
                : "Interés futuro registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int docente, string terminoClave)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para retirar intereses futuros.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para retirar intereses futuros.");

        using var solicitud = CrearSolicitud(HttpMethod.Delete,
            $"/api/intereses-futuros/{docente}/{Uri.EscapeDataString(terminoClave)}");
        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuestaAsync(
            respuesta, "Interés futuro retirado correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> ProcesarRespuestaAsync(
        HttpResponseMessage respuesta, string mensajeExito)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API rechazó el JWT (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permisos para esta operación (403).");

        return await ServicioRespuestaApi.ProcesarAsync(respuesta, mensajeExito);
    }
}