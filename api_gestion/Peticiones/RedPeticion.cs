using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// DTO para la creación de una red (POST).
/// </summary>
public class RedCrear
{
    /// <summary>
    /// Identificador único de la red.
    /// </summary>
    [Required(ErrorMessage = "El ID de la red es obligatorio.")]
    public int Idr { get; set; }

    /// <summary>
    /// Nombre de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Dirección URL de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "La URL es obligatoria.")]
    [StringLength(45, ErrorMessage = "La URL no puede superar los 45 caracteres.")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// País de origen de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El país es obligatorio.")]
    [StringLength(45, ErrorMessage = "El país no puede superar los 45 caracteres.")]
    public string Pais { get; set; } = string.Empty;
}

/// <summary>
/// DTO para el reemplazo completo de una red (PUT).
/// </summary>
public class RedReemplazo
{
    /// <summary>
    /// Nombre de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio para reemplazo completo.")]
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Dirección URL de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "La URL es obligatoria para reemplazo completo.")]
    [StringLength(45, ErrorMessage = "La URL no puede superar los 45 caracteres.")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// País de origen de la red (máximo 45 caracteres).
    /// </summary>
    [Required(ErrorMessage = "El país es obligatorio para reemplazo completo.")]
    [StringLength(45, ErrorMessage = "El país no puede superar los 45 caracteres.")]
    public string Pais { get; set; } = string.Empty;
}

/// <summary>
/// DTO para la actualización parcial de una red (PATCH).
/// </summary>
public class RedActualizar
{
    /// <summary>
    /// Nuevo nombre para la red (opcional).
    /// </summary>
    [StringLength(45, ErrorMessage = "El nombre no puede superar los 45 caracteres.")]
    public string? Nombre { get; set; }

    /// <summary>
    /// Nueva dirección URL para la red (opcional).
    /// </summary>
    [StringLength(45, ErrorMessage = "La URL no puede superar los 45 caracteres.")]
    public string? Url { get; set; }

    /// <summary>
    /// Nuevo país de origen para la red (opcional).
    /// </summary>
    [StringLength(45, ErrorMessage = "El país no puede superar los 45 caracteres.")]
    public string? Pais { get; set; }
}