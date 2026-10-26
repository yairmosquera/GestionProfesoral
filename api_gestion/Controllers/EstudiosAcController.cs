using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/estudios-ac")]
public class EstudiosAcController : ControllerBase
{
    private readonly IServicioEstudioAc _servicio;

    public EstudiosAcController(IServicioEstudioAc servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _servicio.Listar());

    [HttpGet("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> Consultar(int estudio, string areaConocimiento)
    {
        var relacion = await _servicio.Consultar(estudio, areaConocimiento);
        return relacion is null
            ? NotFound(new { mensaje = "No se encontró la asociación activa." })
            : Ok(relacion);
    }

    [HttpGet("estudio/{estudio:int}")]
    public async Task<IActionResult> AreasPorEstudio(int estudio)
        => Ok(await _servicio.AreasPorEstudio(estudio));

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EstudioAcPeticion peticion)
    {
        try
        {
            var relacion = await _servicio.Crear(peticion);
            return relacion is null
                ? StatusCode(500, new { mensaje = "No fue posible asociar el área al estudio." })
                : CreatedAtAction(
                    nameof(Consultar),
                    new
                    {
                        estudio = relacion.Estudio,
                        areaConocimiento = relacion.AreaConocimiento
                    },
                    relacion);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [HttpDelete("{estudio:int}/{areaConocimiento}")]
    public async Task<IActionResult> Eliminar(int estudio, string areaConocimiento)
    {
        try
        {
            return await _servicio.Eliminar(estudio, areaConocimiento)
                ? NoContent()
                : NotFound(new { mensaje = "No se encontró la asociación activa." });
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    private static bool EsConflicto(SqlException ex)
        => ex.Number is 51011 or 547 or 2627 or 2601;

    private static string MensajeConflicto(SqlException ex)
        => ex.Number switch
        {
            51011 => "El área de conocimiento ya está asociada a ese estudio.",
            547 => "El estudio o el área de conocimiento seleccionada no existe.",
            2627 or 2601 => "Ya existe una asociación con esa clave compuesta.",
            _ => "Conflicto al procesar la asociación del área."
        };
}
