using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EstudioAcPeticion
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estudio válido.")]
    public int Estudio { get; set; }

    [Required(ErrorMessage = "El área de conocimiento es obligatoria.")]
    [StringLength(6, ErrorMessage = "El área de conocimiento admite máximo 6 caracteres.")]
    public string AreaConocimiento { get; set; } = string.Empty;
}
