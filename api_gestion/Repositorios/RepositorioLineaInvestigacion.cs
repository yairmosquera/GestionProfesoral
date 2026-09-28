using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Implementación con Dapper del repositorio de Línea de Investigación.
/// </summary>
public class RepositorioLineaInvestigacion : IRepositorioLineaInvestigacion
{
    private readonly string _cadenaConexion;

    public RepositorioLineaInvestigacion(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada."
            );
    }

    private IDbConnection CrearConexion()
        => new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync()
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion,
                activo AS Activo
            FROM linea_investigacion
            WHERE activo = 1";

        return await db.QueryAsync<LineaInvestigacion>(sql);
    }

    public async Task<LineaInvestigacion?> ObtenerPorIdAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion,
                activo AS Activo
            FROM linea_investigacion
            WHERE id = @Id AND activo = 1";

        return await db.QueryFirstOrDefaultAsync<LineaInvestigacion>(
            sql,
            new { Id = id }
        );
    }

    public async Task<int> CrearAsync(LineaInvestigacion linea)
    {
        using var db = CrearConexion();

        const string sql = @"
            INSERT INTO linea_investigacion
            (
                nombre,
                descripcion,
                activo
            )
            VALUES
            (
                @Nombre,
                @Descripcion,
                1
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await db.ExecuteScalarAsync<int>(
            sql,
            linea
        );
    }

    public async Task<bool> ActualizarAsync(LineaInvestigacion linea)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE linea_investigacion
            SET
                nombre = @Nombre,
                descripcion = @Descripcion
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(
            sql,
            linea
        );

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE linea_investigacion
            SET activo = 0
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(
            sql,
            new { Id = id }
        );

        return filasAfectadas > 0;
    }
}