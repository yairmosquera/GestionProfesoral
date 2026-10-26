using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class EstudioAcDto
{
    public int Estudio { get; set; }
    public string AreaConocimiento { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioEstudioAc
{
    private readonly HttpClient _http;

    public ServicioEstudioAc(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EstudioAcDto>> Listar()
        => await _http.GetFromJsonAsync<List<EstudioAcDto>>(
            "/api/estudios-ac") ?? new();

    public async Task<List<EstudioAcDto>> AreasPorEstudio(int estudio)
        => await _http.GetFromJsonAsync<List<EstudioAcDto>>(
            $"/api/estudios-ac/estudio/{estudio}") ?? new();

    public async Task<(bool Ok, string Mensaje)> Guardar(EstudioAcDto relacion)
    {
        using var respuesta = await _http.PostAsJsonAsync(
            "/api/estudios-ac",
            new { relacion.Estudio, relacion.AreaConocimiento });
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Área asociada al estudio correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(
        int estudio, string areaConocimiento)
    {
        using var respuesta = await _http.DeleteAsync(
            $"/api/estudios-ac/{estudio}/{Uri.EscapeDataString(areaConocimiento)}");
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Área retirada del estudio correctamente.");
    }
}
