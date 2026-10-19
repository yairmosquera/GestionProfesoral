
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class EvaluacionDocenteDto
{
    public int Id { get; set; }
    public float Calificacion { get; set; }
    public string Semestre { get; set; } = "";
    public int Docente { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioEvaluacionDocente
{
    private readonly HttpClient _http;

    public ServicioEvaluacionDocente(HttpClient http)
    {
        _http = http;
    }

    // =========================================
    // LISTAR TODAS LAS EVALUACIONES
    // =========================================

    public async Task<List<EvaluacionDocenteDto>> Listar()
    {
        return await _http.GetFromJsonAsync<List<EvaluacionDocenteDto>>(
            "api/evaluaciones-docentes") ?? new();
    }

    // =========================================
    // CONSULTAR EVALUACIONES POR DOCENTE
    // =========================================

    public async Task<List<EvaluacionDocenteDto>> PorDocente(
        int cedula)
    {
        return await _http.GetFromJsonAsync<List<EvaluacionDocenteDto>>(
            $"api/evaluaciones-docentes/docente/{cedula}") ?? new();
    }

    // =========================================
    // REGISTRAR / ACTUALIZAR
    // =========================================

    public async Task<(bool Ok, string Mensaje)> Guardar(
        EvaluacionDocenteDto evaluacion,
        bool editando)
    {
        HttpResponseMessage respuesta;

        var peticion = new
        {
            evaluacion.Calificacion,
            evaluacion.Semestre,
            evaluacion.Docente
        };

        if (editando)
        {
            respuesta = await _http.PutAsJsonAsync(
                $"api/evaluaciones-docentes/{evaluacion.Id}",
                peticion);
        }
        else
        {
            respuesta = await _http.PostAsJsonAsync(
                "api/evaluaciones-docentes",
                peticion);
        }

        using (respuesta)
        {
            return await LeerResultado(respuesta);
        }
    }

    // =========================================
    // RETIRO LÓGICO
    // =========================================

    public async Task<(bool Ok, string Mensaje)> Eliminar(int id)
    {
        using var respuesta = await _http.DeleteAsync(
            $"api/evaluaciones-docentes/{id}");

        return await LeerResultado(respuesta);
    }

    // =========================================
    // MENSAJES DE LA API
    // =========================================

    private static async Task<(bool Ok, string Mensaje)>
        LeerResultado(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            return (true, "Operación realizada correctamente.");
        }

        var contenido = await respuesta.Content.ReadAsStringAsync();

        try
        {
            using var documento = JsonDocument.Parse(contenido);

            if (documento.RootElement.TryGetProperty(
                "mensaje", out var mensaje))
            {
                return (
                    false,
                    mensaje.GetString()
                        ?? "No fue posible completar la operación."
                );
            }

            if (documento.RootElement.TryGetProperty(
                "errors", out var errores))
            {
                return (
                    false,
                    "Revisa los campos obligatorios y sus valores."
                );
            }
        }
        catch (JsonException)
        {
            // La respuesta no contiene JSON.
        }

        return (
            false,
            $"No fue posible completar la operación. HTTP {(int)respuesta.StatusCode}."
        );
    }
}
