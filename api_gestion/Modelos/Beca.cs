namespace ApiGestion.Modelos;

public class Beca
{
    public int Estudios { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;
}
