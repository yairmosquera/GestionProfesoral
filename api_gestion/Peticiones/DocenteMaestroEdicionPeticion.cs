
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiGestion.Peticiones;

// ============================================
// MODELO DE EDICIÓN MAESTRO - DETALLE
// ============================================

public class DocenteMaestroEdicionPeticion
{
    [Required]
    public DocentePeticion Docente { get; set; } = new();

    // Registros existentes y nuevos
    public List<EstudioEdicionPeticion> Estudios
    { get; set; } = new();

    public List<EvaluacionEdicionPeticion> Evaluaciones
    { get; set; } = new();

    public List<ExperienciaEdicionPeticion> Experiencias
    { get; set; } = new();

    public List<ReconocimientoEdicionPeticion> Reconocimientos
    { get; set; } = new();

    // Identificadores de registros que se retirarán
    public List<int> EstudiosPorRetirar
    { get; set; } = new();

    public List<int> EvaluacionesPorRetirar
    { get; set; } = new();

    public List<int> ExperienciasPorRetirar
    { get; set; } = new();

    public List<int> ReconocimientosPorRetirar
    { get; set; } = new();
}

// ============================================
// ESTUDIOS REALIZADOS
// ============================================

public class EstudioEdicionPeticion : EstudioMaestroPeticion
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }
}

// ============================================
// EVALUACIONES DOCENTES
// ============================================

public class EvaluacionEdicionPeticion : EvaluacionMaestroPeticion
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }
}

// ============================================
// EXPERIENCIAS PROFESIONALES
// ============================================

public class ExperienciaEdicionPeticion : ExperienciaMaestroPeticion
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }
}

// ============================================
// RECONOCIMIENTOS
// ============================================

public class ReconocimientoEdicionPeticion
    : ReconocimientoMaestroPeticion
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }
}
