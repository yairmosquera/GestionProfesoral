using System.ComponentModel.DataAnnotations;
using ApiGestion.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

/// <summary>
/// Administración de usuarios y roles. Requiere token JWT con rol ADMIN.
/// No devuelve contraseñas ni hashes.
/// </summary>
[ApiController]
[Route("api/administracion-usuarios")]
[Authorize(Roles = "ADMIN")]
public sealed class AdministracionUsuariosController : ControllerBase
{
    private readonly RepositorioAdministracionUsuarios _repositorio;

    public AdministracionUsuariosController(
        RepositorioAdministracionUsuarios repositorio)
    {
        _repositorio = repositorio;
    }

    // GET /api/administracion-usuarios
    [HttpGet]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _repositorio.ListarUsuariosAsync();
        return Ok(usuarios);
    }

    // GET /api/administracion-usuarios/roles
    [HttpGet("roles")]
    public async Task<IActionResult> ListarRoles()
    {
        var roles = await _repositorio.ListarRolesAsync();
        return Ok(roles);
    }

    // POST /api/administracion-usuarios/{usuarioId}/roles
    // JSON: { "rolCodigo": "GESTOR" }
    [HttpPost("{usuarioId:int}/roles")]
    public async Task<IActionResult> AsignarRol(
        int usuarioId,
        [FromBody] RolUsuarioPeticion peticion)
    {
        if (usuarioId <= 0)
            return BadRequest(new { mensaje = "El usuario indicado no es válido." });

        try
        {
            await _repositorio.AsignarRolAsync(usuarioId, peticion.RolCodigo);
            return Ok(new { mensaje = "Rol asignado correctamente." });
        }
        catch (SqlException ex) when (ex.Number is 51020 or 51021 or 51022)
        {
            return Conflict(new { mensaje = MensajeSql(ex.Number) });
        }
    }

    // DELETE /api/administracion-usuarios/{usuarioId}/roles/{rolCodigo}
    [HttpDelete("{usuarioId:int}/roles/{rolCodigo}")]
    public async Task<IActionResult> RetirarRol(int usuarioId, string rolCodigo)
    {
        if (usuarioId <= 0 || string.IsNullOrWhiteSpace(rolCodigo) || rolCodigo.Length > 30)
            return BadRequest(new { mensaje = "Los datos enviados no son válidos." });

        try
        {
            await _repositorio.RetirarRolAsync(usuarioId, rolCodigo);
            return Ok(new { mensaje = "Rol retirado correctamente." });
        }
        catch (SqlException ex) when (ex.Number is 51023 or 51024 or 51025)
        {
            return Conflict(new { mensaje = MensajeSql(ex.Number) });
        }
    }

    // PATCH /api/administracion-usuarios/{usuarioId}/estado
    // JSON: { "activo": false }
    [HttpPatch("{usuarioId:int}/estado")]
    public async Task<IActionResult> CambiarEstado(
        int usuarioId,
        [FromBody] EstadoUsuarioPeticion peticion)
    {
        if (usuarioId <= 0)
            return BadRequest(new { mensaje = "El usuario indicado no es válido." });

        try
        {
            await _repositorio.CambiarEstadoAsync(usuarioId, peticion.Activo!.Value);
            return Ok(new
            {
                mensaje = peticion.Activo!.Value
                    ? "Usuario activado correctamente."
                    : "Usuario desactivado correctamente."
            });
        }
        catch (SqlException ex) when (ex.Number is 51024 or 51026 or 51027)
        {
            return Conflict(new { mensaje = MensajeSql(ex.Number) });
        }
    }

    private static string MensajeSql(int numero) => numero switch
    {
        51020 => "El usuario no existe o está inactivo.",
        51021 => "El rol no existe o está inactivo.",
        51022 => "El usuario ya tiene asignado este rol.",
        51023 => "El usuario no tiene asignado ese rol.",
        51024 => "No fue posible completar la operación. Intenta nuevamente.",
        51025 => "No se puede retirar el último administrador activo.",
        51026 => "El usuario no existe.",
        51027 => "No se puede desactivar el último administrador activo.",
        _ => "No fue posible completar la operación."
    };
}

public sealed class RolUsuarioPeticion
{
    [Required(ErrorMessage = "El rol es obligatorio.")]
    [StringLength(30, MinimumLength = 1)]
    public string RolCodigo { get; set; } = string.Empty;
}

public sealed class EstadoUsuarioPeticion
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public bool? Activo { get; set; }
}