
namespace ApiGestion.Modelos;

/// <summary>
/// Representa un registro de dbo.usuarios.
/// Nunca devolver PasswordHash en una respuesta pública de la API.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    // Uso interno: nunca almacenar la contraseña original.
    public string PasswordHash { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }
}
