
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/docentes-maestro")]
public class DocenteMaestroEdicionController : ControllerBase
{
    private readonly RepositorioDocenteMaestroEdicion _repositorio;

    public DocenteMaestroEdicionController(
        RepositorioDocenteMaestroEdicion repositorio)
    {
        _repositorio = repositorio;
    }

    // ============================================
    // ACTUALIZAR DOCENTE Y TODOS SUS DETALLES
    // Solo ADMIN y GESTOR
    // ============================================

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{cedula:int}")]
    public async Task<IActionResult> Actualizar(
        int cedula,
        [FromBody] DocenteMaestroEdicionPeticion peticion)
    {
        if (peticion.Docente is null)
        {
            return BadRequest(new
            {
                mensaje = "Los datos del docente son obligatorios."
            });
        }

        if (cedula != peticion.Docente.Cedula)
        {
            return BadRequest(new
            {
                mensaje = "La cédula del docente no coincide."
            });
        }

        if (peticion.Estudios is null ||
            peticion.Evaluaciones is null ||
            peticion.Experiencias is null ||
            peticion.Reconocimientos is null ||
            peticion.EstudiosPorRetirar is null ||
            peticion.EvaluacionesPorRetirar is null ||
            peticion.ExperienciasPorRetirar is null ||
            peticion.ReconocimientosPorRetirar is null)
        {
            return BadRequest(new
            {
                mensaje = "Las listas de detalles no pueden ser nulas."
            });
        }

        try
        {
            var docente = await _repositorio.ActualizarAsync(
                cedula, peticion);

            if (docente is null)
            {
                return NotFound(new
                {
                    mensaje = "El docente no existe o está retirado."
                });
            }

            return Ok(new
            {
                mensaje =
                    "Docente y detalles actualizados correctamente.",
                cedula = docente.Cedula
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                mensaje = ex.Message
            });
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 2601 or 2627)
        {
            return Conflict(new
            {
                mensaje =
                    "No se pudo actualizar debido a una relación o registro duplicado."
            });
        }
    }
}
