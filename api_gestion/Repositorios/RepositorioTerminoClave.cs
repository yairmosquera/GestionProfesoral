using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

/// <summary>
/// Implementación con Dapper del repositorio de Término Clave.
/// </summary>
public class RepositorioTerminoClave : IRepositorioTerminoClave
{
    private readonly string _cadenaConexion;

    public RepositorioTerminoClave(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
    }

    private IDbConnection CrearConexion() => new SqlConnection(_cadenaConexion);

    public async Task<IEnumerable<TerminoClave>> ObtenerTodosAsync()
    {
        using var db = CrearConexion();
        const string sql = @"
            SELECT termino AS Termino, termino_ingles AS TerminoIngles, activo AS Activo
            FROM termino_clave
            WHERE activo = 1";
        return await db.QueryAsync<TerminoClave>(sql);
    }

    public async Task<TerminoClave?> ObtenerPorTerminoAsync(string termino)
    {
        using var db = CrearConexion();
        const string sql = @"
            SELECT termino AS Termino, termino_ingles AS TerminoIngles, activo AS Activo
            FROM termino_clave
            WHERE termino = @Termino AND activo = 1";
        return await db.QueryFirstOrDefaultAsync<TerminoClave>(sql, new { Termino = termino });
    }

    public async Task<bool> CrearAsync(TerminoClave terminoClave)
    {
        using var db = CrearConexion();
        const string sql = @"
            INSERT INTO termino_clave (termino, termino_ingles, activo)
            VALUES (@Termino, @TerminoIngles, 1)";
        var filasAfectadas = await db.ExecuteAsync(sql, terminoClave);
        return filasAfectadas > 0;
    }

    public async Task<bool> ActualizarAsync(TerminoClave terminoClave)
    {
        using var db = CrearConexion();
        const string sql = @"
            UPDATE termino_clave
            SET termino_ingles = @TerminoIngles
            WHERE termino = @Termino AND activo = 1";
        var filasAfectadas = await db.ExecuteAsync(sql, terminoClave);
        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(string termino)
    {
        using var db = CrearConexion();
        const string sql = @"
            UPDATE termino_clave
            SET activo = 0
            WHERE termino = @Termino AND activo = 1";
        var filasAfectadas = await db.ExecuteAsync(sql, new { Termino = termino });
        return filasAfectadas > 0;
    }
}