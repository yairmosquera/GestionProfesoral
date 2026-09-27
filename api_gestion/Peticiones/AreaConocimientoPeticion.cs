using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// DTO para la creación de un área de conocimiento (POST).
/// </summary>
public class AreaConocimientoCrear
{
    [Required(ErrorMessage = "El identificador es obligatorio.")]
    [StringLength(6, ErrorMessage = "El identificador no puede superar los 6 caracteres.")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "La disciplina es obligatoria.")]
    [StringLength(150, ErrorMessage = "La disciplina no puede superar los 150 caracteres.")]
    public string Disciplina { get; set; } = string.Empty;
}

/// <summary>
/// DTO para el reemplazo completo de un área de conocimiento (PUT).
/// </summary>
public class AreaConocimientoReemplazo
{
    [Required(ErrorMessage = "La disciplina es obligatoria para reemplazo completo.")]
    [StringLength(150, ErrorMessage = "La disciplina no puede superar los 150 caracteres.")]
    public string Disciplina { get; set; } = string.Empty;
}

/// <summary>
/// DTO para la actualización parcial de un área de conocimiento (PATCH).
/// Todos los campos son opcionales.
/// </summary>
public class AreaConocimientoActualizar
{
    [StringLength(150, ErrorMessage = "La disciplina no puede superar los 150 caracteres.")]
    public string? Disciplina { get; set; }
}