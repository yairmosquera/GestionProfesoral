using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class InteresFuturoDto
{
    public int Docente { get; set; }
    public string TerminoClave { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioInteresFuturo
{
    private readonly HttpClient _http;

    public ServicioInteresFuturo(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<InteresFuturoDto>> Listar()
        => await _http.GetFromJsonAsync<List<InteresFuturoDto>>(
            "/api/intereses-futuros") ?? new();

    public async Task<List<InteresFuturoDto>> PorDocente(int docente)
        => await _http.GetFromJsonAsync<List<InteresFuturoDto>>(
            $"/api/intereses-futuros/docente/{docente}") ?? new();

    public async Task<(bool Ok, string Mensaje)> Guardar(
        InteresFuturoDto interes)
    {
        using var respuesta = await _http.PostAsJsonAsync(
            "/api/intereses-futuros",
            new { interes.Docente, interes.TerminoClave });
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Interés futuro registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int docente, string terminoClave)
    {
        using var respuesta = await _http.DeleteAsync(
            $"/api/intereses-futuros/{docente}/{Uri.EscapeDataString(terminoClave)}");
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Interés futuro retirado correctamente.");
    }
}
