using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/lineas-investigacion")]
[Produces("application/json")]
public class LineaInvestigacionController : ControllerBase
{
    private readonly IServicioLineaInvestigacion _servicio;

    public LineaInvestigacionController(IServicioLineaInvestigacion servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene todas las líneas de investigación activas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LineaInvestigacion>))]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lineas = await _servicio.ObtenerTodasAsync();
        return Ok(lineas);
    }

    /// <summary>
    /// Obtiene una línea de investigación por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LineaInvestigacion))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var linea = await _servicio.ObtenerPorIdAsync(id);
            return Ok(linea);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva línea de investigación.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LineaInvestigacion))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] LineaInvestigacionCrear dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var idNuevo = await _servicio.CrearAsync(dto);
        var lineaCreada = await _servicio.ObtenerPorIdAsync(idNuevo);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = idNuevo }, lineaCreada);
    }

    /// <summary>
    /// Reemplaza completamente una línea de investigación existente (PUT).
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reemplazar(int id, [FromBody] LineaInvestigacionReemplazo dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ReemplazarAsync(id, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente una línea de investigación (PATCH).
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarParcial(int id, [FromBody] LineaInvestigacionActualizar dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _servicio.ActualizarParcialAsync(id, dto);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza un borrado lógico de una línea de investigación (DELETE).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _servicio.EliminarAsync(id);
            return NoContent();
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}