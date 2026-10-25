using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioBeca : IRepositorioBeca
{
    private readonly string _cadenaConexion;

    public RepositorioBeca(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<IEnumerable<Beca>> Listar()
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<Beca>(
            "dbo.sp_listar_beca",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Beca?> Consultar(int estudios)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Beca>(
            "dbo.sp_consultar_beca",
            new { Estudios = estudios },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Beca?> Crear(Beca beca)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Beca>(
            "dbo.sp_crear_beca",
            Parametros(beca),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Beca?> Actualizar(Beca beca)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Beca>(
            "dbo.sp_actualizar_beca",
            Parametros(beca),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int estudios)
    {
        using var conexion = CrearConexion();
        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_beca",
            new { Estudios = estudios },
            commandType: CommandType.StoredProcedure);
        return filas > 0;
    }

    private static object Parametros(Beca beca) => new
    {
        beca.Estudios,
        beca.Tipo,
        beca.Institucion,
        beca.FechaInicio,
        beca.FechaFin
    };
}
