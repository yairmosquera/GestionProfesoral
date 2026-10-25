using System.Text.Json;

namespace FrontGestion.Servicios;

internal static class ServicioRespuestaApi
{
    public static async Task<(bool Ok, string Mensaje)> ProcesarAsync(
        HttpResponseMessage respuesta,
        string mensajeExito)
    {
        if (respuesta.IsSuccessStatusCode)
            return (true, mensajeExito);

        string contenido = await respuesta.Content.ReadAsStringAsync();
        try
        {
            using var json = JsonDocument.Parse(contenido);
            var raiz = json.RootElement;

            if (raiz.TryGetProperty("mensaje", out var mensaje) &&
                mensaje.ValueKind == JsonValueKind.String)
                return (false, mensaje.GetString() ?? "Ocurrió un error.");

            if (raiz.TryGetProperty("errors", out var errores) &&
                errores.ValueKind == JsonValueKind.Object)
            {
                var mensajes = errores.EnumerateObject()
                    .SelectMany(campo => campo.Value.ValueKind == JsonValueKind.Array
                        ? campo.Value.EnumerateArray()
                            .Where(item => item.ValueKind == JsonValueKind.String)
                            .Select(item => item.GetString())
                        : Enumerable.Empty<string?>())
                    .Where(item => !string.IsNullOrWhiteSpace(item));

                string texto = string.Join(" ", mensajes);
                if (!string.IsNullOrWhiteSpace(texto))
                    return (false, texto);
            }
        }
        catch (JsonException)
        {
            if (!string.IsNullOrWhiteSpace(contenido))
                return (false, contenido);
        }

        return (false, $"No se completó la operación. HTTP {(int)respuesta.StatusCode}.");
    }
}
