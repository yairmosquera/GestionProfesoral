
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/reconocimientos-docentes")]
public class ReconocimientosDocentesController : ControllerBase
{
    private readonly IServicioReconocimientoDocente _servicio;
    private readonly ILogger<ReconocimientosDocentesController> _logger;

    public ReconocimientosDocentesController(
        IServicioReconocimientoDocente servicio,
        ILogger<ReconocimientosDocentesController> logger)
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Consultar(int id)
    {
        if (id <= 0)
            return BadRequest(new { mensaje = "ID inválido." });

        var registro = await _servicio.Consultar(id);

        if (registro is null)
            return NotFound(new
            {
                mensaje = "No se encontró el reconocimiento."
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
        [FromBody] ReconocimientoDocentePeticion peticion)
    {
        try
        {
            var registro = await _servicio.Crear(peticion);

            if (registro is null)
                return StatusCode(500, new
                {
                    mensaje = "No fue posible registrar el reconocimiento."
                });

            return CreatedAtAction(
                nameof(Consultar),
                new { id = registro.Id },
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
                ex,
                "Error al registrar reconocimiento docente.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al registrar el reconocimiento."
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ReconocimientoDocentePeticion peticion)
    {
        if (id <= 0)
            return BadRequest(new { mensaje = "ID inválido." });

        try
        {
            var registro = await _servicio.Actualizar(id, peticion);

            if (registro is null)
                return NotFound(new
                {
                    mensaje = "No se encontró el reconocimiento activo."
                });

            return Ok(registro);
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
                "Error al actualizar reconocimiento {Id}.",
                id);

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al actualizar el reconocimiento."
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
            return BadRequest(new { mensaje = "ID inválido." });

        try
        {
            bool eliminado = await _servicio.Eliminar(id);

            if (!eliminado)
                return NotFound(new
                {
                    mensaje = "No se encontró el reconocimiento activo."
                });

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
                "Error al retirar reconocimiento {Id}.",
                id);

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al retirar el reconocimiento."
            });
        }
    }

    private static bool EsConflicto(SqlException ex)
    {
        return ex.Number is 51002 or 547 or 2627 or 2601;
    }

    private static string MensajeSql(SqlException ex)
    {
        return ex.Number switch
        {
            51002 => "El docente no existe o se encuentra inactivo.",
            547 => "La operación incumple una relación de la base de datos.",
            2627 or 2601 => "Ya existe un registro con esa clave.",
            _ => "Conflicto al procesar el reconocimiento."
        };
    }
}
