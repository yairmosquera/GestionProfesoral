using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class ApoyoProfesoralDto
{
    public int Estudios { get; set; }
    public byte ConApoyo { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioApoyoProfesoral
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioApoyoProfesoral(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage NuevaSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para consultar los apoyos profesorales.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    public async Task<List<ApoyoProfesoralDto>> Listar()
    {
        using var solicitud = NuevaSolicitud(HttpMethod.Get, "/api/apoyos-profesorales");
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó la sesión (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "No tienes permisos para consultar apoyos profesorales (403).");

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<ApoyoProfesoralDto>>()
            ?? new List<ApoyoProfesoralDto>();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        ApoyoProfesoralDto apoyo, bool editando)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para modificar apoyos profesorales.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para modificar apoyos profesorales.");

        var peticion = new
        {
            apoyo.Estudios,
            apoyo.ConApoyo,
            apoyo.Institucion,
            apoyo.Tipo
        };

        using var solicitud = NuevaSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post,
            editando
                ? $"/api/apoyos-profesorales/{apoyo.Estudios}"
                : "/api/apoyos-profesorales");

        solicitud.Content = JsonContent.Create(peticion);
        using var respuesta = await _http.SendAsync(solicitud);

        return await ProcesarRespuesta(respuesta,
            editando
                ? "Apoyo profesoral actualizado correctamente."
                : "Apoyo profesoral registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int estudios)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para retirar apoyos profesorales.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para retirar apoyos profesorales.");

        using var solicitud = NuevaSolicitud(
            HttpMethod.Delete, $"/api/apoyos-profesorales/{estudios}");
        using var respuesta = await _http.SendAsync(solicitud);

        return await ProcesarRespuesta(
            respuesta, "Apoyo profesoral retirado correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> ProcesarRespuesta(
        HttpResponseMessage respuesta, string mensajeExito)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API rechazó la sesión (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permisos para esta operación (403).");

        return await ServicioRespuestaApi.ProcesarAsync(respuesta, mensajeExito);
    }
}