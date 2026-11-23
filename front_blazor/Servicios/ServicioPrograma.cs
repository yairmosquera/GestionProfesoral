using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class Programa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public string? FechaCierre { get; set; }
    public string NumeroCohortes { get; set; } = string.Empty;
    public string CantGraduados { get; set; } = string.Empty;
    public string FechaActualizacion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int Facultad { get; set; }
    public bool Activo { get; set; }
}

public record Resultado<T>(bool Ok, T? Datos, List<string> Errores)
{
    public static Resultado<T> Bien(T datos)
        => new(true, datos, new());

    public static Resultado<T> Mal(params string[] errores)
        => new(false, default, errores.ToList());
}

public class ServicioPrograma
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioPrograma(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    // Se envía el token del usuario actual en cada solicitud.
    // No se altera DefaultRequestHeaders.Authorization.
    private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
    {
        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private bool TieneSesion()
        => _sesion.Autenticado && !string.IsNullOrWhiteSpace(_sesion.Token);

    private static string MensajeError(HttpStatusCode estado) => estado switch
    {
        HttpStatusCode.Unauthorized =>
            "La API rechazó el token JWT (401). Inicia sesión nuevamente; si persiste, revisa la validación del token en la API.",
        HttpStatusCode.Forbidden =>
            "No tienes permisos para realizar esta operación (403).",
        HttpStatusCode.NotFound => "No se encontró el recurso solicitado (404).",
        _ => $"La operación no pudo completarse. Código HTTP: {(int)estado}."
    };

    public async Task<Resultado<List<Programa>>> Listar()
    {
        if (!TieneSesion())
            return Resultado<List<Programa>>.Mal("Debes iniciar sesión para consultar programas.");

        try
        {
            using var peticion = CrearSolicitud(HttpMethod.Get, "/api/programas");
            using var respuesta = await _http.SendAsync(peticion);
            if (!respuesta.IsSuccessStatusCode)
                return Resultado<List<Programa>>.Mal(MensajeError(respuesta.StatusCode));

            var datos = await respuesta.Content.ReadFromJsonAsync<List<Programa>>();
            return Resultado<List<Programa>>.Bien(datos ?? new());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            return Resultado<List<Programa>>.Mal($"No fue posible cargar los programas: {ex.Message}");
        }
    }

    public async Task<Resultado<Programa>> Obtener(int id)
    {
        if (!TieneSesion())
            return Resultado<Programa>.Mal("Debes iniciar sesión para consultar programas.");

        try
        {
            using var peticion = CrearSolicitud(HttpMethod.Get, $"/api/programas/{id}");
            using var respuesta = await _http.SendAsync(peticion);

            if (!respuesta.IsSuccessStatusCode)
                return Resultado<Programa>.Mal(MensajeError(respuesta.StatusCode));

            var programa = await respuesta.Content.ReadFromJsonAsync<Programa>();
            return programa is null
                ? Resultado<Programa>.Mal("La API no devolvió datos del programa.")
                : Resultado<Programa>.Bien(programa);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            return Resultado<Programa>.Mal($"No fue posible obtener el programa: {ex.Message}");
        }
    }

    public Task<Resultado<bool>> Crear(Programa programa)
        => Enviar(HttpMethod.Post, "/api/programas", programa);

    public Task<Resultado<bool>> Reemplazar(int id, Programa programa)
    {
        var cuerpo = new
        {
            nombre = programa.Nombre,
            tipo = programa.Tipo,
            nivel = programa.Nivel,
            fechaCreacion = programa.FechaCreacion,
            fechaCierre = programa.FechaCierre,
            numeroCohortes = programa.NumeroCohortes,
            cantGraduados = programa.CantGraduados,
            fechaActualizacion = programa.FechaActualizacion,
            ciudad = programa.Ciudad,
            facultad = programa.Facultad
        };
        return Enviar(HttpMethod.Put, $"/api/programas/{id}", cuerpo);
    }

    public Task<Resultado<bool>> Actualizar(int id, Dictionary<string, object?> cambios)
        => Enviar(HttpMethod.Patch, $"/api/programas/{id}", cambios);

    public Task<Resultado<bool>> Eliminar(int id)
        => Enviar(HttpMethod.Delete, $"/api/programas/{id}", null);

    private async Task<Resultado<bool>> Enviar(HttpMethod metodo, string ruta, object? cuerpo)
    {
        if (!TieneSesion())
            return Resultado<bool>.Mal("Debes iniciar sesión para modificar programas.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return Resultado<bool>.Mal("No tienes permiso para modificar programas.");

        try
        {
            using var peticion = CrearSolicitud(metodo, ruta);
            if (cuerpo is not null)
                peticion.Content = JsonContent.Create(cuerpo);

            using var respuesta = await _http.SendAsync(peticion);
            if (respuesta.IsSuccessStatusCode)
                return Resultado<bool>.Bien(true);

            return Resultado<bool>.Mal(MensajeError(respuesta.StatusCode));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return Resultado<bool>.Mal($"No fue posible comunicarse con la API: {ex.Message}");
        }
    }
}