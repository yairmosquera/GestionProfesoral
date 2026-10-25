using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class InteresFuturoPeticion
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un docente válido.")]
    public int Docente { get; set; }

    [Required(ErrorMessage = "El término clave es obligatorio.")]
    [StringLength(30, ErrorMessage = "El término clave admite máximo 30 caracteres.")]
    public string TerminoClave { get; set; } = string.Empty;
}
