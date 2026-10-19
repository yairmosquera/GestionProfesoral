
namespace ApiGestion.Modelos;

public class EstudioRealizado
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Universidad { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public int Docente { get; set; }
    public byte InsAcreditada { get; set; }
    public string Metodologia { get; set; } = "";
    public string PerfilEgresado { get; set; } = "";
    public string Pais { get; set; } = "";
    public bool Activo { get; set; } = true;
}
