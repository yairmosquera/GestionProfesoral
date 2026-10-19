
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ExperienciaDocentePeticion : IValidatableObject
{
    [Required(ErrorMessage = "El nombre del cargo es obligatorio.")]
    [StringLength(45)]
    public string NombreCargo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La institución es obligatoria.")]
    [StringLength(45)]
    public string Institucion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(45)]
    public string Tipo { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un docente válido.")]
    public int Docente { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(NombreCargo))
        {
            yield return new ValidationResult(
                "El nombre del cargo es obligatorio.",
                new[] { nameof(NombreCargo) });
        }

        if (string.IsNullOrWhiteSpace(Institucion))
        {
            yield return new ValidationResult(
                "La institución es obligatoria.",
                new[] { nameof(Institucion) });
        }

        if (string.IsNullOrWhiteSpace(Tipo))
        {
            yield return new ValidationResult(
                "El tipo es obligatorio.",
                new[] { nameof(Tipo) });
        }

        if (FechaInicio == default)
        {
            yield return new ValidationResult(
                "La fecha de inicio es obligatoria.",
                new[] { nameof(FechaInicio) });
        }

        if (FechaFin.HasValue && FechaFin.Value.Date < FechaInicio.Date)
        {
            yield return new ValidationResult(
                "La fecha de finalización no puede ser anterior a la de inicio.",
                new[] { nameof(FechaFin) });
        }
    }
}
