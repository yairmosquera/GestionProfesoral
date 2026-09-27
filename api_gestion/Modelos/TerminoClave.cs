namespace ApiGestion.Modelos;

public class TerminoClave
{
    public string Termino { get; set; } = string.Empty;
    public string? TerminoIngles { get; set; }
    public bool Activo { get; set; } = true;
}