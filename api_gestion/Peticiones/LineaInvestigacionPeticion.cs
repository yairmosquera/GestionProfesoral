using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// DTO para la creación de una línea de investigación (POST).
/// </summary>
public class LineaInvestigacionCrear
{
    /// <summary>
    /// Nombre de la línea de investigación (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la línea de investigación (máximo 256 caracteres).
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(256, ErrorMessage = "La descripción no puede superar los 256 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para el reemplazo completo de una línea de investigación (PUT).
/// Exige todos los campos para garantizar la integridad de los datos.
/// </summary>
public class LineaInvestigacionReemplazo
{
    /// <summary>
    /// Nombre de la línea de investigación (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio para reemplazo completo.")]
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la línea de investigación (máximo 256 caracteres).
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria para reemplazo completo.")]
    [StringLength(256, ErrorMessage = "La descripción no puede superar los 256 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para la actualización parcial de una línea de investigación (PATCH).
/// Todos los campos son opcionales.
/// </summary>
public class LineaInvestigacionActualizar
{
    /// <summary>
    /// Nuevo nombre para la línea de investigación (opcional).
    /// </summary>
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string? Nombre { get; set; }

    /// <summary>
    /// Nueva descripción para la línea de investigación (opcional).
    /// </summary>
    [StringLength(256, ErrorMessage = "La descripción no puede superar los 256 caracteres.")]
    public string? Descripcion { get; set; }
}