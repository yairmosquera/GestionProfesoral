
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/redes-docentes")]
public class RedesDocentesController : ControllerBase
{
    private readonly IServicioRedDocente _servicio;
    private readonly ILogger<RedesDocentesController> _logger;

    public RedesDocentesController(
        IServicioRedDocente servicio,
        ILogger<RedesDocentesController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var registros = await _servicio.Listar();
        return Ok(registros);
    }

    [HttpGet("{red:int}/{docente:int}")]
    public async Task<IActionResult> Consultar(int red, int docente)
    {
        if (red <= 0 || docente <= 0)
            return BadRequest(new
            {
                mensaje = "La red y el docente deben ser válidos."
            });

        var registro = await _servicio.Consultar(red, docente);

        if (registro is null)
            return NotFound(new
            {
                mensaje = "No se encontró la asociación."
            });

        return Ok(registro);
    }

    [HttpGet("docente/{cedula:int}")]
    public async Task<IActionResult> PorDocente(int cedula)
    {
        if (cedula <= 0)
            return BadRequest(new
            {
                mensaje = "La cédula debe ser mayor que cero."
            });

        var registros = await _servicio.PorDocente(cedula);
        return Ok(registros);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] RedDocentePeticion peticion)
    {
        try
        {
            var registro = await _servicio.Crear(peticion);

            if (registro is null)
                return StatusCode(500, new
                {
                    mensaje = "No fue posible asociar la red al docente."
                });

            return CreatedAtAction(
                nameof(Consultar),
                new
                {
                    red = registro.Red,
                    docente = registro.Docente
                },
                registro);
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
                ex, "Error al crear asociación Red-Docente.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al registrar la asociación."
            });
        }
    }

    [HttpPut("{red:int}/{docente:int}")]
    public async Task<IActionResult> Actualizar(
        int red,
        int docente,
        [FromBody] RedDocentePeticion peticion)
    {
        if (red <= 0 || docente <= 0)
            return BadRequest(new
            {
                mensaje = "La red y el docente deben ser válidos."
            });

        if (peticion.Red != red || peticion.Docente != docente)
            return BadRequest(new
            {
                mensaje = "La red y el docente del cuerpo deben coincidir con la URL."
            });

        try
        {
            var registro = await _servicio.Actualizar(
                red, docente, peticion);

            if (registro is null)
                return NotFound(new
                {
                    mensaje = "No se encontró la asociación activa."
                });

            return Ok(registro);
        }
        catch (SqlException ex) when (ex.Number == 51005)
        {
            return NotFound(new
            {
                mensaje = "No existe una asociación activa para actualizar."
            });
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
                ex, "Error al actualizar asociación Red-Docente.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al actualizar la asociación."
            });
        }
    }

    [HttpDelete("{red:int}/{docente:int}")]
    public async Task<IActionResult> Eliminar(
        int red, int docente)
    {
        if (red <= 0 || docente <= 0)
            return BadRequest(new
            {
                mensaje = "La red y el docente deben ser válidos."
            });

        try
        {
            bool eliminado = await _servicio.Eliminar(red, docente);

            if (!eliminado)
                return NotFound(new
                {
                    mensaje = "No se encontró la asociación activa."
                });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Error al retirar asociación Red-Docente.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al retirar la asociación."
            });
        }
    }

    private static bool EsConflicto(SqlException ex)
    {
        return ex.Number is
            51002 or 51003 or 51004 or 547 or 2627 or 2601;
    }

    private static string MensajeSql(SqlException ex)
    {
        return ex.Number switch
        {
            51002 => "La red no existe o se encuentra inactiva.",
            51003 => "El docente no existe o se encuentra inactivo.",
            51004 => "El docente ya está asociado a esta red.",
            547 => "La relación incumple una clave foránea.",
            2627 or 2601 => "Ya existe una relación con esa clave compuesta.",
            _ => "Conflicto al procesar la relación Red-Docente."
        };
    }
}
