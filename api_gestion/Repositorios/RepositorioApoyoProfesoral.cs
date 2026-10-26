using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioApoyoProfesoral : IRepositorioApoyoProfesoral
{
    private readonly string _cadenaConexion;

    public RepositorioApoyoProfesoral(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<IEnumerable<ApoyoProfesoral>> Listar()
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<ApoyoProfesoral>(
            "dbo.sp_listar_apoyo_profesoral",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ApoyoProfesoral?> Consultar(int estudios)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<ApoyoProfesoral>(
            "dbo.sp_consultar_apoyo_profesoral",
            new { Estudios = estudios },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ApoyoProfesoral?> Crear(ApoyoProfesoral apoyo)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<ApoyoProfesoral>(
            "dbo.sp_crear_apoyo_profesoral",
            Parametros(apoyo),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<ApoyoProfesoral?> Actualizar(ApoyoProfesoral apoyo)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<ApoyoProfesoral>(
            "dbo.sp_actualizar_apoyo_profesoral",
            Parametros(apoyo),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int estudios)
    {
        using var conexion = CrearConexion();
        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_apoyo_profesoral",
            new { Estudios = estudios },
            commandType: CommandType.StoredProcedure);
        return filas > 0;
    }

    private static object Parametros(ApoyoProfesoral apoyo) => new
    {
        apoyo.Estudios,
        apoyo.ConApoyo,
        apoyo.Institucion,
        apoyo.Tipo
    };
}
