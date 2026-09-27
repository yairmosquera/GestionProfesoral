using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/redes")]
[Produces("application/json")]
public class RedController : ControllerBase
{
    private readonly IServicioRed _servicio;

    public RedController(IServicioRed servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene todas las redes activas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Red>))]
    public async Task<IActionResult> ObtenerTodas()
    {
        var redes = await _servicio.ObtenerTodasAsync();
        return Ok(redes);
    }

    /// <summary>
    /// Obtiene una red por su ID (idr).
    /// </summary>
    [HttpGet("{idr:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Red))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int idr)
    {
        try
        {
            var red = await _servicio.ObtenerPorIdAsync(idr);
            return Ok(red);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva red.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Red))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] RedCrear dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _servicio.CrearAsync(dto);
        var redCreada = await _servicio.ObtenerPorIdAsync(dto.Idr);

        return CreatedAtAction(nameof(ObtenerPorId), new { idr = dto.Idr }, redCreada);
    }

    /// <summary>
    /// Reemplaza completamente una red existente (PUT).
    /// </summary>
    [HttpPut("{idr:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reemplazar(int idr, [FromBody] RedReemplazo dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ReemplazarAsync(idr, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente una red (PATCH).
    /// </summary>
    [HttpPatch("{idr:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarParcial(int idr, [FromBody] RedActualizar dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ActualizarParcialAsync(idr, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza un borrado lógico de una red (DELETE).
    /// </summary>
    [HttpDelete("{idr:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int idr)
    {
        try
        {
            await _servicio.EliminarAsync(idr);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}