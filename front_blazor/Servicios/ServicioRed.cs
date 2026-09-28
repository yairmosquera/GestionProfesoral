using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class RedDto
{
    public int Idr { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;

    public bool Activo { get; set; }
}

public class RedCrearDto
{
    public int Idr { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;
}

public class RedActualizarDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;
}

public class ServicioRed
{
    private readonly HttpClient _http;

    public ServicioRed(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RedDto>> ObtenerTodosAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<RedDto>>(
                "/api/redes"
            ) ?? new List<RedDto>();
        }
        catch
        {
            return new List<RedDto>();
        }
    }

    public async Task<RedDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var respuesta = await _http.GetAsync(
                $"/api/redes/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return null;
            }

            return await respuesta.Content
                .ReadFromJsonAsync<RedDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Resultado<RedDto>> CrearAsync(
        RedCrearDto peticion)
    {
        try
        {
            var respuesta = await _http.PostAsJsonAsync(
                "/api/redes",
                peticion
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<RedDto>.Mal(
                    $"No fue posible crear la red. Código: {(int)respuesta.StatusCode}"
                );
            }

            var red = await respuesta.Content
                .ReadFromJsonAsync<RedDto>();

            return Resultado<RedDto>.Bien(red!);
        }
        catch
        {
            return Resultado<RedDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<RedDto>> ActualizarAsync(
        int id,
        RedActualizarDto peticion)
    {
        try
        {
            var respuesta = await _http.PutAsJsonAsync(
                $"/api/redes/{id}",
                peticion
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<RedDto>.Mal(
                    $"No fue posible actualizar la red. Código: {(int)respuesta.StatusCode}"
                );
            }

            var red = await respuesta.Content
                .ReadFromJsonAsync<RedDto>();

            return Resultado<RedDto>.Bien(red!);
        }
        catch
        {
            return Resultado<RedDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(int id)
    {
        try
        {
            var respuesta = await _http.DeleteAsync(
                $"/api/redes/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<bool>.Mal(
                    $"No fue posible retirar la red. Código: {(int)respuesta.StatusCode}"
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