using ApiGestion.Excepciones;
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

/// <summary>
/// Controlador para las operaciones de Línea de Investigación.
/// </summary>
[ApiController]
[Route("api/lineas-investigacion")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class LineaInvestigacionController : ControllerBase
{
    private readonly IServicioLineaInvestigacion _servicio;

    public LineaInvestigacionController(
        IServicioLineaInvestigacion servicio)
    {
        _servicio = servicio;
    }

    // ADMIN, GESTOR y CONSULTA pueden leer.
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var lineas = await _servicio.ObtenerTodasAsync();

        return Ok(lineas);
    }

    [HttpGet("{id:int}")]
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

    // Solo ADMIN y GESTOR pueden modificar.
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] LineaInvestigacionCrear peticion)
    {
        var linea = await _servicio.CrearAsync(peticion);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = linea.Id },
            linea
        );
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(
        int id,
        [FromBody] LineaInvestigacionReemplazo peticion)
    {
        try
        {
            var linea = await _servicio.ReemplazarAsync(
                id,
                peticion
            );

            return Ok(linea);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] LineaInvestigacionActualizar peticion)
    {
        try
        {
            var linea = await _servicio.ActualizarAsync(
                id,
                peticion
            );

            return Ok(linea);
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

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