using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class BecaDto
{
    public int Estudios { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioBeca
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioBeca(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage NuevaSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
        {
            throw new UnauthorizedAccessException(
                "La sesión no está activa. Inicia sesión para consultar becas.");
        }

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    public async Task<List<BecaDto>> Listar()
    {
        using var solicitud = NuevaSolicitud(HttpMethod.Get, "/api/becas");
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó el JWT al consultar becas (401). Vuelve a iniciar sesión.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "No tienes permisos para consultar becas (403).");

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<BecaDto>>()
            ?? new List<BecaDto>();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        BecaDto beca, bool editando)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para modificar becas.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para modificar becas.");

        var peticion = new
        {
            beca.Estudios,
            beca.Tipo,
            beca.Institucion,
            beca.FechaInicio,
            beca.FechaFin
        };

        using var solicitud = NuevaSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post,
            editando ? $"/api/becas/{beca.Estudios}" : "/api/becas");

        solicitud.Content = JsonContent.Create(peticion);

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(respuesta,
            editando
                ? "Beca actualizada correctamente."
                : "Beca registrada correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int estudios)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para retirar becas.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para retirar becas.");

        using var solicitud =
            NuevaSolicitud(HttpMethod.Delete, $"/api/becas/{estudios}");

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(
            respuesta, "Beca retirada correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> ProcesarRespuesta(
        HttpResponseMessage respuesta, string mensajeExito)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API rechazó el JWT (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permisos para esta operación (403).");

        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, mensajeExito);
    }
}