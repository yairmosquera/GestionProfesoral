using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class DocenteDepartamentoDto
{
    public int Docente { get; set; }
    public int Departamento { get; set; }
    public string Dedicacion { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; } = DateTime.Today;
    public DateTime? FechaSalida { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioDocenteDepartamento
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioDocenteDepartamento(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "Debes iniciar sesión para acceder a Departamentos de Docentes.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private async Task<List<DocenteDepartamentoDto>> ConsultarAsync(string ruta)
    {
        using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API rechazó la sesión (401). Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "Tu usuario no tiene permisos para consultar departamentos de docentes (403).");

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<List<DocenteDepartamentoDto>>()
            ?? new List<DocenteDepartamentoDto>();
    }

    public Task<List<DocenteDepartamentoDto>> Listar()
        => ConsultarAsync("api/docentes-departamentos");

    public Task<List<DocenteDepartamentoDto>> PorDocente(int cedula)
        => ConsultarAsync($"api/docentes-departamentos/docente/{cedula}");

    public async Task<(bool Ok, string Mensaje)> Guardar(
        DocenteDepartamentoDto relacion,
        bool editando)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para modificar departamentos de docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para modificar departamentos de docentes.");

        var peticion = new
        {
            relacion.Docente,
            relacion.Departamento,
            relacion.Dedicacion,
            relacion.Modalidad,
            relacion.FechaIngreso,
            relacion.FechaSalida
        };

        var ruta = editando
            ? $"api/docentes-departamentos/{relacion.Docente}/{relacion.Departamento}"
            : "api/docentes-departamentos";

        using var solicitud = CrearSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post, ruta);
        solicitud.Content = JsonContent.Create(peticion);

        using var respuesta = await _http.SendAsync(solicitud);
        return await LeerResultado(
            respuesta,
            editando
                ? "Asociación actualizada correctamente."
                : "Docente asociado al programa correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int docente,
        int departamento)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para retirar departamentos de docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permisos para retirar departamentos de docentes.");

        using var solicitud = CrearSolicitud(
            HttpMethod.Delete,
            $"api/docentes-departamentos/{docente}/{departamento}");
        using var respuesta = await _http.SendAsync(solicitud);
        return await LeerResultado(respuesta, "Asociación retirada correctamente.");
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
                        mensajes.Add(item.GetString() ?? "");
                }

                if (mensajes.Count > 0)
                    return (false, string.Join(" ", mensajes));
            }
        }
        catch (JsonException)
        {
            // Respuesta sin formato JSON.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}