
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/experiencias-docentes")]
public class ExperienciasDocentesController : ControllerBase
{
    private readonly IServicioExperienciaDocente _servicio;
    private readonly ILogger<ExperienciasDocentesController> _logger;

    public ExperienciasDocentesController(
        IServicioExperienciaDocente servicio,
        ILogger<ExperienciasDocentesController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var experiencias = await _servicio.Listar();

        return Ok(experiencias);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Consultar(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El ID debe ser mayor que cero."
            });
        }

        var experiencia = await _servicio.Consultar(id);

        if (experiencia is null)
        {
            return NotFound(new
            {
                mensaje = "No se encontró la experiencia."
            });
        }

        return Ok(experiencia);
    }

    [HttpGet("docente/{cedula:int}")]
    public async Task<IActionResult> PorDocente(int cedula)
    {
        if (cedula <= 0)
        {
            return BadRequest(new
            {
                mensaje = "La cédula debe ser mayor que cero."
            });
        }

        var experiencias = await _servicio.PorDocente(cedula);

        return Ok(experiencias);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ExperienciaDocentePeticion peticion)
    {
        try
        {
            var experiencia = await _servicio.Crear(peticion);

            if (experiencia is null)
            {
                return StatusCode(500, new
                {
                    mensaje = "No fue posible registrar la experiencia."
                });
            }

            return CreatedAtAction(
                nameof(Consultar),
                new { id = experiencia.Id },
                experiencia);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new
            {
                mensaje = MensajeSql(ex)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al registrar una experiencia docente.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al registrar la experiencia."
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ExperienciaDocentePeticion peticion)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El ID debe ser mayor que cero."
            });
        }

        try
        {
            var experiencia = await _servicio.Actualizar(
                id, peticion);

            if (experiencia is null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la experiencia activa."
                });
            }

            return Ok(experiencia);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new
            {
                mensaje = MensajeSql(ex)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al actualizar la experiencia {Id}.",
                id);

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al actualizar la experiencia."
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El ID debe ser mayor que cero."
            });
        }

        try
        {
            var eliminada = await _servicio.Eliminar(id);

            if (!eliminada)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la experiencia activa."
                });
            }

            return NoContent();
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new
            {
                mensaje = MensajeSql(ex)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al retirar la experiencia {Id}.",
                id);

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al retirar la experiencia."
            });
        }
    }

    private static bool EsConflicto(SqlException ex)
    {
        return ex.Number is 51002 or 51003 or 547 or 2627 or 2601;
    }

    private static string MensajeSql(SqlException ex)
    {
        return ex.Number switch
        {
            51002 => "El docente no existe o está inactivo.",
            51003 => "La fecha final no puede ser anterior a la inicial.",
            547 => "La operación incumple una relación de la base de datos.",
            2627 or 2601 => "Ya existe un registro con esa clave.",
            _ => "Conflicto al procesar la experiencia."
        };
    }
}
