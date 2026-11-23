using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FrontGestion.Servicios;

/// <summary>
/// Mantiene la sesión JWT únicamente en memoria del circuito Blazor Server.
/// No guarda contraseñas ni tokens en localStorage o en archivos.
/// </summary>
public sealed class ServicioSesionJwt
{
    private readonly HttpClient _http;

    public ServicioSesionJwt(HttpClient http)
    {
        _http = http;
    }

    public string? Token { get; private set; }
    public int? UsuarioId { get; private set; }
    public string? NombreUsuario { get; private set; }
    public string? NombreCompleto { get; private set; }
    public IReadOnlyList<string> Roles { get; private set; } =
        Array.Empty<string>();
    public DateTime? ExpiraUtc { get; private set; }

    public bool Autenticado =>
        !string.IsNullOrWhiteSpace(Token) &&
        ExpiraUtc.HasValue &&
        DateTime.UtcNow < ExpiraUtc.Value.ToUniversalTime();

    public bool TieneRol(params string[] roles) =>
        Autenticado && roles.Any(rol =>
            Roles.Contains(rol, StringComparer.OrdinalIgnoreCase));

    public event Action? CambioDeSesion;

    public async Task<(bool Ok, string Mensaje)> IniciarSesionAsync(
        string nombreUsuario,
        string contrasena)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            return (false, "Escribe el usuario y la contraseña.");
        }

        try
        {
            using var respuesta = await _http.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    nombreUsuario = nombreUsuario.Trim(),
                    contrasena
                });

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                CerrarSesion();
                return (false, "Usuario o contraseña incorrectos.");
            }

            if (!respuesta.IsSuccessStatusCode)
            {
                return (false,
                    "No fue posible iniciar sesión. Intenta nuevamente.");
            }

            var datos = await respuesta.Content
                .ReadFromJsonAsync<RespuestaInicioSesion>();

            if (datos is null ||
                string.IsNullOrWhiteSpace(datos.Token) ||
                datos.ExpiraUtc.ToUniversalTime() <= DateTime.UtcNow)
            {
                return (false, "La API devolvió una sesión inválida.");
            }

            Token = datos.Token;
            UsuarioId = datos.Id;
            NombreUsuario = datos.NombreUsuario;
            NombreCompleto = datos.NombreCompleto;
            Roles = datos.Roles ?? Array.Empty<string>();
            ExpiraUtc = datos.ExpiraUtc.ToUniversalTime();

            CambioDeSesion?.Invoke();
            return (true, "Sesión iniciada correctamente.");
        }
        catch (HttpRequestException)
        {
            return (false,
                "No fue posible conectar con la API de Gestión Profesoral.");
        }
        catch (System.Text.Json.JsonException)
        {
            return (false, "La API devolvió una respuesta inválida.");
        }
    }

    public void CerrarSesion()
    {
        Token = null;
        UsuarioId = null;
        NombreUsuario = null;
        NombreCompleto = null;
        Roles = Array.Empty<string>();
        ExpiraUtc = null;
        CambioDeSesion?.Invoke();
    }

    private sealed class RespuestaInicioSesion
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expiraUtc")]
        public DateTime ExpiraUtc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombreUsuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [JsonPropertyName("nombreCompleto")]
        public string NombreCompleto { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public string[]? Roles { get; set; }
    }
}