
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioDocente : IRepositorioDocente
{
    private readonly string _conexion;

    public RepositorioDocente(IConfiguration configuracion)
    {
        _conexion = configuracion
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No está configurada la conexión.");
    }

    private SqlConnection CrearConexion()
        => new SqlConnection(_conexion);

    public async Task<IEnumerable<Docente>> ObtenerTodosAsync()
    {
        using var db = CrearConexion();

        return await db.QueryAsync<Docente>(
            "dbo.sp_listar_docente",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Docente?> ObtenerPorIdAsync(int cedula)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<Docente>(
            "dbo.sp_consultar_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Docente> CrearAsync(Docente docente)
    {
        using var db = CrearConexion();

        return await db.QuerySingleAsync<Docente>(
            "dbo.sp_crear_docente",
            Parametros(docente),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Docente?> ActualizarAsync(Docente docente)
    {
        using var db = CrearConexion();

        return await db.QueryFirstOrDefaultAsync<Docente>(
            "dbo.sp_actualizar_docente",
            Parametros(docente),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> EliminarAsync(int cedula)
    {
        using var db = CrearConexion();

        var filas = await db.ExecuteScalarAsync<int>(
            "dbo.sp_eliminar_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }

    private static object Parametros(Docente d) => new
    {
        d.Cedula,
        d.Nombres,
        d.Apellidos,
        d.Genero,
        d.Cargo,
        d.FechaNacimiento,
        d.Correo,
        d.Telefono,
        d.UrlCvlac,
        d.Escalafon,
        d.Perfil,
        d.CatMinciencia,
        d.ConvMinciencia,
        d.Nacionalidaad,
        d.LineaInvestigacionPrincipal
    };
}
