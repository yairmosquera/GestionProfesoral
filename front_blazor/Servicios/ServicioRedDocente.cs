
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

    public ServicioRedDocente(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RedDocenteDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<RedDocenteDto>>(
            "api/redes-docentes") ?? new();
    }

    public async Task<List<RedDocenteDto>> PorDocente(int cedula)
    {
        return await _http.GetFromJsonAsync<List<RedDocenteDto>>(
            $"api/redes-docentes/docente/{cedula}") ?? new();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        RedDocenteDto relacion, bool editando)
    {
        var peticion = new
        {
            relacion.Red,
            relacion.Docente,
            relacion.FechaInicio,
            relacion.FechaFin,
            relacion.ActDestacadas
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"api/redes-docentes/{relacion.Red}/{relacion.Docente}",
                peticion)
            : await _http.PostAsJsonAsync("api/redes-docentes", peticion);

        return await LeerResultado(
            respuesta,
            editando
                ? "Relación actualizada correctamente."
                : "Red asociada al docente correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int red, int docente)
    {
        using var respuesta = await _http.DeleteAsync(
            $"api/redes-docentes/{red}/{docente}");

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
            // La API respondió con texto u otro formato.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
