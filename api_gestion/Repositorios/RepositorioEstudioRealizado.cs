
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioEstudioRealizado
    : IRepositorioEstudioRealizado
{
    private readonly string _conexion;

    public RepositorioEstudioRealizado(
        IConfiguration configuracion)
    {
        _conexion = configuracion
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No está configurada la conexión.");
    }

    private SqlConnection CrearConexion()
    {
        return new SqlConnection(_conexion);
    }

    // LISTAR
    public async Task<IEnumerable<EstudioRealizado>>
        ObtenerTodosAsync()
    {
        using var db = CrearConexion();

        return await db.QueryAsync<EstudioRealizado>(
            "dbo.sp_listar_estudios_realizados",
            commandType: CommandType.StoredProcedure);
    }

    // CONSULTAR POR ID
    public async Task<EstudioRealizado?> ObtenerPorIdAsync(
        int id)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<EstudioRealizado>(
            "dbo.sp_consultar_estudios_realizados",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    // LISTAR POR DOCENTE
    public async Task<IEnumerable<EstudioRealizado>>
        ObtenerPorDocenteAsync(int cedula)
    {
        using var db = CrearConexion();

        return await db.QueryAsync<EstudioRealizado>(
            "dbo.sp_estudios_por_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    // CREAR - ID GENERADO POR SQL SERVER
    public async Task<EstudioRealizado> CrearAsync(
        EstudioRealizado estudio)
    {
        using var db = CrearConexion();

        return await db.QuerySingleAsync<EstudioRealizado>(
            "dbo.sp_crear_estudios_realizados",
            new
            {
                estudio.Titulo,
                estudio.Universidad,
                estudio.Fecha,
                estudio.Tipo,
                estudio.Ciudad,
                estudio.Docente,
                estudio.InsAcreditada,
                estudio.Metodologia,
                estudio.PerfilEgresado,
                estudio.Pais
            },
            commandType: CommandType.StoredProcedure);
    }

    // ACTUALIZAR - ID OBLIGATORIO
    public async Task<EstudioRealizado?> ActualizarAsync(
        EstudioRealizado estudio)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<EstudioRealizado>(
            "dbo.sp_actualizar_estudios_realizados",
            new
            {
                estudio.Id,
                estudio.Titulo,
                estudio.Universidad,
                estudio.Fecha,
                estudio.Tipo,
                estudio.Ciudad,
                estudio.Docente,
                estudio.InsAcreditada,
                estudio.Metodologia,
                estudio.PerfilEgresado,
                estudio.Pais
            },
            commandType: CommandType.StoredProcedure);
    }

    // RETIRO LÓGICO
    public async Task<bool> EliminarAsync(int id)
    {
        using var db = CrearConexion();

        var filas = await db.ExecuteScalarAsync<int>(
            "dbo.sp_eliminar_estudios_realizados",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }
}
