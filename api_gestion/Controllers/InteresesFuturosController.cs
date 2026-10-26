using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/intereses-futuros")]
public class InteresesFuturosController : ControllerBase
{
    private readonly IServicioInteresesFuturos _servicio;

    public InteresesFuturosController(IServicioInteresesFuturos servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _servicio.Listar());

    [HttpGet("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> Consultar(int docente, string terminoClave)
    {
        var interes = await _servicio.Consultar(docente, terminoClave);
        return interes is null
            ? NotFound(new { mensaje = "No se encontró el interés futuro activo." })
            : Ok(interes);
    }

    [HttpGet("docente/{docente:int}")]
    public async Task<IActionResult> PorDocente(int docente)
        => Ok(await _servicio.PorDocente(docente));

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] InteresFuturoPeticion peticion)
    {
        try
        {
            var interes = await _servicio.Crear(peticion);
            return interes is null
                ? StatusCode(500, new { mensaje = "No fue posible registrar el interés futuro." })
                : CreatedAtAction(
                    nameof(Consultar),
                    new { docente = interes.Docente, terminoClave = interes.TerminoClave },
                    interes);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [HttpPut("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> Actualizar(
        int docente,
        string terminoClave,
        [FromBody] InteresFuturoPeticion peticion)
    {
        try
        {
            var interes = await _servicio.Actualizar(docente, terminoClave, peticion);
            return interes is null
                ? NotFound(new { mensaje = "No se encontró el interés futuro activo." })
                : Ok(interes);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [HttpDelete("{docente:int}/{terminoClave}")]
    public async Task<IActionResult> Eliminar(int docente, string terminoClave)
    {
        try
        {
            return await _servicio.Eliminar(docente, terminoClave)
                ? NoContent()
                : NotFound(new { mensaje = "No se encontró el interés futuro activo." });
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    private static bool EsConflicto(SqlException ex)
        => ex.Number is 51010 or 51016 or 547 or 2627 or 2601;

    private static string MensajeConflicto(SqlException ex)
        => ex.Number switch
        {
            51010 => "El docente ya tiene registrado ese término clave.",
            51016 => "No se encontró el interés futuro activo para actualizar.",
            547 => "El docente o el término clave seleccionado no existe.",
            2627 or 2601 => "Ya existe un interés futuro con esa clave compuesta.",
            _ => "Conflicto al procesar el interés futuro."
        };
}
