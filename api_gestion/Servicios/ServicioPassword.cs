
using ApiGestion.Modelos;
using Microsoft.AspNetCore.Identity;

namespace ApiGestion.Servicios;

/// <summary>
/// Protege y verifica contraseñas con ASP.NET Core Identity.
/// Nunca almacena ni registra contraseñas en texto plano.
/// </summary>
public sealed class ServicioPassword
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string GenerarHash(Usuario usuario, string password)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "La contraseña es obligatoria.", nameof(password));
        }

        return _hasher.HashPassword(usuario, password);
    }

    public bool Verificar(Usuario usuario, string password)
    {
        return VerificarConRehash(usuario, password)
            != PasswordVerificationResult.Failed;
    }

    public PasswordVerificationResult VerificarConRehash(
        Usuario usuario,
        string password)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        if (string.IsNullOrEmpty(password) ||
            string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            return PasswordVerificationResult.Failed;
        }

        return _hasher.VerifyHashedPassword(
            usuario, usuario.PasswordHash, password);
    }
}
