
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

    public ServicioDocenteDepartamento(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<DocenteDepartamentoDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<DocenteDepartamentoDto>>(
            "api/docentes-departamentos") ?? new();
    }

    public async Task<List<DocenteDepartamentoDto>> PorDocente(int cedula)
    {
        return await _http.GetFromJsonAsync<List<DocenteDepartamentoDto>>(
            $"api/docentes-departamentos/docente/{cedula}") ?? new();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        DocenteDepartamentoDto relacion,
        bool editando)
    {
        var peticion = new
        {
            relacion.Docente,
            relacion.Departamento,
            relacion.Dedicacion,
            relacion.Modalidad,
            relacion.FechaIngreso,
            relacion.FechaSalida
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"api/docentes-departamentos/{relacion.Docente}/{relacion.Departamento}",
                peticion)
            : await _http.PostAsJsonAsync(
                "api/docentes-departamentos",
                peticion);

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
        using var respuesta = await _http.DeleteAsync(
            $"api/docentes-departamentos/{docente}/{departamento}");

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
            // Respuesta sin formato JSON.
        }

        return (false,
            $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
