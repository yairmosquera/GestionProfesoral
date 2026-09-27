using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Área de Conocimiento usando Dapper.
/// </summary>
public interface IRepositorioAreaConocimiento
{
    /// <summary>
    /// Obtiene todas las áreas de conocimiento activas.
    /// </summary>
    Task<IEnumerable<AreaConocimiento>> ObtenerTodasAsync();

    /// <summary>
    /// Obtiene un área de conocimiento por su ID si está activa.
    /// </summary>
    Task<AreaConocimiento?> ObtenerPorIdAsync(string id);

    /// <summary>
    /// Crea una nueva área de conocimiento.
    /// </summary>
    Task<string> CrearAsync(AreaConocimiento area);

    /// <summary>
    /// Actualiza un área de conocimiento existente.
    /// </summary>
    Task<bool> ActualizarAsync(AreaConocimiento area);

    /// <summary>
    /// Realiza un borrado lógico estableciendo activo = 0.
    /// </summary>
    Task<bool> EliminarLogicoAsync(string id);
}