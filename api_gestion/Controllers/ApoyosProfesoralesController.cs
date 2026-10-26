using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/apoyos-profesorales")]
public class ApoyosProfesoralesController : ControllerBase
{
    private readonly IServicioApoyoProfesoral _servicio;

    public ApoyosProfesoralesController(IServicioApoyoProfesoral servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _servicio.Listar());

    [HttpGet("{estudios:int}")]
    public async Task<IActionResult> Consultar(int estudios)
    {
        var apoyo = await _servicio.Consultar(estudios);
        return apoyo is null
            ? NotFound(new { mensaje = "No se encontró el apoyo profesoral activo." })
            : Ok(apoyo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ApoyoProfesoralPeticion peticion)
    {
        try
        {
            var apoyo = await _servicio.Crear(peticion);
            return apoyo is null
                ? StatusCode(500, new { mensaje = "No fue posible registrar el apoyo profesoral." })
                : CreatedAtAction(nameof(Consultar), new { estudios = apoyo.Estudios }, apoyo);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [HttpPut("{estudios:int}")]
    public async Task<IActionResult> Actualizar(
        int estudios,
        [FromBody] ApoyoProfesoralPeticion peticion)
    {
        if (peticion.Estudios != estudios)
            return BadRequest(new { mensaje = "El estudio del cuerpo no coincide con la URL." });

        try
        {
            var apoyo = await _servicio.Actualizar(estudios, peticion);
            return apoyo is null
                ? NotFound(new { mensaje = "No se encontró el apoyo profesoral activo." })
                : Ok(apoyo);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [HttpDelete("{estudios:int}")]
    public async Task<IActionResult> Eliminar(int estudios)
    {
        try
        {
            return await _servicio.Eliminar(estudios)
                ? NoContent()
                : NotFound(new { mensaje = "No se encontró el apoyo profesoral activo." });
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    private static bool EsConflicto(SqlException ex)
        => ex.Number is 51012 or 51013 or 547 or 2627 or 2601;

    private static string MensajeConflicto(SqlException ex)
        => ex.Number switch
        {
            51012 => "El indicador de apoyo debe ser 0 o 1.",
            51013 => "El estudio ya tiene un apoyo profesoral registrado.",
            547 => "El estudio seleccionado no existe.",
            2627 or 2601 => "El estudio ya tiene un apoyo profesoral registrado.",
            _ => "Conflicto al procesar el apoyo profesoral."
        };
}
