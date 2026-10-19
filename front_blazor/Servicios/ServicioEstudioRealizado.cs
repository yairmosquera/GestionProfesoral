
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class EstudioRealizadoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Universidad { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Tipo { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Docente { get; set; }
    public byte InsAcreditada { get; set; }
    public string Metodologia { get; set; } = "";
    public string PerfilEgresado { get; set; } = "";
    public string Pais { get; set; } = "";
    public bool Activo { get; set; } = true;
}

public class ServicioEstudioRealizado
{
    private readonly HttpClient _http;

    public ServicioEstudioRealizado(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EstudioRealizadoDto>> Listar()
    {
        return await _http
            .GetFromJsonAsync<List<EstudioRealizadoDto>>(
                "/api/estudios-realizados"
            ) ?? new();
    }

    public async Task<List<EstudioRealizadoDto>> PorDocente(
        int cedula)
    {
        return await _http
            .GetFromJsonAsync<List<EstudioRealizadoDto>>(
                $"/api/estudios-realizados/docente/{cedula}"
            ) ?? new();
    }

    public async Task<(bool Ok, string Mensaje)> Guardar(
        EstudioRealizadoDto estudio,
        bool editando)
    {
        HttpResponseMessage respuesta;

        if (editando)
        {
            respuesta = await _http.PutAsJsonAsync(
                $"/api/estudios-realizados/{estudio.Id}",
                estudio);
        }
        else
        {
            // La creación no envía el ID.
            respuesta = await _http.PostAsJsonAsync(
                "/api/estudios-realizados",
                new
                {
                    estudio.Titulo,
                    estudio.Universidad,
                    estudio.Fecha,
                    estudio.Tipo,
                    estudio.Ciudad,
                    estudio.Docente,
                    estudio.InsAcreditada,
                    estudio.Metodologia,
                    estudio.PerfilEgresado,
                    estudio.Pais
                });
        }

        using (respuesta)
        {
            return await ProcesarRespuesta(respuesta);
        }
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int id)
    {
        using var respuesta = await _http.DeleteAsync(
            $"/api/estudios-realizados/{id}");

        return await ProcesarRespuesta(respuesta);
    }

    private static async Task<(bool Ok, string Mensaje)>
        ProcesarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            return (true, "Operación realizada correctamente.");
        }

        try
        {
            var contenido =
                await respuesta.Content.ReadAsStringAsync();

            using var json = JsonDocument.Parse(contenido);

            if (json.RootElement.TryGetProperty(
                "mensaje", out var mensaje))
            {
                return (
                    false,
                    mensaje.GetString()
                        ?? "No fue posible completar la operación."
                );
            }

            if (json.RootElement.TryGetProperty(
                "errors", out var errores))
            {
                return (
                    false,
                    "Revisa los datos obligatorios del formulario."
                );
            }
        }
        catch (JsonException)
        {
            // La respuesta puede tener otro formato.
        }

        return (
            false,
            "No fue posible completar la operación."
        );
    }
}
