using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Programa usando Dapper.
/// </summary>
public interface IRepositorioPrograma
{
    /// <summary>
    /// Obtiene todos los programas activos.
    /// </summary>
    Task<IEnumerable<Programa>> ObtenerTodasAsync();

    /// <summary>
    /// Obtiene un programa por su ID si está activo.
    /// </summary>
    Task<Programa?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Crea un nuevo programa.
    /// </summary>
    Task<int> CrearAsync(Programa programa);

    /// <summary>
    /// Actualiza un programa existente.
    /// </summary>
    Task<bool> ActualizarAsync(Programa programa);

    /// <summary>
    /// Realiza un borrado lógico estableciendo activo = 0.
    /// </summary>
    Task<bool> EliminarLogicoAsync(int id);
}