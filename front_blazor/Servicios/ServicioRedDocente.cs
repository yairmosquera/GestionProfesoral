using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class RedDocenteDto
{
    public int Red { get; set; }
    public int Docente { get; set; }
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public string? FechaFin { get; set; }
    public string? ActDestacadas { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioRedDocente
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioRedDocente(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para acceder a Redes de Docentes.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);

        return solicitud;
    }

    private async Task<List<RedDocenteDto>> ConsultarAsync(string ruta)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó la sesión (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "Tu usuario no tiene permisos para consultar redes de docentes (403).");

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<RedDocenteDto>>()
            ?? new List<RedDocenteDto>();
    }

    public Task<List<RedDocenteDto>> Listar()
        => ConsultarAsync("/api/redes-docentes");

    public Task<List<RedDocenteDto>> PorDocente(int cedula)
        => ConsultarAsync($"/api/redes-docentes/docente/{cedula}");

    public async Task<(bool Ok, string Mensaje)> Guardar(
        RedDocenteDto relacion, bool editando)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para modificar redes de docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para modificar redes de docentes.");

        var peticion = new
        {
            relacion.Red,
            relacion.Docente,
            relacion.FechaInicio,
            relacion.FechaFin,
            relacion.ActDestacadas
        };

        var ruta = editando
            ? $"/api/redes-docentes/{relacion.Red}/{relacion.Docente}"
            : "/api/redes-docentes";

        using var solicitud = CrearSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post, ruta);

        solicitud.Content = JsonContent.Create(peticion);

        using var respuesta = await _http.SendAsync(solicitud);

        return await LeerResultado(
            respuesta,
            editando
                ? "Relación actualizada correctamente."
                : "Red asociada al docente correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int red, int docente)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para retirar redes de docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para retirar redes de docentes.");

        using var solicitud = CrearSolicitud(
            HttpMethod.Delete, $"/api/redes-docentes/{red}/{docente}");

        using var respuesta = await _http.SendAsync(solicitud);

        return await LeerResultado(
            respuesta,
            "Asociación retirada correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> LeerResultado(
        HttpResponseMessage respuesta,
        string mensajeExito)
    {
        if (respuesta.IsSuccessStatusCode)
            return (true, mensajeExito);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API rechazó la sesión (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "Tu usuario no tiene permisos para esta operación (403).");

        string contenido = await respuesta.Content.ReadAsStringAsync();

        try
        {
            using var documento = JsonDocument.Parse(contenido);
            var raiz = documento.RootElement;

            if (raiz.TryGetProperty("mensaje", out var mensaje))
                return (false, mensaje.GetString() ?? "Ocurrió un error.");

            if (raiz.TryGetProperty("errors", out var errores) &&
                errores.ValueKind == JsonValueKind.Object)
            {
                var mensajes = new List<string>();

                foreach (var campo in errores.EnumerateObject())
                {
                    if (campo.Value.ValueKind != JsonValueKind.Array)
                        continue;

                    foreach (var item in campo.Value.EnumerateArray())
                        mensajes.Add(item.ToString());
                }

                if (mensajes.Count > 0)
                    return (false, string.Join(" ", mensajes));
            }
        }
        catch (JsonException)
        {
            // La API respondió con texto u otro formato.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}