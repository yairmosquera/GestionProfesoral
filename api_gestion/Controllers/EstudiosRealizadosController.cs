using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/estudios-realizados")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class EstudiosRealizadosController : ControllerBase
{
    private readonly IServicioEstudioRealizado _servicio;

    public EstudiosRealizadosController(
        IServicioEstudioRealizado servicio)
    {
        _servicio = servicio;
    }

    // ADMIN, GESTOR y CONSULTA pueden consultar los estudios.
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var estudio = await _servicio.ObtenerPorIdAsync(id);

        return estudio is null
            ? NotFound(new { mensaje = "Estudio no encontrado." })
            : Ok(estudio);
    }

    [HttpGet("docente/{cedula:int}")]
    public async Task<IActionResult> ObtenerPorDocente(int cedula)
    {
        return Ok(
            await _servicio.ObtenerPorDocenteAsync(cedula));
    }

    // Solo ADMIN y GESTOR pueden registrar, editar y retirar.
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EstudioRealizadoCrearPeticion peticion)
    {
        try
        {
            var estudio = await _servicio.CrearAsync(peticion);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = estudio.Id },
                estudio);
        }
        catch (SqlException ex)
            when (ex.Number is 2627 or 2601)
        {
            return Conflict(new
            {
                mensaje = "El identificador del estudio ya existe."
            });
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 51002)
        {
            return Conflict(new
            {
                mensaje = "El docente seleccionado no existe o está retirado."
            });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] EstudioRealizadoPeticion peticion)
    {
        if (id != peticion.Id)
        {
            return BadRequest(new
            {
                mensaje = "El identificador no coincide."
            });
        }

        try
        {
            var estudio = await _servicio.ActualizarAsync(
                id, peticion);

            return estudio is null
                ? NotFound(new
                {
                    mensaje = "Estudio no encontrado."
                })
                : Ok(estudio);
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 51002)
        {
            return Conflict(new
            {
                mensaje = "El docente seleccionado no existe o está retirado."
            });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _servicio.EliminarAsync(id);

        return eliminado
            ? NoContent()
            : NotFound(new
            {
                mensaje = "Estudio no encontrado."
            });
    }
}