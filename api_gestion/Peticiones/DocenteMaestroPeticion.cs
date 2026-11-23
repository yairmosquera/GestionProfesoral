
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

// Recibe el docente y sus detalles en una sola petición.
public class DocenteMaestroPeticion
{
    [Required(ErrorMessage = "Los datos del docente son obligatorios.")]
    public DocentePeticion Docente { get; set; } = new();

    public List<EstudioMaestroPeticion> Estudios { get; set; } = new();

    public List<EvaluacionMaestroPeticion> Evaluaciones { get; set; } = new();

    public List<ExperienciaMaestroPeticion> Experiencias { get; set; } = new();

    public List<ReconocimientoMaestroPeticion> Reconocimientos { get; set; } = new();
}

// DETALLE 1: Estudios realizados
public class EstudioMaestroPeticion
{
    [Required, StringLength(45)]
    public string Titulo { get; set; } = "";

    [Required, StringLength(50)]
    public string Universidad { get; set; } = "";

    [Required]
    public DateTime? Fecha { get; set; }

    [Required, StringLength(45)]
    public string Tipo { get; set; } = "";

    [Required, StringLength(45)]
    public string Ciudad { get; set; } = "";

    [Range(0, 1)]
    public byte InsAcreditada { get; set; }

    [Required, StringLength(45)]
    public string Metodologia { get; set; } = "";

    [Required]
    public string PerfilEgresado { get; set; } = "";

    [Required, StringLength(45)]
    public string Pais { get; set; } = "";
}

// DETALLE 2: Evaluaciones docentes
public class EvaluacionMaestroPeticion
{
    [Required, StringLength(45)]
    public string Semestre { get; set; } = "";

    [Range(0, 5, ErrorMessage = "La calificación debe estar entre 0 y 5.")]
    public float Calificacion { get; set; }
}

// DETALLE 3: Experiencia profesional
public class ExperienciaMaestroPeticion : IValidatableObject
{
    [Required, StringLength(45)]
    public string NombreCargo { get; set; } = "";

    [Required, StringLength(45)]
    public string Institucion { get; set; } = "";

    [Required, StringLength(45)]
    public string Tipo { get; set; } = "";

    [Required]
    public DateTime? FechaInicio { get; set; }

    // Puede ser nula si continúa trabajando.
    public DateTime? FechaFin { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (FechaInicio.HasValue && FechaFin.HasValue &&
            FechaFin.Value.Date < FechaInicio.Value.Date)
        {
            yield return new ValidationResult(
                "La fecha final no puede ser anterior a la fecha inicial.",
                new[] { nameof(FechaFin) });
        }
    }
}

// DETALLE 4: Reconocimientos
public class ReconocimientoMaestroPeticion
{
    [Required, StringLength(45)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(45)]
    public string Institucion { get; set; } = "";

    [Required, StringLength(45)]
    public string Tipo { get; set; } = "";

    [Required, StringLength(45)]
    public string Ambito { get; set; } = "";

    [Required]
    public DateTime? Fecha { get; set; }
}
