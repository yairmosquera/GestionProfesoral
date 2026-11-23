using ApiGestion.Peticiones;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/becas")]
[Authorize(Roles = "ADMIN,GESTOR,CONSULTA")]
public class BecasController : ControllerBase
{
    private readonly IServicioBeca _servicio;

    public BecasController(IServicioBeca servicio)
    {
        _servicio = servicio;
    }

    // ADMIN, GESTOR y CONSULTA pueden consultar.
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _servicio.Listar());

    [HttpGet("{estudios:int}")]
    public async Task<IActionResult> Consultar(int estudios)
    {
        var beca = await _servicio.Consultar(estudios);
        return beca is null
            ? NotFound(new { mensaje = "No se encontró la beca activa." })
            : Ok(beca);
    }

    // Solo ADMIN y GESTOR pueden crear, modificar o retirar.
    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] BecaPeticion peticion)
    {
        try
        {
            var beca = await _servicio.Crear(peticion);
            return beca is null
                ? StatusCode(500, new { mensaje = "No fue posible registrar la beca." })
                : CreatedAtAction(nameof(Consultar), new { estudios = beca.Estudios }, beca);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPut("{estudios:int}")]
    public async Task<IActionResult> Actualizar(
        int estudios,
        [FromBody] BecaPeticion peticion)
    {
        if (peticion.Estudios != estudios)
            return BadRequest(new { mensaje = "El estudio del cuerpo no coincide con la URL." });

        try
        {
            var beca = await _servicio.Actualizar(estudios, peticion);
            return beca is null
                ? NotFound(new { mensaje = "No se encontró la beca activa." })
                : Ok(beca);
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpDelete("{estudios:int}")]
    public async Task<IActionResult> Eliminar(int estudios)
    {
        try
        {
            return await _servicio.Eliminar(estudios)
                ? NoContent()
                : NotFound(new { mensaje = "No se encontró la beca activa." });
        }
        catch (SqlException ex) when (EsConflicto(ex))
        {
            return Conflict(new { mensaje = MensajeConflicto(ex) });
        }
    }

    private static bool EsConflicto(SqlException ex)
        => ex.Number is 51014 or 51015 or 547 or 2627 or 2601;

    private static string MensajeConflicto(SqlException ex)
        => ex.Number switch
        {
            51014 => "La fecha de fin no puede ser anterior a la fecha de inicio.",
            51015 => "El estudio ya tiene una beca registrada.",
            547 => "El estudio seleccionado no existe o las fechas no son válidas.",
            2627 or 2601 => "El estudio ya tiene una beca registrada.",
            _ => "Conflicto al procesar la beca."
        };
}