
using System.Data;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Repositorios;

public class RepositorioDocenteMaestroEdicion
{
    private readonly string _cadenaConexion;

    public RepositorioDocenteMaestroEdicion(
        IConfiguration configuracion)
    {
        _cadenaConexion = configuracion
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    // ============================================
    // ACTUALIZAR DOCENTE CON TODOS SUS DETALLES
    // ============================================

    public async Task<Docente?> ActualizarAsync(
        int cedula,
        DocenteMaestroEdicionPeticion peticion)
    {
        if (peticion.Docente is null)
        {
            throw new ArgumentException(
                "Debe enviar los datos del docente.");
        }

        if (peticion.Docente.Cedula != cedula)
        {
            throw new ArgumentException(
                "La cédula del docente no coincide.");
        }

        // Evitar que se solicite actualizar y retirar
        // un mismo registro en una sola operación.
        ValidarOperaciones(peticion);

        await using var conexion =
            new SqlConnection(_cadenaConexion);

        await conexion.OpenAsync();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            // ====================================
            // 1. VERIFICAR DOCENTE PRINCIPAL
            // ====================================

            var existente = await conexion
                .QueryFirstOrDefaultAsync<Docente>(
                    "dbo.sp_consultar_docente",
                    new { Cedula = cedula },
                    transaction: transaccion,
                    commandType: CommandType.StoredProcedure);

            if (existente is null)
            {
                transaccion.Rollback();
                return null;
            }

            // ====================================
            // 2. ACTUALIZAR DATOS DEL DOCENTE
            // ====================================

            var d = peticion.Docente;

            var docente = await conexion
                .QueryFirstOrDefaultAsync<Docente>(
                    "dbo.sp_actualizar_docente",
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
                    commandType: CommandType.StoredProcedure);

            if (docente is null)
            {
                throw new InvalidOperationException(
                    "No fue posible actualizar el docente.");
            }

            // ====================================
            // 3. ESTUDIOS REALIZADOS
            // ====================================

            foreach (var estudio in peticion.Estudios)
            {
                if (estudio.Id.HasValue)
                {
                    await VerificarPropietarioAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_consultar_estudios_realizados",
                        estudio.Id.Value,
                        cedula);

                    await EjecutarActualizacionAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_actualizar_estudios_realizados",
                        new
                        {
                            Id = estudio.Id.Value,
                            estudio.Titulo,
                            estudio.Universidad,
                            Fecha = estudio.Fecha!.Value,
                            estudio.Tipo,
                            estudio.Ciudad,
                            Docente = cedula,
                            estudio.InsAcreditada,
                            estudio.Metodologia,
                            estudio.PerfilEgresado,
                            estudio.Pais
                        });
                }
                else
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
                            Docente = cedula,
                            estudio.InsAcreditada,
                            estudio.Metodologia,
                            estudio.PerfilEgresado,
                            estudio.Pais
                        },
                        transaction: transaccion,
                        commandType: CommandType.StoredProcedure);
                }
            }

            // ====================================
            // 4. EVALUACIONES
            // ====================================

            foreach (var evaluacion in peticion.Evaluaciones)
            {
                if (evaluacion.Id.HasValue)
                {
                    await VerificarPropietarioAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_consultar_evaluacion_docente",
                        evaluacion.Id.Value,
                        cedula);

                    await EjecutarActualizacionAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_actualizar_evaluacion_docente",
                        new
                        {
                            Id = evaluacion.Id.Value,
                            evaluacion.Calificacion,
                            evaluacion.Semestre,
                            Docente = cedula
                        });
                }
                else
                {
                    await conexion.ExecuteAsync(
                        "dbo.sp_crear_evaluacion_docente",
                        new
                        {
                            evaluacion.Calificacion,
                            evaluacion.Semestre,
                            Docente = cedula
                        },
                        transaction: transaccion,
                        commandType: CommandType.StoredProcedure);
                }
            }

            // ====================================
            // 5. EXPERIENCIAS
            // ====================================

            foreach (var experiencia in peticion.Experiencias)
            {
                if (experiencia.Id.HasValue)
                {
                    await VerificarPropietarioAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_consultar_experiencia_docente",
                        experiencia.Id.Value,
                        cedula);

                    await EjecutarActualizacionAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_actualizar_experiencia_docente",
                        new
                        {
                            Id = experiencia.Id.Value,
                            experiencia.NombreCargo,
                            experiencia.Institucion,
                            experiencia.Tipo,
                            FechaInicio =
                                experiencia.FechaInicio!.Value,
                            experiencia.FechaFin,
                            Docente = cedula
                        });
                }
                else
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
                            Docente = cedula
                        },
                        transaction: transaccion,
                        commandType: CommandType.StoredProcedure);
                }
            }

            // ====================================
            // 6. RECONOCIMIENTOS
            // ====================================

            foreach (var reconocimiento in peticion.Reconocimientos)
            {
                if (reconocimiento.Id.HasValue)
                {
                    await VerificarPropietarioAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_consultar_reconocimiento",
                        reconocimiento.Id.Value,
                        cedula);

                    await EjecutarActualizacionAsync(
                        conexion,
                        transaccion,
                        "dbo.sp_actualizar_reconocimiento",
                        new
                        {
                            Id = reconocimiento.Id.Value,
                            reconocimiento.Tipo,
                            Fecha = reconocimiento.Fecha!.Value,
                            reconocimiento.Institucion,
                            reconocimiento.Nombre,
                            reconocimiento.Ambito,
                            Docente = cedula
                        });
                }
                else
                {
                    await conexion.ExecuteAsync(
                        "dbo.sp_crear_reconocimiento",
                        new
                        {
                            reconocimiento.Tipo,
                            Fecha = reconocimiento.Fecha!.Value,
                            reconocimiento.Institucion,
                            reconocimiento.Nombre,
                            reconocimiento.Ambito,
                            Docente = cedula
                        },
                        transaction: transaccion,
                        commandType: CommandType.StoredProcedure);
                }
            }

            // ====================================
            // 7. RETIRAR ESTUDIOS
            // ====================================

            foreach (var id in peticion.EstudiosPorRetirar)
            {
                await RetirarDetalleAsync(
                    conexion,
                    transaccion,
                    "dbo.sp_consultar_estudios_realizados",
                    "dbo.sp_eliminar_estudios_realizados",
                    id,
                    cedula);
            }

            // ====================================
            // 8. RETIRAR EVALUACIONES
            // ====================================

            foreach (var id in peticion.EvaluacionesPorRetirar)
            {
                await RetirarDetalleAsync(
                    conexion,
                    transaccion,
                    "dbo.sp_consultar_evaluacion_docente",
                    "dbo.sp_eliminar_evaluacion_docente",
                    id,
                    cedula);
            }

            // ====================================
            // 9. RETIRAR EXPERIENCIAS
            // ====================================

            foreach (var id in peticion.ExperienciasPorRetirar)
            {
                await RetirarDetalleAsync(
                    conexion,
                    transaccion,
                    "dbo.sp_consultar_experiencia_docente",
                    "dbo.sp_eliminar_experiencia_docente",
                    id,
                    cedula);
            }

            // ====================================
            // 10. RETIRAR RECONOCIMIENTOS
            // ====================================

            foreach (var id in peticion.ReconocimientosPorRetirar)
            {
                await RetirarDetalleAsync(
                    conexion,
                    transaccion,
                    "dbo.sp_consultar_reconocimiento",
                    "dbo.sp_eliminar_reconocimiento",
                    id,
                    cedula);
            }

            // ====================================
            // 11. VALIDAR DETALLES OBLIGATORIOS
            // ====================================

            await VerificarCantidadAsync(
                conexion,
                transaccion,
                "dbo.sp_estudios_por_docente",
                cedula,
                "estudio");

            await VerificarCantidadAsync(
                conexion,
                transaccion,
                "dbo.sp_evaluaciones_por_docente",
                cedula,
                "evaluación");

            await VerificarCantidadAsync(
                conexion,
                transaccion,
                "dbo.sp_experiencias_por_docente",
                cedula,
                "experiencia");

            await VerificarCantidadAsync(
                conexion,
                transaccion,
                "dbo.sp_reconocimientos_por_docente",
                cedula,
                "reconocimiento");

            // ====================================
            // 12. CONFIRMAR TRANSACCIÓN
            // ====================================

            transaccion.Commit();

            return docente;
        }
        catch
        {
            try
            {
                transaccion.Rollback();
            }
            catch (InvalidOperationException)
            {
                // La transacción pudo haber finalizado.
            }
            catch (SqlException)
            {
                // Conservar la excepción original.
            }

            throw;
        }
    }

    // ============================================
    // VALIDAR IDENTIFICADORES DUPLICADOS
    // ============================================

    private static void ValidarOperaciones(
        DocenteMaestroEdicionPeticion peticion)
    {
        ValidarIds(
            peticion.Estudios.Select(x => x.Id),
            peticion.EstudiosPorRetirar,
            "estudios");

        ValidarIds(
            peticion.Evaluaciones.Select(x => x.Id),
            peticion.EvaluacionesPorRetirar,
            "evaluaciones");

        ValidarIds(
            peticion.Experiencias.Select(x => x.Id),
            peticion.ExperienciasPorRetirar,
            "experiencias");

        ValidarIds(
            peticion.Reconocimientos.Select(x => x.Id),
            peticion.ReconocimientosPorRetirar,
            "reconocimientos");
    }

    private static void ValidarIds(
        IEnumerable<int?> identificadores,
        List<int> retiros,
        string nombre)
    {
        var ids = identificadores
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        if (ids.Any(x => x <= 0) ||
            retiros.Any(x => x <= 0))
        {
            throw new ArgumentException(
                $"Hay identificadores inválidos en {nombre}.");
        }

        if (ids.Count != ids.Distinct().Count() ||
            retiros.Count != retiros.Distinct().Count())
        {
            throw new ArgumentException(
                $"Hay identificadores repetidos en {nombre}.");
        }

        if (ids.Intersect(retiros).Any())
        {
            throw new ArgumentException(
                $"No puede actualizar y retirar el mismo registro en {nombre}.");
        }
    }

    // ============================================
    // VERIFICAR QUE UN DETALLE SEA DEL DOCENTE
    // ============================================

    private static async Task VerificarPropietarioAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string procedimientoConsultar,
        int id,
        int cedula)
    {
        var registro = await conexion
            .QueryFirstOrDefaultAsync<DetallePropietario>(
                procedimientoConsultar,
                new { Id = id },
                transaction: transaccion,
                commandType: CommandType.StoredProcedure);

        if (registro is null)
        {
            throw new InvalidOperationException(
                $"El detalle {id} no existe o está retirado.");
        }

        if (registro.Docente != cedula)
        {
            throw new InvalidOperationException(
                $"El detalle {id} pertenece a otro docente.");
        }
    }

    // ============================================
    // EJECUTAR ACTUALIZACIÓN
    // ============================================

    private static async Task EjecutarActualizacionAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string procedimiento,
        object parametros)
    {
        // Estos procedimientos devuelven el registro
        // actualizado mediante SELECT.
        var resultado = await conexion
            .QueryFirstOrDefaultAsync<dynamic>(
                procedimiento,
                parametros,
                transaction: transaccion,
                commandType: CommandType.StoredProcedure);

        if (resultado is null)
        {
            throw new InvalidOperationException(
                $"No fue posible completar {procedimiento}.");
        }
    }

    // ============================================
    // RETIRAR DETALLE
    // ============================================

    private static async Task RetirarDetalleAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string procedimientoConsultar,
        string procedimientoRetirar,
        int id,
        int cedula)
    {
        await VerificarPropietarioAsync(
            conexion,
            transaccion,
            procedimientoConsultar,
            id,
            cedula);

        // El procedimiento de retiro devuelve
        // la cantidad de filas afectadas.
        var filas = await conexion.ExecuteScalarAsync<int>(
            procedimientoRetirar,
            new { Id = id },
            transaction: transaccion,
            commandType: CommandType.StoredProcedure);

        if (filas <= 0)
        {
            throw new InvalidOperationException(
                $"No fue posible retirar el detalle {id}.");
        }
    }

    // ============================================
    // GARANTIZAR AL MENOS UN DETALLE POR SECCIÓN
    // ============================================

    private static async Task VerificarCantidadAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string procedimiento,
        int cedula,
        string tipoDetalle)
    {
        var registros = await conexion.QueryAsync(
            procedimiento,
            new { Cedula = cedula },
            transaction: transaccion,
            commandType: CommandType.StoredProcedure);

        if (!registros.Any())
        {
            throw new InvalidOperationException(
                $"El docente debe conservar al menos un {tipoDetalle}.");
        }
    }

    // ============================================
    // MODELO INTERNO PARA VERIFICAR PROPIETARIO
    // ============================================

    private sealed class DetallePropietario
    {
        public int Docente { get; set; }
    }
}
