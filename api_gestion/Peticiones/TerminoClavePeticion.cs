using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// DTO para la creación de un término clave (POST).
/// </summary>
public class TerminoClaveCrear
{
    /// <summary>
    /// Término clave en español (máximo 30 caracteres). Funciona como clave primaria.
    /// </summary>
    [Required(ErrorMessage = "El término es obligatorio.")]
    [StringLength(30, ErrorMessage = "El término no puede superar los 30 caracteres.")]
    public string Termino { get; set; } = string.Empty;

    /// <summary>
    /// Traducción o equivalente del término en inglés (opcional, máximo 30 caracteres).
    /// </summary>
    [StringLength(30, ErrorMessage = "El término en inglés no puede superar los 30 caracteres.")]
    public string? TerminoIngles { get; set; }
}

/// <summary>
/// DTO para el reemplazo completo de un término clave (PUT).
/// </summary>
public class TerminoClaveReemplazo
{
    /// <summary>
    /// Traducción o equivalente del término en inglés (opcional, máximo 30 caracteres).
    /// </summary>
    [StringLength(30, ErrorMessage = "El término en inglés no puede superar los 30 caracteres.")]
    public string? TerminoIngles { get; set; }
}

/// <summary>
/// DTO para la actualización parcial de un término clave (PATCH).
/// </summary>
public class TerminoClaveActualizar
{
    /// <summary>
    /// Nueva traducción o equivalente del término en inglés (opcional).
    /// </summary>
    [StringLength(30, ErrorMessage = "El término en inglés no puede superar los 30 caracteres.")]
    public string? TerminoIngles { get; set; }
}