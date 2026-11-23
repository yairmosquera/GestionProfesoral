using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

/// <summary>
/// Controlador para las operaciones de Programa.
/// </summary>
[ApiController]
[Route("api/programas")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class ProgramaController : ControllerBase
{
    private readonly IServicioPrograma _servicio;

    public ProgramaController(IServicioPrograma servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene todos los programas activos.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var programas = await _servicio.ObtenerTodasAsync();

        return Ok(programas);
    }

    /// <summary>
    /// Obtiene un programa por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var programa = await _servicio.ObtenerPorIdAsync(id);

            return Ok(programa);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo programa.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ProgramaCrear peticion)
    {
        var programa = await _servicio.CrearAsync(peticion);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = programa.Id },
            programa
        );
    }

    /// <summary>
    /// Reemplaza completamente un programa.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] ProgramaReemplazo peticion)
    {
        try
        {
            var programa = await _servicio.ReemplazarAsync(id, peticion);

            return Ok(programa);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente un programa.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ProgramaActualizar peticion)
    {
        try
        {
            var programa = await _servicio.ActualizarAsync(id, peticion);

            return Ok(programa);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza el borrado lógico de un programa.
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