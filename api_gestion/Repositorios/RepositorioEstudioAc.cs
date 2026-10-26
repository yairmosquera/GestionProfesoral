using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioEstudioAc : IRepositorioEstudioAc
{
    private readonly string _cadenaConexion;

    public RepositorioEstudioAc(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<IEnumerable<EstudioAc>> Listar()
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<EstudioAc>(
            "dbo.sp_listar_estudio_ac",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<EstudioAc?> Consultar(int estudio, string areaConocimiento)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<EstudioAc>(
            "dbo.sp_consultar_estudio_ac",
            new { Estudio = estudio, AreaConocimiento = areaConocimiento },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<EstudioAc>> AreasPorEstudio(int estudio)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<EstudioAc>(
            "dbo.sp_areas_por_estudio",
            new { Estudio = estudio },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<EstudioAc?> Crear(EstudioAc relacion)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<EstudioAc>(
            "dbo.sp_crear_estudio_ac",
            new { relacion.Estudio, relacion.AreaConocimiento },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<EstudioAc?> Actualizar(
        int estudioOriginal,
        string areaConocimientoOriginal,
        EstudioAc relacion)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<EstudioAc>(
            "dbo.sp_actualizar_estudio_ac",
            new
            {
                Estudio = estudioOriginal,
                AreaConocimiento = areaConocimientoOriginal,
                NuevoEstudio = relacion.Estudio,
                NuevaAreaConocimiento = relacion.AreaConocimiento
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int estudio, string areaConocimiento)
    {
        using var conexion = CrearConexion();
        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_estudio_ac",
            new { Estudio = estudio, AreaConocimiento = areaConocimiento },
            commandType: CommandType.StoredProcedure);
        return filas > 0;
    }
}
