
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

    public ServicioExperienciaDocente(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ExperienciaDocenteDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<ExperienciaDocenteDto>>(
            "api/experiencias-docentes") ?? new();
    }

    public async Task<List<ExperienciaDocenteDto>> PorDocente(
        int cedula)
    {
        return await _http.GetFromJsonAsync<List<ExperienciaDocenteDto>>(
            $"api/experiencias-docentes/docente/{cedula}") ?? new();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        ExperienciaDocenteDto experiencia,
        bool editando)
    {
        var peticion = new
        {
            experiencia.NombreCargo,
            experiencia.Institucion,
            experiencia.Tipo,
            experiencia.FechaInicio,
            experiencia.FechaFin,
            experiencia.Docente
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"api/experiencias-docentes/{experiencia.Id}",
                peticion)
            : await _http.PostAsJsonAsync(
                "api/experiencias-docentes",
                peticion);

        return await LeerResultado(
            respuesta,
            editando
                ? "Experiencia actualizada correctamente."
                : "Experiencia registrada correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int id)
    {
        using var respuesta = await _http.DeleteAsync(
            $"api/experiencias-docentes/{id}");

        return await LeerResultado(
            respuesta,
            "Experiencia retirada correctamente.");
    }

    private static async Task<(bool Ok, string Mensaje)> LeerResultado(
        HttpResponseMessage respuesta,
        string mensajeExito)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            return (true, mensajeExito);
        }

        var contenido = await respuesta.Content.ReadAsStringAsync();

        try
        {
            using var documento = JsonDocument.Parse(contenido);

            var raiz = documento.RootElement;

            if (raiz.TryGetProperty("mensaje", out var mensaje))
            {
                return (false, mensaje.GetString() ?? "Ocurrió un error.");
            }

            if (raiz.TryGetProperty("errors", out var errores) &&
                errores.ValueKind == JsonValueKind.Object)
            {
                var mensajes = new List<string>();

                foreach (var propiedad in errores.EnumerateObject())
                {
                    if (propiedad.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var valor in propiedad.Value.EnumerateArray())
                        {
                            mensajes.Add(valor.GetString() ?? "");
                        }
                    }
                }

                if (mensajes.Count > 0)
                {
                    return (false, string.Join(" ", mensajes));
                }
            }
        }
        catch (JsonException)
        {
            // La respuesta no tiene formato JSON.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
