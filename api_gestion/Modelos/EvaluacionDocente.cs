
namespace ApiGestion.Modelos;

public class EvaluacionDocente
{
    public int Id { get; set; }
    public float Calificacion { get; set; }
    public string Semestre { get; set; } = "";
    public int Docente { get; set; }
    public bool Activo { get; set; } = true;
}
