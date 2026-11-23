using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApiGestion.Modelos;
using Microsoft.IdentityModel.Tokens;

namespace ApiGestion.Servicios;

/// <summary>
/// Genera JWT firmados. La clave de firma debe mantenerse en una variable
/// de entorno del servidor y nunca enviarse al frontend ni subirse a Git.
/// </summary>
public sealed class ServicioToken
{
    private readonly byte[] _clave;
    private readonly string _emisor;
    private readonly string _audiencia;
    private readonly int _duracionMinutos;

    public ServicioToken(IConfiguration configuracion)
    {
        string secreto = configuracion["JWT_SECRET"]
            ?? throw new InvalidOperationException(
                "Falta configurar JWT_SECRET en el servidor.");

        _clave = Encoding.UTF8.GetBytes(secreto);
        if (_clave.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET debe tener por lo menos 32 bytes UTF-8.");
        }

        _emisor = configuracion["JWT_ISSUER"] ?? "GestionProfesoralAPI";
        _audiencia = configuracion["JWT_AUDIENCE"] ?? "GestionProfesoralBlazor";

        if (string.IsNullOrWhiteSpace(_emisor) ||
            string.IsNullOrWhiteSpace(_audiencia))
        {
            throw new InvalidOperationException(
                "JWT_ISSUER y JWT_AUDIENCE deben tener valores válidos.");
        }

        if (!int.TryParse(configuracion["JWT_MINUTES"], out var minutos))
            minutos = 60;

        if (minutos < 5 || minutos > 240)
        {
            throw new InvalidOperationException(
                "JWT_MINUTES debe estar entre 5 y 240.");
        }

        _duracionMinutos = minutos;
    }

    /// <summary>
    /// Crea un token solo para un usuario activo y roles previamente verificados
    /// en la base de datos. No incluir contraseñas ni hashes en los claims.
    /// </summary>
    public (string Token, DateTime ExpiraUtc) Generar(
        Usuario usuario,
        IEnumerable<string> roles)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(roles);

        if (usuario.Id <= 0 || !usuario.Activo)
            throw new InvalidOperationException("El usuario no está habilitado.");

        var ahora = DateTime.UtcNow;
        var expiracion = ahora.AddMinutes(_duracionMinutos);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var rol in roles.Where(r => !string.IsNullOrWhiteSpace(r))
                                 .Select(r => r.Trim())
                                 .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(_clave),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _emisor,
            audience: _audiencia,
            claims: claims,
            notBefore: ahora,
            expires: expiracion,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiracion);
    }
}
