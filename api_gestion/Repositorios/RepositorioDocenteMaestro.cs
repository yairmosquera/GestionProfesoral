
using System.Data;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioDocenteMaestro
{
    private readonly string _cadenaConexion;

    public RepositorioDocenteMaestro(
        IConfiguration configuracion)
    {
        _cadenaConexion = configuracion
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión a SQL Server.");
    }

    // ==========================================
    // REGISTRAR DOCENTE CON TODOS SUS DETALLES
    // ==========================================

    public async Task<Docente> CrearAsync(
        DocenteMaestroPeticion peticion)
    {
        using var conexion =
            new SqlConnection(_cadenaConexion);

        await conexion.OpenAsync();

        using var transaccion =
            conexion.BeginTransaction();

        try
        {
            var d = peticion.Docente;

            // 1. GUARDAR EL DOCENTE PRINCIPAL

            var docente = await conexion
                .QuerySingleAsync<Docente>(
                    "dbo.sp_crear_docente",
                    new
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
                    },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure
                );

            // 2. REGISTRAR ESTUDIOS REALIZADOS

            foreach (var estudio in peticion.Estudios)
            {
                await conexion.ExecuteAsync(
                    "dbo.sp_crear_estudios_realizados",
                    new
                    {
                        estudio.Titulo,
                        estudio.Universidad,
                        Fecha = estudio.Fecha!.Value,
                        estudio.Tipo,
                        estudio.Ciudad,
                        Docente = docente.Cedula,
                        estudio.InsAcreditada,
                        estudio.Metodologia,
                        estudio.PerfilEgresado,
                        estudio.Pais
                    },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure
                );
            }

            // 3. REGISTRAR EVALUACIONES

            foreach (var evaluacion
                     in peticion.Evaluaciones)
            {
                await conexion.ExecuteAsync(
                    "dbo.sp_crear_evaluacion_docente",
                    new
                    {
                        evaluacion.Calificacion,
                        evaluacion.Semestre,
                        Docente = docente.Cedula
                    },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure
                );
            }

            // 4. REGISTRAR EXPERIENCIAS

            foreach (var experiencia
                     in peticion.Experiencias)
            {
                await conexion.ExecuteAsync(
                    "dbo.sp_crear_experiencia_docente",
                    new
                    {
                        experiencia.NombreCargo,
                        experiencia.Institucion,
                        experiencia.Tipo,
                        FechaInicio =
                            experiencia.FechaInicio!.Value,
                        experiencia.FechaFin,
                        Docente = docente.Cedula
                    },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure
                );
            }

            // 5. REGISTRAR RECONOCIMIENTOS

            foreach (var reconocimiento
                     in peticion.Reconocimientos)
            {
                await conexion.ExecuteAsync(
                    "dbo.sp_crear_reconocimiento",
                    new
                    {
                        reconocimiento.Tipo,
                        Fecha =
                            reconocimiento.Fecha!.Value,
                        reconocimiento.Institucion,
                        reconocimiento.Nombre,
                        reconocimiento.Ambito,
                        Docente = docente.Cedula
                    },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure
                );
            }

            // 6. CONFIRMAR TODAS LAS OPERACIONES

            transaccion.Commit();

            return docente;
        }
        catch
        {
            // Si falla algún detalle, se revierte
            // también el registro del docente.
            try
            {
                transaccion.Rollback();
            }
            catch (InvalidOperationException)
            {
                // SQL Server pudo finalizar la transacción
                // automáticamente al producirse el error.
            }
            catch (SqlException)
            {
                // Se conserva la excepción original.
            }

            throw;
        }
    }
}
