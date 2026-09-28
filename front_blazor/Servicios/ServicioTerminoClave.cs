using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class TerminoClaveDto
{
    public string Termino { get; set; } = string.Empty;

    public string? TerminoIngles { get; set; }

    public bool Activo { get; set; }
}

public class TerminoClaveCrearDto
{
    public string Termino { get; set; } = string.Empty;

    public string? TerminoIngles { get; set; }
}

public class TerminoClaveActualizarDto
{
    public string? TerminoIngles { get; set; }
}

public class ServicioTerminoClave
{
    private readonly HttpClient _http;

    public ServicioTerminoClave(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TerminoClaveDto>> ObtenerTodosAsync()
    {
        try
        {
            return await _http
                .GetFromJsonAsync<List<TerminoClaveDto>>(
                    "/api/terminos-clave"
                )
                ?? new List<TerminoClaveDto>();
        }
        catch
        {
            return new List<TerminoClaveDto>();
        }
    }

    public async Task<TerminoClaveDto?> ObtenerPorTerminoAsync(
        string termino)
    {
        try
        {
            var ruta =
                $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";

            var respuesta =
                await _http.GetAsync(ruta);

            if (!respuesta.IsSuccessStatusCode)
            {
                return null;
            }

            return await respuesta.Content
                .ReadFromJsonAsync<TerminoClaveDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Resultado<TerminoClaveDto>> CrearAsync(
        TerminoClaveCrearDto peticion)
    {
        try
        {
            var respuesta =
                await _http.PostAsJsonAsync(
                    "/api/terminos-clave",
                    peticion
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<TerminoClaveDto>.Mal(
                    $"No fue posible crear el término clave. Código: {(int)respuesta.StatusCode}"
                );
            }

            var termino =
                await respuesta.Content
                    .ReadFromJsonAsync<TerminoClaveDto>();

            return Resultado<TerminoClaveDto>.Bien(
                termino!
            );
        }
        catch
        {
            return Resultado<TerminoClaveDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<TerminoClaveDto>> ActualizarAsync(
        string termino,
        TerminoClaveActualizarDto peticion)
    {
        try
        {
            var ruta =
                $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";

            var respuesta =
                await _http.PutAsJsonAsync(
                    ruta,
                    peticion
                );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<TerminoClaveDto>.Mal(
                    $"No fue posible actualizar el término clave. Código: {(int)respuesta.StatusCode}"
                );
            }

            var terminoActualizado =
                await respuesta.Content
                    .ReadFromJsonAsync<TerminoClaveDto>();

            return Resultado<TerminoClaveDto>.Bien(
                terminoActualizado!
            );
        }
        catch
        {
            return Resultado<TerminoClaveDto>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(
        string termino)
    {
        try
        {
            var ruta =
                $"/api/terminos-clave/{Uri.EscapeDataString(termino)}";

            var respuesta =
                await _http.DeleteAsync(ruta);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<bool>.Mal(
                    $"No fue posible retirar el término clave. Código: {(int)respuesta.StatusCode}"
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