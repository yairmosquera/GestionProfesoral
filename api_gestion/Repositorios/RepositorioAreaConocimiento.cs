using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Implementación con Dapper del repositorio de Área de Conocimiento.
/// </summary>
public class RepositorioAreaConocimiento : IRepositorioAreaConocimiento
{
    private readonly string _cadenaConexion;

    public RepositorioAreaConocimiento(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada."
            );
    }

    private IDbConnection CrearConexion() => new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<AreaConocimiento>> ObtenerTodasAsync()
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                gran_area AS GranArea,
                area AS Area,
                disciplina AS Disciplina,
                activo AS Activo
            FROM area_conocimiento
            WHERE activo = 1";

        return await db.QueryAsync<AreaConocimiento>(sql);
    }

    public async Task<AreaConocimiento?> ObtenerPorIdAsync(string id)
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                gran_area AS GranArea,
                area AS Area,
                disciplina AS Disciplina,
                activo AS Activo
            FROM area_conocimiento
            WHERE id = @Id AND activo = 1";

        return await db.QueryFirstOrDefaultAsync<AreaConocimiento>(
            sql,
            new { Id = id }
        );
    }

    public async Task<string> CrearAsync(AreaConocimiento areaConocimiento)
    {
        using var db = CrearConexion();

        const string sql = @"
            INSERT INTO area_conocimiento
            (
                id,
                gran_area,
                area,
                disciplina,
                activo
            )
            VALUES
            (
                @Id,
                @GranArea,
                @Area,
                @Disciplina,
                1
            );";

        await db.ExecuteAsync(sql, areaConocimiento);

        return areaConocimiento.Id;
    }

    public async Task<bool> ActualizarAsync(AreaConocimiento areaConocimiento)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE area_conocimiento
            SET
                gran_area = @GranArea,
                area = @Area,
                disciplina = @Disciplina
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(sql, areaConocimiento);

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(string id)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE area_conocimiento
            SET activo = 0
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(
            sql,
            new { Id = id }
        );

        return filasAfectadas > 0;
    }
}