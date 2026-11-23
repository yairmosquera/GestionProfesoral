using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Acceso a la administración de usuarios de la versión 3.
/// IMPORTANTE: invocar desde endpoints protegidos con el rol ADMIN.
/// </summary>
public sealed class RepositorioAdministracionUsuarios
{
    private readonly string _cadenaConexion;

    public RepositorioAdministracionUsuarios(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No está configurada la conexión DefaultConnection.");
    }

    public async Task<IReadOnlyList<UsuarioAdministracionDto>> ListarUsuariosAsync()
    {
        await using var conexion = new SqlConnection(_cadenaConexion);
        var usuarios = await conexion.QueryAsync<UsuarioAdministracionDto>(
            "dbo.sp_usuarios_listar",
            commandType: CommandType.StoredProcedure);

        return usuarios.ToList();
    }

    public async Task<IReadOnlyList<RolAdministracionDto>> ListarRolesAsync()
    {
        await using var conexion = new SqlConnection(_cadenaConexion);
        var roles = await conexion.QueryAsync<RolAdministracionDto>(
            "dbo.sp_roles_listar",
            commandType: CommandType.StoredProcedure);

        return roles.ToList();
    }

    public async Task AsignarRolAsync(int usuarioId, string rolCodigo)
    {
        ValidarUsuarioYRol(usuarioId, rolCodigo);

        await using var conexion = new SqlConnection(_cadenaConexion);
        await conexion.ExecuteAsync(
            "dbo.sp_usuario_asignar_rol",
            new { UsuarioId = usuarioId, RolCodigo = rolCodigo.Trim() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task RetirarRolAsync(int usuarioId, string rolCodigo)
    {
        ValidarUsuarioYRol(usuarioId, rolCodigo);

        await using var conexion = new SqlConnection(_cadenaConexion);
        await conexion.ExecuteAsync(
            "dbo.sp_usuario_retirar_rol",
            new { UsuarioId = usuarioId, RolCodigo = rolCodigo.Trim() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task CambiarEstadoAsync(int usuarioId, bool activo)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(nameof(usuarioId));

        await using var conexion = new SqlConnection(_cadenaConexion);
        await conexion.ExecuteAsync(
            "dbo.sp_usuario_cambiar_estado",
            new { UsuarioId = usuarioId, Activo = activo },
            commandType: CommandType.StoredProcedure);
    }

    private static void ValidarUsuarioYRol(int usuarioId, string rolCodigo)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(nameof(usuarioId));

        ArgumentException.ThrowIfNullOrWhiteSpace(rolCodigo);

        if (rolCodigo.Length > 30)
            throw new ArgumentException(
                "El código de rol no puede superar los 30 caracteres.",
                nameof(rolCodigo));
    }
}

/// <summary>
/// DTO de consulta. No contiene PasswordHash.
/// </summary>
public sealed class UsuarioAdministracionDto
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}

public sealed class RolAdministracionDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}