
namespace ApiGestion.Modelos;

public class DocenteDepartamento
{
    public int Docente { get; set; }

    public int Departamento { get; set; }

    public string Dedicacion { get; set; } = string.Empty;

    public string Modalidad { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public bool Activo { get; set; } = true;
}
