using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioInteresesFuturos : IRepositorioInteresesFuturos
{
    private readonly string _cadenaConexion;

    public RepositorioInteresesFuturos(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<IEnumerable<InteresFuturo>> Listar()
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<InteresFuturo>(
            "dbo.sp_listar_intereses_futuros",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<InteresFuturo?> Consultar(int docente, string terminoClave)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<InteresFuturo>(
            "dbo.sp_consultar_intereses_futuros",
            new { Docente = docente, TerminoClave = terminoClave },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<InteresFuturo>> PorDocente(int docente)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryAsync<InteresFuturo>(
            "dbo.sp_intereses_por_docente",
            new { Docente = docente },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<InteresFuturo?> Crear(InteresFuturo interes)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<InteresFuturo>(
            "dbo.sp_crear_intereses_futuros",
            new { interes.Docente, interes.TerminoClave },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<InteresFuturo?> Actualizar(
        int docenteOriginal,
        string terminoClaveOriginal,
        InteresFuturo interes)
    {
        using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<InteresFuturo>(
            "dbo.sp_actualizar_intereses_futuros",
            new
            {
                Docente = docenteOriginal,
                TerminoClave = terminoClaveOriginal,
                NuevoDocente = interes.Docente,
                NuevoTerminoClave = interes.TerminoClave
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(int docente, string terminoClave)
    {
        using var conexion = CrearConexion();
        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_intereses_futuros",
            new { Docente = docente, TerminoClave = terminoClave },
            commandType: CommandType.StoredProcedure);
        return filas > 0;
    }
}
