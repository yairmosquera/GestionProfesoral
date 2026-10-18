
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

// Modelo que utiliza el frontend
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

// Servicio para consumir la API
public class ServicioDocente
{
    private readonly HttpClient _http;

    public ServicioDocente(HttpClient http)
    {
        _http = http;
    }

    // Listar docentes
    public async Task<List<DocenteDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<DocenteDto>>(
            "/api/docentes") ?? new();
    }

    // Crear o actualizar
    public async Task<(bool Ok, string Mensaje)> Guardar(
        DocenteDto docente, bool editando)
    {
        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"/api/docentes/{docente.Cedula}", docente)
            : await _http.PostAsJsonAsync(
                "/api/docentes", docente);

        return await ProcesarRespuesta(respuesta);
    }

    // Eliminar docente
    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int cedula)
    {
        using var respuesta = await _http.DeleteAsync(
            $"/api/docentes/{cedula}");

        return await ProcesarRespuesta(respuesta);
    }

    // Procesar respuesta de la API
    private static async Task<(bool Ok, string Mensaje)>
        ProcesarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            return (
                true,
                "Operación realizada correctamente."
            );
        }

        try
        {
            using var json = JsonDocument.Parse(
                await respuesta.Content.ReadAsStringAsync());

            if (json.RootElement.TryGetProperty(
                "mensaje", out var mensaje))
            {
                return (
                    false,
                    mensaje.GetString()
                    ?? "No se pudo completar la operación."
                );
            }
        }
        catch (JsonException)
        {
            // Manejar otros formatos de error
        }

        return (
            false,
            "No se pudo completar la operación. Revise los datos."
        );
    }
}
