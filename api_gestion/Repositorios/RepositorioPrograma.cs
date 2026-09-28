using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Implementación con Dapper del repositorio de Programa.
/// </summary>
public class RepositorioPrograma : IRepositorioPrograma
{
    private readonly string _cadenaConexion;

    public RepositorioPrograma(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada."
            );
    }

    private IDbConnection CrearConexion() => new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Programa>> ObtenerTodasAsync()
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                nombre AS Nombre,
                tipo AS Tipo,
                nivel AS Nivel,
                fecha_creacion AS FechaCreacion,
                fecha_cierre AS FechaCierre,
                numero_cohortes AS NumeroCohortes,
                cant_graduados AS CantGraduados,
                fecha_actualizacion AS FechaActualizacion,
                ciudad AS Ciudad,
                facultad AS Facultad,
                activo AS Activo
            FROM programa
            WHERE activo = 1";

        return await db.QueryAsync<Programa>(sql);
    }

    public async Task<Programa?> ObtenerPorIdAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            SELECT
                id AS Id,
                nombre AS Nombre,
                tipo AS Tipo,
                nivel AS Nivel,
                fecha_creacion AS FechaCreacion,
                fecha_cierre AS FechaCierre,
                numero_cohortes AS NumeroCohortes,
                cant_graduados AS CantGraduados,
                fecha_actualizacion AS FechaActualizacion,
                ciudad AS Ciudad,
                facultad AS Facultad,
                activo AS Activo
            FROM programa
            WHERE id = @Id AND activo = 1";

        return await db.QueryFirstOrDefaultAsync<Programa>(
            sql,
            new { Id = id }
        );
    }

    public async Task<int> CrearAsync(Programa programa)
    {
        using var db = CrearConexion();

        const string sql = @"
            INSERT INTO programa
            (
                id,
                nombre,
                tipo,
                nivel,
                fecha_creacion,
                fecha_cierre,
                numero_cohortes,
                cant_graduados,
                fecha_actualizacion,
                ciudad,
                facultad,
                activo
            )
            VALUES
            (
                @Id,
                @Nombre,
                @Tipo,
                @Nivel,
                @FechaCreacion,
                @FechaCierre,
                @NumeroCohortes,
                @CantGraduados,
                @FechaActualizacion,
                @Ciudad,
                @Facultad,
                1
            );";

        await db.ExecuteAsync(sql, programa);

        return programa.Id;
    }

    public async Task<bool> ActualizarAsync(Programa programa)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE programa
            SET
                nombre = @Nombre,
                tipo = @Tipo,
                nivel = @Nivel,
                fecha_creacion = @FechaCreacion,
                fecha_cierre = @FechaCierre,
                numero_cohortes = @NumeroCohortes,
                cant_graduados = @CantGraduados,
                fecha_actualizacion = @FechaActualizacion,
                ciudad = @Ciudad,
                facultad = @Facultad
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(sql, programa);

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var db = CrearConexion();

        const string sql = @"
            UPDATE programa
            SET activo = 0
            WHERE id = @Id AND activo = 1";

        var filasAfectadas = await db.ExecuteAsync(
            sql,
            new { Id = id }
        );

        return filasAfectadas > 0;
    }
}