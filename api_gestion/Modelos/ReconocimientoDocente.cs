
namespace ApiGestion.Modelos;

public class ReconocimientoDocente
{
    public int Id { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string Institucion { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Ambito { get; set; } = string.Empty;

    public int Docente { get; set; }

    public bool Activo { get; set; } = true;
}
