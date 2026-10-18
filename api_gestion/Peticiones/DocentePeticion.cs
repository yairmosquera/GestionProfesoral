
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

public class DocentePeticion
{
    [Range(1, int.MaxValue)]
    public int Cedula { get; set; }

    [Required, StringLength(60)]
    public string Nombres { get; set; } = "";

    [Required, StringLength(60)]
    public string Apellidos { get; set; } = "";

    [Required, StringLength(12)]
    public string Genero { get; set; } = "";

    [Required, StringLength(30)]
    public string Cargo { get; set; } = "";

    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Required, EmailAddress, StringLength(70)]
    public string Correo { get; set; } = "";

    [Required, StringLength(20)]
    public string Telefono { get; set; } = "";

    [Required, StringLength(128)]
    public string UrlCvlac { get; set; } = "";

    [Required, StringLength(45)]
    public string Escalafon { get; set; } = "";

    [Required]
    public string Perfil { get; set; } = "";

    [StringLength(45)]
    public string? CatMinciencia { get; set; }

    [Required, StringLength(45)]
    public string ConvMinciencia { get; set; } = "";

    [Required, StringLength(45)]
    public string Nacionalidaad { get; set; } = "";

    public int? LineaInvestigacionPrincipal { get; set; }
}
