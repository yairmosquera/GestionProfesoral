using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

// Modelo utilizado por el frontend (sin cambios).
public class DocenteDto
{
    public int Cedula { get; set; }
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string Genero { get; set; } = "";
    public string Cargo { get; set; } = "";
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;
    public string Correo { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string UrlCvlac { get; set; } = "";
    public string Escalafon { get; set; } = "";
    public string Perfil { get; set; } = "";
    public string? CatMinciencia { get; set; }
    public string ConvMinciencia { get; set; } = "";
    public string Nacionalidaad { get; set; } = "";
    public int? LineaInvestigacionPrincipal { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioDocente
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioDocente(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // Cada solicitud usa el token del usuario del circuito actual.
    // No utilizamos DefaultRequestHeaders para no mezclar sesiones.
    private HttpRequestMessage NuevaSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
        {
            throw new UnauthorizedAccessException(
                "Inicia sesión para consultar la información de docentes.");
        }

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);

        return solicitud;
    }

    // Listar docentes.
    public async Task<List<DocenteDto>> Listar()
    {
        using var solicitud = NuevaSolicitud(HttpMethod.Get, "/api/docentes");
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La sesión no es válida o ha expirado. Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "Tu usuario no tiene permiso para consultar docentes.");

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<DocenteDto>>()
            ?? new List<DocenteDto>();
    }

    // Crear o actualizar.
    public async Task<(bool Ok, string Mensaje)> Guardar(
        DocenteDto docente, bool editando)
    {
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para modificar docentes.");

        var ruta = editando
            ? $"/api/docentes/{docente.Cedula}"
            : "/api/docentes";

        using var solicitud = NuevaSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post, ruta);

        solicitud.Content = JsonContent.Create(docente);

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(respuesta);
    }

    // Retirar docente.
    public async Task<(bool Ok, string Mensaje)> Eliminar(int cedula)
    {
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para retirar docentes.");

        using var solicitud = NuevaSolicitud(
            HttpMethod.Delete, $"/api/docentes/{cedula}");

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(respuesta);
    }

    private static async Task<(bool Ok, string Mensaje)>
        ProcesarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
            return (true, "Operación realizada correctamente.");

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "Tu sesión expiró. Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permisos para esta operación.");

        try
        {
            using var json = JsonDocument.Parse(
                await respuesta.Content.ReadAsStringAsync());

            if (json.RootElement.TryGetProperty("mensaje", out var mensaje))
            {
                return (false, mensaje.GetString()
                    ?? "No se pudo completar la operación.");
            }
        }
        catch (JsonException)
        {
            // Respuesta sin cuerpo JSON.
        }

        return (false,
            "No se pudo completar la operación. Revisa los datos.");
    }
}