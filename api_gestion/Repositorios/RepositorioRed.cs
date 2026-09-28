using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Implementación con Dapper del repositorio de Red.
/// </summary>
public class RepositorioRed : IRepositorioRed
{
    private readonly string _cadenaConexion;

    public RepositorioRed(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada."
            );
    }

    private IDbConnection CrearConexion() => new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Red>> ObtenerTodasAsync()
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                idr AS Idr,
                nombre AS Nombre,
                url AS Url,
                pais AS Pais,
                activo AS Activo
            FROM red
            WHERE activo = 1";

        return await db.QueryAsync<Red>(sql);
    }

    public async Task<Red?> ObtenerPorIdAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                idr AS Idr,
                nombre AS Nombre,
                url AS Url,
                pais AS Pais,
                activo AS Activo
            FROM red
            WHERE idr = @Idr AND activo = 1";

        return await db.QueryFirstOrDefaultAsync<Red>(
            sql,
            new { Idr = id }
        );
    }

    public async Task<int> CrearAsync(Red red)
    {
        using var db = CrearConexion();

        const string sql = @"
            INSERT INTO red
            (
                idr,
                nombre,
                url,
                pais,
                activo
            )
            VALUES
            (
                @Idr,
                @Nombre,
                @Url,
                @Pais,
                1
            );";

        await db.ExecuteAsync(sql, red);

        return red.Idr;
    }

    public async Task<bool> ActualizarAsync(Red red)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE red
            SET
                nombre = @Nombre,
                url = @Url,
                pais = @Pais
            WHERE idr = @Idr AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(sql, red);

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE red
            SET activo = 0
            WHERE idr = @Idr AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(
            sql,
            new { Idr = id }
        );

        return filasAfectadas > 0;
    }
}