using System.ComponentModel.DataAnnotations;
using ApiGestion.Modelos;
using ApiGestion.Repositorios;
using ApiGestion.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

/// <summary>
/// Registro de usuarios nuevos. Solo un administrador autenticado
/// puede utilizar este endpoint; no existe registro público.
/// </summary>
[ApiController]
[Route("api/administracion-usuarios")]
[Authorize(Roles = "ADMIN")]
public sealed class RegistroUsuariosController : ControllerBase
{
    private readonly RepositorioUsuario _usuarios;
    private readonly ServicioPassword _passwords;

    public RegistroUsuariosController(
        RepositorioUsuario usuarios,
        ServicioPassword passwords)
    {
        _usuarios = usuarios;
        _passwords = passwords;
    }

    // POST /api/administracion-usuarios/registrar
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistroUsuarioPeticion peticion)
    {
        string nombreUsuario = peticion.NombreUsuario.Trim();
        string correo = peticion.Correo.Trim();
        string nombreCompleto = peticion.NombreCompleto.Trim();

        // Validación adicional a las anotaciones de datos.
        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(nombreCompleto))
        {
            return BadRequest(new
            {
                mensaje = "El nombre de usuario y el nombre completo son obligatorios."
            });
        }

        // Evita registrar claves demasiado débiles.
        if (peticion.Contrasena.Length < 12 ||
            !peticion.Contrasena.Any(char.IsLetter) ||
            !peticion.Contrasena.Any(char.IsDigit))
        {
            return BadRequest(new
            {
                mensaje = "La contraseña debe tener 12 caracteres como mínimo, letras y números."
            });
        }

        // Crear el hash en la API: nunca enviar contraseñas en texto plano a SQL.
        var usuarioParaHash = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Correo = correo,
            NombreCompleto = nombreCompleto
        };

        var hash = _passwords.GenerarHash(
            usuarioParaHash, peticion.Contrasena);

        try
        {
            var usuario = await _usuarios.CrearAsync(
                nombreUsuario, correo, nombreCompleto, hash);

            // Proyección explícita: evita devolver PasswordHash.
            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Usuario registrado correctamente con rol CONSULTA.",
                usuario = new
                {
                    usuario.Id,
                    usuario.NombreUsuario,
                    usuario.Correo,
                    usuario.NombreCompleto,
                    usuario.Activo,
                    usuario.FechaCreacion
                },
                rolInicial = "CONSULTA"
            });
        }
        catch (SqlException ex) when (ex.Number is 51033 or 51034 or 2601 or 2627)
        {
            string mensaje = ex.Number switch
            {
                51033 => "El nombre de usuario ya está registrado.",
                51034 => "El correo ya está registrado.",
                _ => "Ya existe un usuario con ese nombre o correo."
            };
            return Conflict(new { mensaje });
        }
        catch (SqlException ex) when (ex.Number is 51031 or 51032)
        {
            return BadRequest(new
            {
                mensaje = "Los datos del usuario no son válidos."
            });
        }
        catch (SqlException ex) when (ex.Number == 51035)
        {
            return Conflict(new
            {
                mensaje = "El rol CONSULTA no está disponible. Contacta al administrador."
            });
        }
    }
}

public sealed class RegistroUsuarioPeticion
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(80, MinimumLength = 3)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(160)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(12, ErrorMessage = "La contraseña debe tener 12 caracteres como mínimo.")]
    public string Contrasena { get; set; } = string.Empty;
}