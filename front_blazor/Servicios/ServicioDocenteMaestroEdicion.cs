using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

// Versión 3: edición de docente maestro-detalle con JWT por solicitud.
public class ServicioDocenteMaestroEdicion
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioDocenteMaestroEdicion(
        HttpClient http,
        ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    public async Task<(bool Ok, string Mensaje)> Actualizar(
        int cedula, object datos)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            return (false, "Debes iniciar sesión para editar docentes.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para editar docentes.");

        using var solicitud = new HttpRequestMessage(
            HttpMethod.Put, $"/api/docentes-maestro/{cedula}");

        // Se añade el token por solicitud para no compartirlo
        // accidentalmente entre usuarios/circuitos Blazor.
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        solicitud.Content = JsonContent.Create(datos);

        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.IsSuccessStatusCode)
            return (true, "Docente y detalles actualizados correctamente.");

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "Tu sesión expiró o no es válida. Inicia sesión nuevamente.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permiso para editar este docente.");

        var cuerpo = await respuesta.Content.ReadAsStringAsync();
        try
        {
            using var documento = JsonDocument.Parse(cuerpo);
            var raiz = documento.RootElement;

            if (raiz.TryGetProperty("mensaje", out var mensaje))
                return (false, mensaje.GetString()
                    ?? "No fue posible actualizar.");

            if (raiz.TryGetProperty("errors", out var errores) &&
                errores.ValueKind == JsonValueKind.Object)
            {
                var mensajes = errores.EnumerateObject()
                    .SelectMany(campo =>
                        campo.Value.ValueKind == JsonValueKind.Array
                            ? campo.Value.EnumerateArray().Select(x => x.ToString())
                            : Enumerable.Empty<string>())
                    .ToList();

                if (mensajes.Count > 0)
                    return (false, string.Join(" | ", mensajes));
            }
        }
        catch (JsonException)
        {
            // La respuesta no es JSON.
        }

        return (false, $"Error HTTP {(int)respuesta.StatusCode} al actualizar el docente.");
    }
}