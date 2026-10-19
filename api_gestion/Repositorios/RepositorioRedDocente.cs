
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioRedDocente : IRepositorioRedDocente
{
    private readonly string _cadenaConexion;

    public RepositorioRedDocente(IConfiguration configuration)
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

    public async Task<IEnumerable<RedDocente>> Listar()
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<RedDocente>(
            "dbo.sp_listar_red_docente",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<RedDocente?> Consultar(
        int red, int docente)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<RedDocente>(
            "dbo.sp_consultar_red_docente",
            new { Red = red, Docente = docente },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RedDocente>> PorDocente(
        int cedula)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<RedDocente>(
            "dbo.sp_redes_por_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<RedDocente?> Crear(
        RedDocente relacion)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<RedDocente>(
            "dbo.sp_crear_red_docente",
            new
            {
                relacion.Red,
                relacion.Docente,
                relacion.FechaInicio,
                relacion.FechaFin,
                relacion.ActDestacadas
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<RedDocente?> Actualizar(
        RedDocente relacion)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<RedDocente>(
            "dbo.sp_actualizar_red_docente",
            new
            {
                relacion.Red,
                relacion.Docente,
                relacion.FechaInicio,
                relacion.FechaFin,
                relacion.ActDestacadas
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(
        int red, int docente)
    {
        using var conexion = CrearConexion();

        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_red_docente",
            new { Red = red, Docente = docente },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }
}
