
using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/docentes")]
public class DocenteController : ControllerBase
{
    private readonly IServicioDocente _servicio;

    public DocenteController(IServicioDocente servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
        => Ok(await _servicio.ObtenerTodosAsync());

    [HttpGet("{cedula:int}")]
    public async Task<IActionResult> ObtenerPorId(int cedula)
    {
        var docente = await _servicio.ObtenerPorIdAsync(cedula);
        return docente is null
            ? NotFound(new { mensaje = "Docente no encontrado." })
            : Ok(docente);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] DocentePeticion peticion)
    {
        try
        {
            var docente = await _servicio.CrearAsync(peticion);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { cedula = docente.Cedula },
                docente);
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un docente con esa cédula."
            });
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Conflict(new
            {
                mensaje = "La línea de investigación seleccionada no existe."
            });
        }
    }

    [HttpPut("{cedula:int}")]
    public async Task<IActionResult> Actualizar(
        int cedula,
        [FromBody] DocentePeticion peticion)
    {
        if (peticion.Cedula != cedula)
            return BadRequest(new
            {
                mensaje = "La cédula no coincide."
            });

        try
        {
            var docente = await _servicio.ActualizarAsync(
                cedula, peticion);

            return docente is null
                ? NotFound(new { mensaje = "Docente no encontrado." })
                : Ok(docente);
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Conflict(new
            {
                mensaje = "La línea de investigación seleccionada no existe."
            });
        }
    }

    [HttpDelete("{cedula:int}")]
    public async Task<IActionResult> Eliminar(int cedula)
    {
        try
        {
            var eliminado = await _servicio.EliminarAsync(cedula);

            return eliminado
                ? NoContent()
                : NotFound(new { mensaje = "Docente no encontrado." });
        }
        catch (SqlException ex) when (ex.Number == 51001)
        {
            return Conflict(new
            {
                mensaje = "No se puede retirar el docente porque tiene relaciones que deben revisarse."
            });
        }
    }
}
