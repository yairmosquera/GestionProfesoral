using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Línea de Investigación usando Dapper.
/// </summary>
public interface IRepositorioLineaInvestigacion
{
    /// <summary>
    /// Obtiene todas las líneas de investigación activas.
    /// </summary>
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync();

    /// <summary>
    /// Obtiene una línea de investigación por su ID (si está activa).
    /// </summary>
    Task<LineaInvestigacion?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Crea una nueva línea de investigación en la base de datos.
    /// </summary>
    Task<int> CrearAsync(LineaInvestigacion linea);

    /// <summary>
    /// Actualiza los datos de una línea de investigación existente.
    /// </summary>
    Task<bool> ActualizarAsync(LineaInvestigacion linea);

    /// <summary>
    /// Realiza un borrado lógico estableciendo activo = 0.
    /// </summary>
    Task<bool> EliminarLogicoAsync(int id);
}