
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class RedDocentePeticion : IValidatableObject
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Seleccione una red válida.")]
    public int Red { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Seleccione un docente válido.")]
    public int Docente { get; set; }

    public DateTime FechaInicio { get; set; }

    [StringLength(45,
        ErrorMessage = "La fecha de finalización no puede superar 45 caracteres.")]
    public string? FechaFin { get; set; }

    public string? ActDestacadas { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (FechaInicio == default)
        {
            yield return new ValidationResult(
                "La fecha de inicio es obligatoria.",
                new[] { nameof(FechaInicio) });
        }
    }
}
