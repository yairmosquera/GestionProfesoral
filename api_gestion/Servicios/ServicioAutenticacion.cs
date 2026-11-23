using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Identity;

namespace ApiGestion.Servicios;

public sealed class ServicioAutenticacion
{
    private readonly RepositorioUsuario _usuarios;
    private readonly ServicioPassword _password;
    private readonly ServicioToken _tokens;

    public ServicioAutenticacion(
        RepositorioUsuario usuarios,
        ServicioPassword password,
        ServicioToken tokens)
    {
        _usuarios = usuarios;
        _password = password;
        _tokens = tokens;
    }

    public async Task<ResultadoInicioSesion?> IniciarSesionAsync(
        string nombreUsuario,
        string contrasena)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrEmpty(contrasena))
            return null;

        var usuario = await _usuarios.ConsultarPorNombreAsync(
            nombreUsuario.Trim());

        if (usuario is null || !usuario.Activo)
            return null;

        PasswordVerificationResult verificacion;
        try
        {
            verificacion = _password.VerificarConRehash(
                usuario, contrasena);
        }
        catch (FormatException)
        {
            return null; // Hash inválido o incompatible.
        }

        if (verificacion == PasswordVerificationResult.Failed)
            return null;

        var roles = await _usuarios.ConsultarRolesActivosAsync(usuario.Id);
        var (token, expiraUtc) = _tokens.Generar(usuario, roles);

        await _usuarios.RegistrarAccesoAsync(usuario.Id);

        // Si se indica SuccessRehashNeeded, una futura operación de
        // actualización autenticada deberá reemplazar el hash almacenado.
        return new ResultadoInicioSesion(
            token,
            expiraUtc,
            usuario.Id,
            usuario.NombreUsuario,
            usuario.NombreCompleto,
            roles);
    }
}

public sealed record ResultadoInicioSesion(
    string Token,
    DateTime ExpiraUtc,
    int Id,
    string NombreUsuario,
    string NombreCompleto,
    IReadOnlyList<string> Roles);
