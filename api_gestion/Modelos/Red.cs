namespace ApiGestion.Modelos;

public class Red
{
    public int Idr { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}