using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

/// <summary>
/// Controlador para las operaciones de Término Clave.
/// </summary>
[ApiController]
[Route("api/terminos-clave")]
public class TerminoClaveController : ControllerBase
{
    private readonly IServicioTerminoClave _servicio;

    public TerminoClaveController(
        IServicioTerminoClave servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var terminos =
            await _servicio.ObtenerTodosAsync();

        return Ok(terminos);
    }

    [HttpGet("{termino}")]
    public async Task<IActionResult> ObtenerPorTermino(
        string termino)
    {
        try
        {
            var terminoClave =
                await _servicio.ObtenerPorTerminoAsync(termino);

            return Ok(terminoClave);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(
                new { mensaje = ex.Message }
            );
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] TerminoClaveCrear peticion)
    {
        var terminoClave =
            await _servicio.CrearAsync(peticion);

        return CreatedAtAction(
            nameof(ObtenerPorTermino),
            new { termino = terminoClave.Termino },
            terminoClave
        );
    }

    [HttpPut("{termino}")]
    public async Task<IActionResult> Reemplazar(
        string termino,
        [FromBody] TerminoClaveReemplazo peticion)
    {
        try
        {
            var terminoClave =
                await _servicio.ReemplazarAsync(
                    termino,
                    peticion
                );

            return Ok(terminoClave);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(
                new { mensaje = ex.Message }
            );
        }
    }

    [HttpPatch("{termino}")]
    public async Task<IActionResult> Actualizar(
        string termino,
        [FromBody] TerminoClaveActualizar peticion)
    {
        try
        {
            var terminoClave =
                await _servicio.ActualizarAsync(
                    termino,
                    peticion
                );

            return Ok(terminoClave);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(
                new { mensaje = ex.Message }
            );
        }
    }

    [HttpDelete("{termino}")]
    public async Task<IActionResult> Eliminar(
        string termino)
    {
        try
        {
            await _servicio.EliminarAsync(termino);

            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(
                new { mensaje = ex.Message }
            );
        }
    }
}