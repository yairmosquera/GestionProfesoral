
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ReconocimientoDocentePeticion : IValidatableObject
{
    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(45, ErrorMessage = "El tipo no puede superar 45 caracteres.")]
    public string Tipo { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La institución es obligatoria.")]
    [StringLength(45, ErrorMessage = "La institución no puede superar 45 caracteres.")]
    public string Institucion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(45, ErrorMessage = "El nombre no puede superar 45 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ámbito es obligatorio.")]
    [StringLength(45, ErrorMessage = "El ámbito no puede superar 45 caracteres.")]
    public string Ambito { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un docente válido.")]
    public int Docente { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Fecha == default)
        {
            yield return new ValidationResult(
                "La fecha del reconocimiento es obligatoria.",
                new[] { nameof(Fecha) });
        }

        if (string.IsNullOrWhiteSpace(Tipo))
            yield return Error("El tipo es obligatorio.", nameof(Tipo));

        if (string.IsNullOrWhiteSpace(Institucion))
            yield return Error("La institución es obligatoria.", nameof(Institucion));

        if (string.IsNullOrWhiteSpace(Nombre))
            yield return Error("El nombre es obligatorio.", nameof(Nombre));

        if (string.IsNullOrWhiteSpace(Ambito))
            yield return Error("El ámbito es obligatorio.", nameof(Ambito));
    }

    private static ValidationResult Error(string texto, string campo)
        => new(texto, new[] { campo });
}
