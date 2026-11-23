
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Acceso interno a usuarios de la versión 3.
/// PasswordHash nunca debe exponerse directamente desde un controlador.
/// </summary>
public sealed class RepositorioUsuario
{
    private readonly string _cadenaConexion;

    public RepositorioUsuario(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No está configurada la conexión DefaultConnection.");
    }

    public async Task<Usuario?> ConsultarPorNombreAsync(string nombreUsuario)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreUsuario);

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QueryFirstOrDefaultAsync<Usuario>(
            "dbo.sp_usuario_consultar_por_nombre",
            new { NombreUsuario = nombreUsuario },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Usuario?> ConsultarPorIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        await using var conexion = new SqlConnection(_cadenaConexion);

        return await conexion.QueryFirstOrDefaultAsync<Usuario>(
            "dbo.sp_usuario_consultar_por_id",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Solo debe llamarse desde un servicio de administración autorizado.
    /// passwordHash debe generarse con PasswordHasher antes de invocar este método.
    /// </summary>
    public async Task<Usuario> CrearAsync(
        string nombreUsuario,
        string correo,
        string nombreCompleto,
        string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreUsuario);
        ArgumentException.ThrowIfNullOrWhiteSpace(correo);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreCompleto);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        await using var conexion = new SqlConnection(_cadenaConexion);

        var usuario = await conexion.QuerySingleAsync<Usuario>(
            "dbo.sp_usuario_crear",
            new
            {
                NombreUsuario = nombreUsuario,
                Correo = correo,
                NombreCompleto = nombreCompleto,
                PasswordHash = passwordHash
            },
            commandType: CommandType.StoredProcedure);

        return usuario;
    }

    public async Task RegistrarAccesoAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        await using var conexion = new SqlConnection(_cadenaConexion);

        await conexion.ExecuteAsync(
            "dbo.sp_usuario_registrar_acceso",
            new { UsuarioId = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IReadOnlyList<string>> ConsultarRolesActivosAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        await using var conexion = new SqlConnection(_cadenaConexion);

        var roles = await conexion.QueryAsync<string>(
            "dbo.sp_usuario_roles_activos",
            new { UsuarioId = id },
            commandType: CommandType.StoredProcedure);

        return roles.ToList();
    }
}
