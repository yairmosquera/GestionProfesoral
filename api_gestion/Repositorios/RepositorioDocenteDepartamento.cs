
using System.Data;
using ApiGestion.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioDocenteDepartamento
    : IRepositorioDocenteDepartamento
{
    private readonly string _cadenaConexion;

    public RepositorioDocenteDepartamento(
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

    public async Task<IEnumerable<DocenteDepartamento>> Listar()
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<DocenteDepartamento>(
            "dbo.sp_listar_docente_departamento",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<DocenteDepartamento?> Consultar(
        int docente, int departamento)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>(
            "dbo.sp_consultar_docente_departamento",
            new
            {
                Docente = docente,
                Departamento = departamento
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DocenteDepartamento>> PorDocente(
        int cedula)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryAsync<DocenteDepartamento>(
            "dbo.sp_departamentos_por_docente",
            new { Cedula = cedula },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<DocenteDepartamento?> Crear(
        DocenteDepartamento relacion)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>(
            "dbo.sp_crear_docente_departamento",
            Parametros(relacion),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<DocenteDepartamento?> Actualizar(
        DocenteDepartamento relacion)
    {
        using var conexion = CrearConexion();

        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>(
            "dbo.sp_actualizar_docente_departamento",
            Parametros(relacion),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Eliminar(
        int docente, int departamento)
    {
        using var conexion = CrearConexion();

        int filas = await conexion.QuerySingleAsync<int>(
            "dbo.sp_eliminar_docente_departamento",
            new
            {
                Docente = docente,
                Departamento = departamento
            },
            commandType: CommandType.StoredProcedure);

        return filas > 0;
    }

    private static object Parametros(
        DocenteDepartamento relacion)
    {
        return new
        {
            relacion.Docente,
            relacion.Departamento,
            relacion.Dedicacion,
            relacion.Modalidad,
            relacion.FechaIngreso,
            relacion.FechaSalida
        };
    }
}
