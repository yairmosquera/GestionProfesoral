using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class ApoyoProfesoralDto
{
    public int Estudios { get; set; }
    public byte ConApoyo { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class ServicioApoyoProfesoral
{
    private readonly HttpClient _http;

    public ServicioApoyoProfesoral(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ApoyoProfesoralDto>> Listar()
        => await _http.GetFromJsonAsync<List<ApoyoProfesoralDto>>(
            "/api/apoyos-profesorales") ?? new();

    public async Task<(bool Ok, string Mensaje)> Guardar(
        ApoyoProfesoralDto apoyo, bool editando)
    {
        var peticion = new
        {
            apoyo.Estudios,
            apoyo.ConApoyo,
            apoyo.Institucion,
            apoyo.Tipo
        };

        using var respuesta = editando
            ? await _http.PutAsJsonAsync(
                $"/api/apoyos-profesorales/{apoyo.Estudios}", peticion)
            : await _http.PostAsJsonAsync("/api/apoyos-profesorales", peticion);

        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta,
            editando
                ? "Apoyo profesoral actualizado correctamente."
                : "Apoyo profesoral registrado correctamente.");
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int estudios)
    {
        using var respuesta = await _http.DeleteAsync(
            $"/api/apoyos-profesorales/{estudios}");
        return await ServicioRespuestaApi.ProcesarAsync(
            respuesta, "Apoyo profesoral retirado correctamente.");
    }
}
