using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class ExperienciaDocenteDto
{
    public int Id { get; set; }
    public string NombreCargo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public DateTime? FechaFin { get; set; }
    public int Docente { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioExperienciaDocente
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioExperienciaDocente(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException("Debes iniciar sesión para consultar los datos.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private async Task<List<ExperienciaDocenteDto>> Consultar(string ruta)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("HTTP 401: La sesión JWT es inválida o expiró.");
        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("HTTP 403: No tienes permiso para consultar esta información.");

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<List<ExperienciaDocenteDto>>() ?? new();
    }

    public Task<List<ExperienciaDocenteDto>> Listar()
        => Consultar("/api/experiencias-docentes");

    public Task<List<ExperienciaDocenteDto>> PorDocente(int cedula)
        => Consultar($"/api/experiencias-docentes/docente/{cedula}");

    public async Task<(bool Ok, string Mensaje)> Guardar(ExperienciaDocenteDto experiencia, bool editando)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para guardar.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para modificar estos datos.");

        var ruta = editando
            ? $"/api/experiencias-docentes/{experiencia.Id}"
            : "/api/experiencias-docentes";

        using var solicitud = CrearSolicitud(editando ? HttpMethod.Put : HttpMethod.Post, ruta);
        solicitud.Content = JsonContent.Create(new
        {
            experiencia.NombreCargo,
            experiencia.Institucion,
            experiencia.Tipo,
            experiencia.FechaInicio,
            experiencia.FechaFin,
            experiencia.Docente
        });

        using var respuesta = await _http.SendAsync(solicitud);
        return await LeerResultado(respuesta);
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int id)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para retirar.");
        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para retirar estos datos.");

        using var solicitud = CrearSolicitud(HttpMethod.Delete, $"/api/experiencias-docentes/{id}");
        using var respuesta = await _http.SendAsync(solicitud);
        return await LeerResultado(respuesta);
    }

    private static async Task<(bool Ok, string Mensaje)> LeerResultado(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
            return (true, "Operación realizada correctamente.");
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "Tu sesión expiró o no es válida. Inicia sesión nuevamente.");
        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permiso para realizar esta operación.");

        var contenido = await respuesta.Content.ReadAsStringAsync();
        try
        {
            using var documento = JsonDocument.Parse(contenido);
            var raiz = documento.RootElement;
            if (raiz.TryGetProperty("mensaje", out var mensaje) &&
                mensaje.ValueKind == JsonValueKind.String)
                return (false, mensaje.GetString() ?? "No fue posible completar la operación.");
            if (raiz.TryGetProperty("errors", out _))
                return (false, "Revisa los campos obligatorios y sus valores.");
        }
        catch (JsonException)
        {
            // La respuesta puede no ser JSON.
        }
        return (false, $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
