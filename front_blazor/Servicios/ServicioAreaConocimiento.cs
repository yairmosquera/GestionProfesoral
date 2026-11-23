using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class AreaConocimiento
{
    public string Id { get; set; } = string.Empty;
    public string GranArea { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class ServicioAreaConocimiento
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioAreaConocimiento(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // El token pertenece a la sesión actual de Blazor Server.
    // Se asigna por solicitud, nunca a encabezados globales.
    private HttpRequestMessage Preparar(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException("Debes iniciar sesión para acceder a Áreas de Conocimiento.");

        var peticion = new HttpRequestMessage(metodo, ruta);
        peticion.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return peticion;
    }

    private static string MensajeError(HttpStatusCode estado) => estado switch
    {
        HttpStatusCode.Unauthorized => "La API rechazó la autenticación (401). Inicia sesión nuevamente y verifica la configuración JWT.",
        HttpStatusCode.Forbidden => "Tu usuario no tiene permisos para esta operación (403).",
        HttpStatusCode.NotFound => "No se encontró el registro solicitado (404).",
        _ => $"La API respondió con HTTP {(int)estado}."
    };

    public async Task<Resultado<List<AreaConocimiento>>> Listar()
    {
        try
        {
            using var peticion = Preparar(HttpMethod.Get, "/api/areas-conocimiento");
            using var respuesta = await _http.SendAsync(peticion);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<List<AreaConocimiento>>.Mal(MensajeError(respuesta.StatusCode));

            var datos = await respuesta.Content.ReadFromJsonAsync<List<AreaConocimiento>>();
            return Resultado<List<AreaConocimiento>>.Bien(datos ?? new());
        }
        catch (UnauthorizedAccessException ex)
        {
            return Resultado<List<AreaConocimiento>>.Mal(ex.Message);
        }
        catch (Exception ex)
        {
            return Resultado<List<AreaConocimiento>>.Mal($"No fue posible cargar las áreas de conocimiento: {ex.Message}");
        }
    }

    public async Task<Resultado<AreaConocimiento>> Obtener(string id)
    {
        try
        {
            using var peticion = Preparar(HttpMethod.Get,
                $"/api/areas-conocimiento/{Uri.EscapeDataString(id)}");
            using var respuesta = await _http.SendAsync(peticion);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<AreaConocimiento>.Mal(MensajeError(respuesta.StatusCode));

            var area = await respuesta.Content.ReadFromJsonAsync<AreaConocimiento>();
            return area is null
                ? Resultado<AreaConocimiento>.Mal("La API devolvió un área vacía.")
                : Resultado<AreaConocimiento>.Bien(area);
        }
        catch (Exception ex)
        {
            return Resultado<AreaConocimiento>.Mal($"No fue posible obtener el área de conocimiento: {ex.Message}");
        }
    }

    public Task<Resultado<bool>> Crear(AreaConocimiento area)
    {
        var cuerpo = new
        {
            id = area.Id,
            granArea = area.GranArea,
            area = area.Area,
            disciplina = area.Disciplina
        };
        return Enviar(HttpMethod.Post, "/api/areas-conocimiento", cuerpo);
    }

    public Task<Resultado<bool>> Reemplazar(string id, AreaConocimiento area)
    {
        var cuerpo = new
        {
            granArea = area.GranArea,
            area = area.Area,
            disciplina = area.Disciplina
        };
        return Enviar(HttpMethod.Put,
            $"/api/areas-conocimiento/{Uri.EscapeDataString(id)}", cuerpo);
    }

    public Task<Resultado<bool>> Actualizar(
        string id, Dictionary<string, object?> cambios)
    {
        return Enviar(HttpMethod.Patch,
            $"/api/areas-conocimiento/{Uri.EscapeDataString(id)}", cambios);
    }

    public Task<Resultado<bool>> Eliminar(string id)
    {
        return Enviar(HttpMethod.Delete,
            $"/api/areas-conocimiento/{Uri.EscapeDataString(id)}", null);
    }

    private async Task<Resultado<bool>> Enviar(
        HttpMethod metodo, string ruta, object? cuerpo)
    {
        if (!_sesion.Autenticado)
            return Resultado<bool>.Mal("Debes iniciar sesión para modificar áreas de conocimiento.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<bool>.Mal("No tienes permiso para modificar áreas de conocimiento.");

        try
        {
            using var peticion = Preparar(metodo, ruta);
            if (cuerpo is not null)
                peticion.Content = JsonContent.Create(cuerpo);

            using var respuesta = await _http.SendAsync(peticion);
            return respuesta.IsSuccessStatusCode
                ? Resultado<bool>.Bien(true)
                : Resultado<bool>.Mal(MensajeError(respuesta.StatusCode));
        }
        catch (Exception ex)
        {
            return Resultado<bool>.Mal($"No fue posible comunicarse con la API: {ex.Message}");
        }
    }
}