using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

/// <summary>
/// DTO para la creación de un programa (POST).
/// </summary>
public class ProgramaCrear
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(45, ErrorMessage = "El tipo no puede superar los 45 caracteres.")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel es obligatorio.")]
    [StringLength(45, ErrorMessage = "El nivel no puede superar los 45 caracteres.")]
    public string Nivel { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
    [StringLength(45, ErrorMessage = "La fecha de creación no puede superar los 45 caracteres.")]
    public string FechaCreacion { get; set; } = string.Empty;

    [StringLength(45, ErrorMessage = "La fecha de cierre no puede superar los 45 caracteres.")]
    public string? FechaCierre { get; set; }

    [Required(ErrorMessage = "El número de cohortes es obligatorio.")]
    [StringLength(45, ErrorMessage = "El número de cohortes no puede superar los 45 caracteres.")]
    public string NumeroCohortes { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad de graduados es obligatoria.")]
    [StringLength(45, ErrorMessage = "La cantidad de graduados no puede superar los 45 caracteres.")]
    public string CantGraduados { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de actualización es obligatoria.")]
    [StringLength(45, ErrorMessage = "La fecha de actualización no puede superar los 45 caracteres.")]
    public string FechaActualizacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(45, ErrorMessage = "La ciudad no puede superar los 45 caracteres.")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La facultad es obligatoria.")]
    public int Facultad { get; set; }
}

/// <summary>
/// DTO para el reemplazo completo de un programa (PUT).
/// Exige todos los campos obligatorios.
/// </summary>
public class ProgramaReemplazo
{
    [Required(ErrorMessage = "El nombre es obligatorio para reemplazo completo.")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es obligatorio para reemplazo completo.")]
    [StringLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel es obligatorio para reemplazo completo.")]
    [StringLength(45)]
    public string Nivel { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de creación es obligatoria para reemplazo completo.")]
    [StringLength(45)]
    public string FechaCreacion { get; set; } = string.Empty;

    [StringLength(45)]
    public string? FechaCierre { get; set; }

    [Required(ErrorMessage = "El número de cohortes es obligatorio para reemplazo completo.")]
    [StringLength(45)]
    public string NumeroCohortes { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad de graduados es obligatoria para reemplazo completo.")]
    [StringLength(45)]
    public string CantGraduados { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de actualización es obligatoria para reemplazo completo.")]
    [StringLength(45)]
    public string FechaActualizacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria para reemplazo completo.")]
    [StringLength(45)]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La facultad es obligatoria para reemplazo completo.")]
    public int Facultad { get; set; }
}

/// <summary>
/// DTO para la actualización parcial de un programa (PATCH).
/// Todos los campos son opcionales.
/// </summary>
public class ProgramaActualizar
{
    [StringLength(60)]
    public string? Nombre { get; set; }

    [StringLength(45)]
    public string? Tipo { get; set; }

    [StringLength(45)]
    public string? Nivel { get; set; }

    [StringLength(45)]
    public string? FechaCreacion { get; set; }

    [StringLength(45)]
    public string? FechaCierre { get; set; }

    [StringLength(45)]
    public string? NumeroCohortes { get; set; }

    [StringLength(45)]
    public string? CantGraduados { get; set; }

    [StringLength(45)]
    public string? FechaActualizacion { get; set; }

    [StringLength(45)]
    public string? Ciudad { get; set; }

    public int? Facultad { get; set; }
}