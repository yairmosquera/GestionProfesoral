using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/evaluaciones-docentes")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class EvaluacionesDocentesController : ControllerBase
{
    private readonly IServicioEvaluacionDocente _servicio;

    public EvaluacionesDocentesController(
        IServicioEvaluacionDocente servicio)
    {
        _servicio = servicio;
    }

    // GET /api/evaluaciones-docentes
    // ADMIN, GESTOR y CONSULTA
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerTodosAsync());
    }

    // GET /api/evaluaciones-docentes/1
    // ADMIN, GESTOR y CONSULTA
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var evaluacion = await _servicio.ObtenerPorIdAsync(id);

        return evaluacion is null
            ? NotFound(new
            {
                mensaje = "Evaluación no encontrada."
            })
            : Ok(evaluacion);
    }

    // GET /api/evaluaciones-docentes/docente/123456
    // ADMIN, GESTOR y CONSULTA
    [HttpGet("docente/{cedula:int}")]
    public async Task<IActionResult> ObtenerPorDocente(
        int cedula)
    {
        return Ok(
            await _servicio.ObtenerPorDocenteAsync(cedula));
    }

    // POST /api/evaluaciones-docentes
    // Solo ADMIN y GESTOR
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EvaluacionDocentePeticion peticion)
    {
        try
        {
            var evaluacion = await _servicio.CrearAsync(
                peticion);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = evaluacion.Id },
                evaluacion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 51002)
        {
            return Conflict(new
            {
                mensaje = "El docente no existe o está retirado."
            });
        }
    }

    // PUT /api/evaluaciones-docentes/1
    // Solo ADMIN y GESTOR
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] EvaluacionDocentePeticion peticion)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El ID no es válido."
            });
        }

        try
        {
            var evaluacion = await _servicio.ActualizarAsync(
                id, peticion);

            return evaluacion is null
                ? NotFound(new
                {
                    mensaje = "Evaluación no encontrada."
                })
                : Ok(evaluacion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 51002)
        {
            return Conflict(new
            {
                mensaje = "El docente no existe o está retirado."
            });
        }
    }

    // DELETE /api/evaluaciones-docentes/1
    // Solo ADMIN y GESTOR
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _servicio.EliminarAsync(id);

        return eliminado
            ? NoContent()
            : NotFound(new
            {
                mensaje = "Evaluación no encontrada."
            });
    }
}