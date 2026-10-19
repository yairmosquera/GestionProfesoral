
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioEvaluacionDocente
    : IRepositorioEvaluacionDocente
{
    private readonly string _conexion;

    public RepositorioEvaluacionDocente(
        IConfiguration configuracion)
    {
        _conexion = configuracion
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No está configurada la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion()
    {
        return new SqlConnection(_conexion);
    }

    // =========================================
    // LISTAR EVALUACIONES
    // =========================================

    public async Task<IEnumerable<EvaluacionDocente>>
        ObtenerTodosAsync()
    {
        using var db = CrearConexion();

        return await db.QueryAsync<EvaluacionDocente>(
            "dbo.sp_listar_evaluacion_docente",
            commandType: CommandType.StoredProcedure);
    }

    // =========================================
    // CONSULTAR EVALUACIÓN POR ID
    // =========================================

    public async Task<EvaluacionDocente?> ObtenerPorIdAsync(
        int id)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<EvaluacionDocente>(
            "dbo.sp_consultar_evaluacion_docente",
            new
            {
                Id = id
            },
            commandType: CommandType.StoredProcedure);
    }

    // =========================================
    // CONSULTAR EVALUACIONES POR DOCENTE
    // =========================================

    public async Task<IEnumerable<EvaluacionDocente>>
        ObtenerPorDocenteAsync(int cedula)
    {
        using var db = CrearConexion();

        return await db.QueryAsync<EvaluacionDocente>(
            "dbo.sp_evaluaciones_por_docente",
            new
            {
                Cedula = cedula
            },
            commandType: CommandType.StoredProcedure);
    }

    // =========================================
    // CREAR EVALUACIÓN
    // ID GENERADO AUTOMÁTICAMENTE POR SQL SERVER
    // =========================================

    public async Task<EvaluacionDocente> CrearAsync(
        EvaluacionDocente evaluacion)
    {
        using var db = CrearConexion();

        return await db.QuerySingleAsync<EvaluacionDocente>(
            "dbo.sp_crear_evaluacion_docente",
            new
            {
                evaluacion.Calificacion,
                evaluacion.Semestre,
                evaluacion.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    // =========================================
    // ACTUALIZAR EVALUACIÓN
    // =========================================

    public async Task<EvaluacionDocente?> ActualizarAsync(
        EvaluacionDocente evaluacion)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<EvaluacionDocente>(
            "dbo.sp_actualizar_evaluacion_docente",
            new
            {
                evaluacion.Id,
                evaluacion.Calificacion,
                evaluacion.Semestre,
                evaluacion.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    // =========================================
    // RETIRO LÓGICO
    // =========================================

    public async Task<bool> EliminarAsync(int id)
    {
        using var db = CrearConexion();

        var filas = await db.ExecuteScalarAsync<int>(
            "dbo.sp_eliminar_evaluacion_docente",
            new
            {
                Id = id
            },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }
}
