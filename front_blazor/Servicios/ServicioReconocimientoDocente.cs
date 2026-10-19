
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class ReconocimientoDocenteDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Institucion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Ambito { get; set; } = string.Empty;
    public int Docente { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioReconocimientoDocente
{
    private readonly HttpClient _http;

    public ServicioReconocimientoDocente(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ReconocimientoDocenteDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<ReconocimientoDocenteDto>>(
            "api/reconocimientos-docentes") ?? new();
    }

    public async Task<List<ReconocimientoDocenteDto>> PorDocente(int cedula)
    {
        return await _http.GetFromJsonAsync<List<ReconocimientoDocenteDto>>(
            $"api/reconocimientos-docentes/docente/{cedula}") ?? new();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        ReconocimientoDocenteDto reconocimiento, bool editando)
    {
        var peticion = new
        {
            reconocimiento.Tipo,
            reconocimiento.Fecha,
            reconocimiento.Institucion,
            reconocimiento.Nombre,
            reconocimiento.Ambito,
            reconocimiento.Docente
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"api/reconocimientos-docentes/{reconocimiento.Id}", peticion)
            : await _http.PostAsJsonAsync(
                "api/reconocimientos-docentes", peticion);

        return await LeerResultado(
            respuesta,
            editando
                ? "Reconocimiento actualizado correctamente."
                : "Reconocimiento registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int id)
    {
        using var respuesta = await _http.DeleteAsync(
            $"api/reconocimientos-docentes/{id}");

        return await LeerResultado(
            respuesta,
            "Reconocimiento retirado correctamente.");
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
            // Respuesta no JSON.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
