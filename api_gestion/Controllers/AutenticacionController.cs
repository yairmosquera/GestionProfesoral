using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AutenticacionController : ControllerBase
{
    private readonly ServicioAutenticacion _autenticacion;
    private readonly RepositorioUsuario _usuarios;

    public AutenticacionController(
        ServicioAutenticacion autenticacion,
        RepositorioUsuario usuarios)
    {
        _autenticacion = autenticacion;
        _usuarios = usuarios;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] InicioSesionPeticion peticion)
    {
        var resultado = await _autenticacion.IniciarSesionAsync(
            peticion.NombreUsuario,
            peticion.Contrasena);

        if (resultado is null)
            return Unauthorized(new
            {
                mensaje = "Usuario o contraseña incorrectos."
            });

        return Ok(resultado);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> MiPerfil()
    {
        var valorId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(valorId, out int id))
            return Unauthorized();

        var usuario = await _usuarios.ConsultarPorIdAsync(id);

        if (usuario is null || !usuario.Activo)
            return Unauthorized(new
            {
                mensaje = "El usuario no está habilitado."
            });

        var roles = await _usuarios.ConsultarRolesActivosAsync(id);

        return Ok(new
        {
            usuario.Id,
            usuario.NombreUsuario,
            usuario.Correo,
            usuario.NombreCompleto,
            Roles = roles
        });
    }
}

public sealed class InicioSesionPeticion
{
    [Required]
    [StringLength(80)]
    public string NombreUsuario { get; set; } = "";

    [Required]
    public string Contrasena { get; set; } = "";
}
