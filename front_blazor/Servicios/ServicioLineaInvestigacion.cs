using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class LineaInvestigacionDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; }
}

public class LineaInvestigacionCrearDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}

public class LineaInvestigacionActualizarDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}

public class ServicioLineaInvestigacion
{
    private readonly HttpClient _http;

    public ServicioLineaInvestigacion(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<LineaInvestigacionDto>> ObtenerTodosAsync()
    {
        try
        {
            return await _http
                .GetFromJsonAsync<List<LineaInvestigacionDto>>(
                    "/api/lineas-investigacion"
                )
                ?? new List<LineaInvestigacionDto>();
        }
        catch
        {
            return new List<LineaInvestigacionDto>();
        }
    }

    public async Task<LineaInvestigacionDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var respuesta = await _http.GetAsync(
                $"/api/lineas-investigacion/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return null;
            }

            return await respuesta.Content
                .ReadFromJsonAsync<LineaInvestigacionDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Resultado<LineaInvestigacionDto>> CrearAsync(
        LineaInvestigacionCrearDto peticion)
    {
        try
        {
            var respuesta = await _http.PostAsJsonAsync(
                "/api/lineas-investigacion",
                peticion
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<LineaInvestigacionDto>.Mal(
                    $"No fue posible crear la línea de investigación. Código: {(int)respuesta.StatusCode}"
                );
            }

            var linea = await respuesta.Content
                .ReadFromJsonAsync<LineaInvestigacionDto>();

            return Resultado<LineaInvestigacionDto>.Bien(linea!);
        }
        catch
        {
            return Resultado<LineaInvestigacionDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<LineaInvestigacionDto>> ActualizarAsync(
        int id,
        LineaInvestigacionActualizarDto peticion)
    {
        try
        {
            var respuesta = await _http.PutAsJsonAsync(
                $"/api/lineas-investigacion/{id}",
                peticion
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<LineaInvestigacionDto>.Mal(
                    $"No fue posible actualizar la línea de investigación. Código: {(int)respuesta.StatusCode}"
                );
            }

            var linea = await respuesta.Content
                .ReadFromJsonAsync<LineaInvestigacionDto>();

            return Resultado<LineaInvestigacionDto>.Bien(linea!);
        }
        catch
        {
            return Resultado<LineaInvestigacionDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        try
        {
            var respuesta = await _http.DeleteAsync(
                $"/api/lineas-investigacion/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<bool>.Mal(
                    $"No fue posible retirar la línea de investigación. Código: {(int)respuesta.StatusCode}"
                );
            }

            return Resultado<bool>.Bien(true);
        }
        catch
        {
            return Resultado<bool>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }
}