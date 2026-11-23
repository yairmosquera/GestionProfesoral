using System.Security.Claims;
using ApiGestion.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApiGestion.Seguridad;

/// <summary>
/// Al validar la firma y vigencia de cada JWT, consulta nuevamente SQL Server.
/// Una cuenta desactivada o un rol retirado no conserva permisos por tener
/// un token emitido anteriormente.
/// </summary>
public static class ValidacionJwtVigente
{
    public static async Task ValidarAsync(TokenValidatedContext contexto)
    {
        var principal = contexto.Principal;
        var identidad = principal?.Identity as ClaimsIdentity;

        if (identidad?.IsAuthenticated != true)
        {
            contexto.Fail("No se pudo autenticar al usuario.");
            return;
        }

        // ServicioToken usa NameIdentifier y sub para el ID.
        string? valorId = principal!.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (!int.TryParse(valorId, out int id) || id <= 0)
        {
            contexto.Fail("El token no contiene un usuario válido.");
            return;
        }

        var repositorio = contexto.HttpContext.RequestServices
            .GetRequiredService<RepositorioUsuario>();

        var usuario = await repositorio.ConsultarPorIdAsync(id);

        if (usuario is null || !usuario.Activo)
        {
            contexto.Fail("La cuenta está desactivada o ya no existe.");
            return;
        }

        var rolesActuales = await repositorio.ConsultarRolesActivosAsync(id);

        if (rolesActuales.Count == 0)
        {
            contexto.Fail("El usuario no tiene roles activos.");
            return;
        }

        // Eliminar roles antiguos incorporados al token y sustituirlos por
        // los roles vigentes que devuelve la base de datos.
        var claimsSinRoles = identidad.Claims
            .Where(c => c.Type != ClaimTypes.Role &&
                        c.Type != "role" &&
                        c.Type != "roles")
            .ToList();

        var identidadActual = new ClaimsIdentity(
            claimsSinRoles,
            identidad.AuthenticationType,
            identidad.NameClaimType,
            ClaimTypes.Role);

        foreach (var rol in rolesActuales
                     .Where(r => !string.IsNullOrWhiteSpace(r))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            identidadActual.AddClaim(new Claim(ClaimTypes.Role, rol));
        }

        contexto.Principal = new ClaimsPrincipal(identidadActual);
    }
}