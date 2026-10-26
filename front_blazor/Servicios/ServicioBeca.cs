using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class BecaDto
{
    public int Estudios { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;
}

public class ServicioBeca
{
    private readonly HttpClient _http;

    public ServicioBeca(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BecaDto>> Listar()
        => await _http.GetFromJsonAsync<List<BecaDto>>(
            "/api/becas") ?? new();

    public async Task<(bool Ok, string Mensaje)> Guardar(
        BecaDto beca, bool editando)
    {
        var peticion = new
        {
            beca.Estudios,
            beca.Tipo,
            beca.Institucion,
            beca.FechaInicio,
            beca.FechaFin
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync($"/api/becas/{beca.Estudios}", peticion)
            : await _http.PostAsJsonAsync("/api/becas", peticion);

        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta,
            editando ? "Beca actualizada correctamente." : "Beca registrada correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int estudios)
    {
        using var respuesta = await _http.DeleteAsync($"/api/becas/{estudios}");
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Beca retirada correctamente.");
    }
}
