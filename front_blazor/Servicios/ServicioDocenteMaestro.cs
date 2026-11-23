using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

/// <summary>
/// Registra un docente con sus estudios, evaluaciones, experiencias y
/// reconocimientos mediante la API maestro-detalle.
/// </summary>
public sealed class ServicioDocenteMaestro
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioDocenteMaestro(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // Mantiene la firma que utiliza DocentesMaestro.razor:
    // ServicioMaestro.Registrar(formulario)
    public async Task<(bool Ok, string Mensaje)> Registrar(object formulario)
    {
        if (formulario is null)
            return (false, "Falta la información del docente.");

        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para registrar docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para registrar docentes.");

        using var solicitud = new HttpRequestMessage(
            HttpMethod.Post, "/api/docentes-maestro");

        // No usar DefaultRequestHeaders.Authorization: cada solicitud
        // lleva exclusivamente el token del usuario actual.
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        solicitud.Content = JsonContent.Create(formulario);

        try
        {
            using var respuesta = await _http.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
                return (true, "Docente y detalles registrados correctamente.");

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                return (false, "Tu sesión expiró o no es válida. Inicia sesión nuevamente.");

            if (respuesta.StatusCode == HttpStatusCode.Forbidden)
                return (false, "No tienes permiso para registrar docentes.");

            var contenido = await respuesta.Content.ReadAsStringAsync();

            try
            {
                using var json = JsonDocument.Parse(contenido);
                var raiz = json.RootElement;

                if (raiz.TryGetProperty("mensaje", out var mensaje) &&
                    mensaje.ValueKind == JsonValueKind.String)
                {
                    return (false, mensaje.GetString()
                        ?? "No se pudo registrar el docente.");
                }

                if (raiz.TryGetProperty("errors", out var errores) &&
                    errores.ValueKind == JsonValueKind.Object)
                {
                    var detalles = errores.EnumerateObject()
                        .SelectMany(campo =>
                            campo.Value.ValueKind == JsonValueKind.Array
                                ? campo.Value.EnumerateArray().Select(x => x.ToString())
                                : Enumerable.Empty<string>())
                        .ToList();

                    if (detalles.Count > 0)
                        return (false, string.Join(" | ", detalles));
                }
            }
            catch (JsonException)
            {
                // La API devolvió una respuesta no JSON.
            }

            return (false, $"No se pudo registrar el docente (HTTP {(int)respuesta.StatusCode}).");
        }
        catch (HttpRequestException)
        {
            return (false, "No se pudo conectar con la API de Gestión Profesoral.");
        }
        catch (TaskCanceledException)
        {
            return (false, "La solicitud tardó demasiado. Verifica si el docente fue registrado antes de reintentar.");
        }
    }
}


// Modelos utilizados por DocentesMaestro.razor.
// Se conservan los nombres y los tipos compatibles con sus formularios.
public sealed class DocenteMaestroDto
{
    public DocenteDto Docente { get; set; } = new();
    public List<EstudioMaestroDto> Estudios { get; set; } = new();
    public List<EvaluacionMaestroDto> Evaluaciones { get; set; } = new();
    public List<ExperienciaMaestroDto> Experiencias { get; set; } = new();
    public List<ReconocimientoMaestroDto> Reconocimientos { get; set; } = new();
}

public sealed class EstudioMaestroDto
{
    public string Titulo { get; set; } = "";
    public string Universidad { get; set; } = "";
    public DateTime? Fecha { get; set; }
    public string Tipo { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public byte InsAcreditada { get; set; }
    public string Metodologia { get; set; } = "";
    public string PerfilEgresado { get; set; } = "";
    public string Pais { get; set; } = "";
}

public sealed class EvaluacionMaestroDto
{
    public float Calificacion { get; set; }
    public string Semestre { get; set; } = "";
}

public sealed class ExperienciaMaestroDto
{
    public string NombreCargo { get; set; } = "";
    public string Institucion { get; set; } = "";
    public string Tipo { get; set; } = "";
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

public sealed class ReconocimientoMaestroDto
{
    public string Tipo { get; set; } = "";
    public DateTime? Fecha { get; set; }
    public string Institucion { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Ambito { get; set; } = "";
}