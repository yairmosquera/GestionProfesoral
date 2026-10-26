using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class ApoyoProfesoralPeticion
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estudio válido.")]
    public int Estudios { get; set; }

    [Range(0, 1, ErrorMessage = "El indicador de apoyo debe ser 0 o 1.")]
    public byte ConApoyo { get; set; }

    [Required(ErrorMessage = "La institución es obligatoria.")]
    [StringLength(45, ErrorMessage = "La institución admite máximo 45 caracteres.")]
    public string Institucion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de apoyo es obligatorio.")]
    [StringLength(45, ErrorMessage = "El tipo de apoyo admite máximo 45 caracteres.")]
    public string Tipo { get; set; } = string.Empty;
}
