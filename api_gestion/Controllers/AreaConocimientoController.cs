using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

/// <summary>
/// Controlador para las operaciones de Área de Conocimiento.
/// </summary>
[ApiController]
[Route("api/areas-conocimiento")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class AreaConocimientoController : ControllerBase
{
    private readonly IServicioAreaConocimiento _servicio;

    public AreaConocimientoController(IServicioAreaConocimiento servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene todas las áreas de conocimiento activas.
    /// Disponible para ADMIN, GESTOR y CONSULTA.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var areas = await _servicio.ObtenerTodasAsync();

        return Ok(areas);
    }

    /// <summary>
    /// Obtiene un área de conocimiento por su ID.
    /// Disponible para ADMIN, GESTOR y CONSULTA.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(string id)
    {
        try
        {
            var area = await _servicio.ObtenerPorIdAsync(id);

            return Ok(area);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva área de conocimiento.
    /// Solo ADMIN y GESTOR.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] AreaConocimientoCrear peticion)
    {
        var area = await _servicio.CrearAsync(peticion);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = area.Id },
            area
        );
    }

    /// <summary>
    /// Reemplaza completamente un área de conocimiento.
    /// Solo ADMIN y GESTOR.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Reemplazar(
        string id,
        [FromBody] AreaConocimientoReemplazo peticion)
    {
        try
        {
            var area = await _servicio.ReemplazarAsync(id, peticion);

            return Ok(area);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza parcialmente un área de conocimiento.
    /// Solo ADMIN y GESTOR.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Actualizar(
        string id,
        [FromBody] AreaConocimientoActualizar peticion)
    {
        try
        {
            var area = await _servicio.ActualizarAsync(id, peticion);

            return Ok(area);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Realiza el borrado lógico de un área de conocimiento.
    /// Solo ADMIN y GESTOR.
    /// </summary>
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(string id)
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