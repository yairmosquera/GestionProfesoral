using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/terminos-clave")]
[Produces("application/json")]
public class TerminoClaveController : ControllerBase
{
    private readonly IServicioTerminoClave _servicio;

    public TerminoClaveController(IServicioTerminoClave servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene todos los términos clave activos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TerminoClave>))]
    public async Task<IActionResult> ObtenerTodos()
    {
        var terminos = await _servicio.ObtenerTodosAsync();
        return Ok(terminos);
    }

    /// <summary>
    /// Obtiene un término clave por su nombre.
    /// </summary>
    [HttpGet("{termino}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TerminoClave))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorTermino(string termino)
    {
        try
        {
            var terminoClave = await _servicio.ObtenerPorTerminoAsync(termino);
            return Ok(terminoClave);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo término clave.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TerminoClave))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] TerminoClaveCrear dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _servicio.CrearAsync(dto);
        var terminoCreado = await _servicio.ObtenerPorTerminoAsync(dto.Termino);

        return CreatedAtAction(nameof(ObtenerPorTermino), new { termino = dto.Termino }, terminoCreado);
    }

    /// <summary>
    /// Reemplaza completamente un término clave existente (PUT).
    /// </summary>
    [HttpPut("{termino}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reemplazar(string termino, [FromBody] TerminoClaveReemplazo dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ReemplazarAsync(termino, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente un término clave (PATCH).
    /// </summary>
    [HttpPatch("{termino}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarParcial(string termino, [FromBody] TerminoClaveActualizar dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ActualizarParcialAsync(termino, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza un borrado lógico de un término clave (DELETE).
    /// </summary>
    [HttpDelete("{termino}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(string termino)
    {
        try
        {
            await _servicio.EliminarAsync(termino);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}