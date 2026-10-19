
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioExperienciaDocente
    : IRepositorioExperienciaDocente
{
    private readonly string _cadenaConexion;

    public RepositorioExperienciaDocente(
        IConfiguration configuration)
    {
        _cadenaConexion =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion()
    {
        return new SqlConnection(_cadenaConexion);
    }

    public async Task<IEnumerable<ExperienciaDocente>> Listar()
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<ExperienciaDocente>(
            "dbo.sp_listar_experiencia_docente",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ExperienciaDocente?> Consultar(int id)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ExperienciaDocente>(
            "dbo.sp_consultar_experiencia_docente",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ExperienciaDocente>> PorDocente(
        int cedula)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<ExperienciaDocente>(
            "dbo.sp_experiencias_por_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ExperienciaDocente?> Crear(
        ExperienciaDocente experiencia)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ExperienciaDocente>(
            "dbo.sp_crear_experiencia_docente",
            new
            {
                experiencia.NombreCargo,
                experiencia.Institucion,
                experiencia.Tipo,
                experiencia.FechaInicio,
                experiencia.FechaFin,
                experiencia.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ExperienciaDocente?> Actualizar(
        ExperienciaDocente experiencia)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ExperienciaDocente>(
            "dbo.sp_actualizar_experiencia_docente",
            new
            {
                experiencia.Id,
                experiencia.NombreCargo,
                experiencia.Institucion,
                experiencia.Tipo,
                experiencia.FechaInicio,
                experiencia.FechaFin,
                experiencia.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int id)
    {
        using var conexion = CrearConexion();

        var filasAfectadas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_experiencia_docente",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        return filasAfectadas > 0;
    }
}
