
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class DocenteDepartamentoPeticion : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un docente válido.")]
    public int Docente { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un departamento válido.")]
    public int Departamento { get; set; }

    [Required(ErrorMessage = "La dedicación es obligatoria.")]
    [StringLength(15, ErrorMessage = "Máximo 15 caracteres.")]
    public string Dedicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(45, ErrorMessage = "Máximo 45 caracteres.")]
    public string Modalidad { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Dedicacion))
        {
            yield return new ValidationResult(
                "La dedicación es obligatoria.",
                new[] { nameof(Dedicacion) });
        }

        if (string.IsNullOrWhiteSpace(Modalidad))
        {
            yield return new ValidationResult(
                "La modalidad es obligatoria.",
                new[] { nameof(Modalidad) });
        }

        if (FechaIngreso == default)
        {
            yield return new ValidationResult(
                "La fecha de ingreso es obligatoria.",
                new[] { nameof(FechaIngreso) });
        }

        if (FechaSalida.HasValue &&
            FechaSalida.Value.Date < FechaIngreso.Date)
        {
            yield return new ValidationResult(
                "La fecha de salida no puede ser anterior a la fecha de ingreso.",
                new[] { nameof(FechaSalida) });
        }
    }
}
