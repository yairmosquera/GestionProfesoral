
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/docentes-departamentos")]
public class DocentesDepartamentosController : ControllerBase
{
    private readonly IServicioDocenteDepartamento _servicio;
    private readonly ILogger<DocentesDepartamentosController> _logger;

    public DocentesDepartamentosController(
        IServicioDocenteDepartamento servicio,
        ILogger<DocentesDepartamentosController> logger)
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

    [HttpGet("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> Consultar(
        int docente, int departamento)
    {
        if (docente <= 0 || departamento <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El docente y el departamento deben ser válidos."
            });
        }

        var registro = await _servicio.Consultar(
            docente, departamento);

        if (registro is null)
        {
            return NotFound(new
            {
                mensaje = "No se encontró la asociación activa."
            });
        }

        return Ok(registro);
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

        var registros = await _servicio.PorDocente(cedula);

        return Ok(registros);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] DocenteDepartamentoPeticion peticion)
    {
        try
        {
            var registro = await _servicio.Crear(peticion);

            if (registro is null)
            {
                return StatusCode(500, new
                {
                    mensaje = "No fue posible crear la asociación."
                });
            }

            return CreatedAtAction(
                nameof(Consultar),
                new
                {
                    docente = registro.Docente,
                    departamento = registro.Departamento
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
                ex,
                "Error al crear asociación Docente-Departamento.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al crear la asociación."
            });
        }
    }

    [HttpPut("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> Actualizar(
        int docente,
        int departamento,
        [FromBody] DocenteDepartamentoPeticion peticion)
    {
        if (docente <= 0 || departamento <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El docente y el departamento deben ser válidos."
            });
        }

        if (peticion.Docente != docente ||
            peticion.Departamento != departamento)
        {
            return BadRequest(new
            {
                mensaje = "Las claves del cuerpo no coinciden con la URL."
            });
        }

        try
        {
            var registro = await _servicio.Actualizar(
                docente, departamento, peticion);

            if (registro is null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la asociación activa."
                });
            }

            return Ok(registro);
        }
        catch (SqlException ex) when (ex.Number == 51006)
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
                ex,
                "Error al actualizar asociación Docente-Departamento.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al actualizar la asociación."
            });
        }
    }

    [HttpDelete("{docente:int}/{departamento:int}")]
    public async Task<IActionResult> Eliminar(
        int docente, int departamento)
    {
        if (docente <= 0 || departamento <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El docente y el departamento deben ser válidos."
            });
        }

        try
        {
            bool eliminado = await _servicio.Eliminar(
                docente, departamento);

            if (!eliminado)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró la asociación activa."
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al retirar asociación Docente-Departamento.");

            return StatusCode(500, new
            {
                mensaje = "Ocurrió un error al retirar la asociación."
            });
        }
    }

    private static bool EsConflicto(SqlException ex)
    {
        return ex.Number is
            51002 or 51003 or 51004 or 51005
            or 547 or 2627 or 2601;
    }

    private static string MensajeSql(SqlException ex)
    {
        return ex.Number switch
        {
            51002 => "El docente no existe o está inactivo.",
            51003 => "El programa no existe o está inactivo.",
            51004 => "La fecha de salida no puede ser anterior a la de ingreso.",
            51005 => "El docente ya está asociado a este departamento.",
            547 => "La asociación incumple una clave foránea.",
            2627 or 2601 => "Ya existe una asociación con esa clave compuesta.",
            _ => "Conflicto al procesar la asociación."
        };
    }
}
