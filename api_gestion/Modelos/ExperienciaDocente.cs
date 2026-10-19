
namespace ApiGestion.Modelos;

public class ExperienciaDocente
{
    public int Id { get; set; }

    public string NombreCargo { get; set; } = string.Empty;

    public string Institucion { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int Docente { get; set; }

    public bool Activo { get; set; } = true;
}
