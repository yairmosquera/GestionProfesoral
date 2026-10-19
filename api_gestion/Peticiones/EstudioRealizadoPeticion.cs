
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class EstudioRealizadoCrearPeticion
{
    [Required, StringLength(45)]
    public string Titulo { get; set; } = "";

    [Required, StringLength(50)]
    public string Universidad { get; set; } = "";

    public DateTime Fecha { get; set; }

    [Required, StringLength(45)]
    public string Tipo { get; set; } = "";

    [Required, StringLength(45)]
    public string Ciudad { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Docente { get; set; }

    [Range(0, 1)]
    public byte InsAcreditada { get; set; }

    [Required, StringLength(45)]
    public string Metodologia { get; set; } = "";

    [Required]
    public string PerfilEgresado { get; set; } = "";

    [Required, StringLength(45)]
    public string Pais { get; set; } = "";
}

public class EstudioRealizadoPeticion
    : EstudioRealizadoCrearPeticion
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }
}
