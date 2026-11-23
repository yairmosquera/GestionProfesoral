using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontGestion.Servicios;

public class EstudioRealizadoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Universidad { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Tipo { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Docente { get; set; }
    public byte InsAcreditada { get; set; }
    public string Metodologia { get; set; } = "";
    public string PerfilEgresado { get; set; } = "";
    public string Pais { get; set; } = "";
    public bool Activo { get; set; } = true;
}

public class ServicioEstudioRealizado
{
    private readonly HttpClient _http;
    private readonly ServicioSesionJwt _sesion;

    public ServicioEstudioRealizado(HttpClient http, ServicioSesionJwt sesion)
    {
        _http = http;
        _sesion = sesion;
    }

    private HttpRequestMessage PrepararSolicitud(HttpMethod metodo, string ruta)
    {
        if (!_sesion.Autenticado || string.IsNullOrWhiteSpace(_sesion.Token))
            throw new UnauthorizedAccessException(
                "No se encontró una sesión JWT activa en el circuito de Blazor. " +
                "Verifica que ServicioSesionJwt esté registrado como Scoped en Program.cs.");

        var solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _sesion.Token);
        return solicitud;
    }

    private async Task<List<EstudioRealizadoDto>> ConsultarAsync(string ruta)
    {
        using var solicitud = PrepararSolicitud(HttpMethod.Get, ruta);
        using var respuesta = await _http.SendAsync(solicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "La API devolvió 401 aunque se envió el token JWT. " +
                "Comprueba la validación JWT de la API, las credenciales y que estés usando " +
                "la misma configuración de firma (JWT_SECRET, JWT_ISSUER y JWT_AUDIENCE). " +
                "No se requiere autorizar otra vez en Swagger.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException(
                "No tienes permiso para consultar los estudios.");

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content
            .ReadFromJsonAsync<List<EstudioRealizadoDto>>() ?? new();
    }

    public Task<List<EstudioRealizadoDto>> Listar()
        => ConsultarAsync("/api/estudios-realizados");

    public Task<List<EstudioRealizadoDto>> PorDocente(int cedula)
        => ConsultarAsync($"/api/estudios-realizados/docente/{cedula}");

    public async Task<(bool Ok, string Mensaje)> Guardar(
        EstudioRealizadoDto estudio, bool editando)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para guardar estudios.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para modificar estudios.");

        var ruta = editando
            ? $"/api/estudios-realizados/{estudio.Id}"
            : "/api/estudios-realizados";

        using var solicitud = PrepararSolicitud(
            editando ? HttpMethod.Put : HttpMethod.Post, ruta);

        if (editando)
        {
            solicitud.Content = JsonContent.Create(estudio);
        }
        else
        {
            // Mantener el contrato de creación sin Id.
            solicitud.Content = JsonContent.Create(new
            {
                estudio.Titulo,
                estudio.Universidad,
                estudio.Fecha,
                estudio.Tipo,
                estudio.Ciudad,
                estudio.Docente,
                estudio.InsAcreditada,
                estudio.Metodologia,
                estudio.PerfilEgresado,
                estudio.Pais
            });
        }

        using var respuesta = await _http.SendAsync(solicitud);
        return await ProcesarRespuesta(respuesta);
    }

    public async Task<(bool Ok, string Mensaje)> Eliminar(int id)
    {
        if (!_sesion.Autenticado)
            return (false, "Debes iniciar sesión para retirar estudios.");

        if (!_sesion.TieneRol("ADMIN", "GESTOR"))
            return (false, "No tienes permiso para retirar estudios.");

        using var solicitud = PrepararSolicitud(
            HttpMethod.Delete, $"/api/estudios-realizados/{id}");
        using var respuesta = await _http.SendAsync(solicitud);

        return await ProcesarRespuesta(respuesta);
    }

    private static async Task<(bool Ok, string Mensaje)>
        ProcesarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
            return (true, "Operación realizada correctamente.");

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return (false, "La API devolvió 401 pese a enviar el JWT. " +
                "Revisa su validación y la configuración de autenticación.");

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
            return (false, "No tienes permisos para esta operación.");

        try
        {
            var contenido = await respuesta.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(contenido);

            if (json.RootElement.TryGetProperty("mensaje", out var mensaje))
                return (false, mensaje.GetString()
                    ?? "No fue posible completar la operación.");

            if (json.RootElement.TryGetProperty("errors", out _))
                return (false, "Revisa los datos obligatorios del formulario.");
        }
        catch (JsonException)
        {
            // La API puede devolver un error sin JSON.
        }

        return (false, "No fue posible completar la operación.");
    }
}