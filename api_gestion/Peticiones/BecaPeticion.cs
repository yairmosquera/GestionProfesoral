using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class BecaPeticion : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estudio válido.")]
    public int Estudios { get; set; }

    [Required(ErrorMessage = "El tipo de beca es obligatorio.")]
    [StringLength(45, ErrorMessage = "El tipo de beca admite máximo 45 caracteres.")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La institución es obligatoria.")]
    [StringLength(80, ErrorMessage = "La institución admite máximo 80 caracteres.")]
    public string Institucion { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (FechaInicio == default)
        {
            yield return new ValidationResult(
                "La fecha de inicio es obligatoria.",
                new[] { nameof(FechaInicio) });
        }

        if (FechaFin.HasValue && FechaInicio != default &&
            FechaFin.Value.Date < FechaInicio.Date)
        {
            yield return new ValidationResult(
                "La fecha de fin no puede ser anterior a la fecha de inicio.",
                new[] { nameof(FechaFin) });
        }
    }
}
