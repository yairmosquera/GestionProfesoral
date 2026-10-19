
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioReconocimientoDocente
    : IRepositorioReconocimientoDocente
{
    private readonly string _cadenaConexion;

    public RepositorioReconocimientoDocente(
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

    public async Task<IEnumerable<ReconocimientoDocente>> Listar()
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<ReconocimientoDocente>(
            "dbo.sp_listar_reconocimiento",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ReconocimientoDocente?> Consultar(int id)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ReconocimientoDocente>(
            "dbo.sp_consultar_reconocimiento",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ReconocimientoDocente>> PorDocente(
        int cedula)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<ReconocimientoDocente>(
            "dbo.sp_reconocimientos_por_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ReconocimientoDocente?> Crear(
        ReconocimientoDocente reconocimiento)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ReconocimientoDocente>(
            "dbo.sp_crear_reconocimiento",
            new
            {
                reconocimiento.Tipo,
                reconocimiento.Fecha,
                reconocimiento.Institucion,
                reconocimiento.Nombre,
                reconocimiento.Ambito,
                reconocimiento.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ReconocimientoDocente?> Actualizar(
        ReconocimientoDocente reconocimiento)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<ReconocimientoDocente>(
            "dbo.sp_actualizar_reconocimiento",
            new
            {
                reconocimiento.Id,
                reconocimiento.Tipo,
                reconocimiento.Fecha,
                reconocimiento.Institucion,
                reconocimiento.Nombre,
                reconocimiento.Ambito,
                reconocimiento.Docente
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int id)
    {
        using var conexion = CrearConexion();

        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_reconocimiento",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }
}
