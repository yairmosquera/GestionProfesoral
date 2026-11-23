using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

/// <summary>
/// Controlador para las operaciones de Red.
/// </summary>
[ApiController]
[Route("api/redes")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
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
    public async Task<IActionResult> ObtenerTodos()
    {
        var redes = await _servicio.ObtenerTodasAsync();

        return Ok(redes);
    }

    /// <summary>
    /// Obtiene una red por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var red = await _servicio.ObtenerPorIdAsync(id);

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
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] RedCrear peticion)
    {
        var red = await _servicio.CrearAsync(peticion);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = red.Idr },
            red
        );
    }

    /// <summary>
    /// Reemplaza completamente una red.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] RedReemplazo peticion)
    {
        try
        {
            var red = await _servicio.ReemplazarAsync(
                id,
                peticion
            );

            return Ok(red);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente una red.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] RedActualizar peticion)
    {
        try
        {
            var red = await _servicio.ActualizarAsync(
                id,
                peticion
            );

            return Ok(red);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza el borrado lógico de una red.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpDelete("{id:int}")]
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