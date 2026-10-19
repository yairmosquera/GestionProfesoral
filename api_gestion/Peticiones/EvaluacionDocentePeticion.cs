
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EvaluacionDocentePeticion
{
    [Required]
    public float Calificacion { get; set; }

    [Required]
    [StringLength(45)]
    public string Semestre { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Docente { get; set; }
}
