using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Red usando Dapper.
/// </summary>
public interface IRepositorioRed
{
    /// <summary>
    /// Obtiene todas las redes activas.
    /// </summary>
    Task<IEnumerable<Red>> ObtenerTodasAsync();

    /// <summary>
    /// Obtiene una red por su ID (idr) si está activa.
    /// </summary>
    Task<Red?> ObtenerPorIdAsync(int idr);

    /// <summary>
    /// Crea una nueva red en la base de datos.
    /// </summary>
    Task<bool> CrearAsync(Red red);

    /// <summary>
    /// Actualiza los datos de una red existente.
    /// </summary>
    Task<bool> ActualizarAsync(Red red);

    /// <summary>
    /// Realiza un borrado lógico estableciendo activo = 0.
    /// </summary>
    Task<bool> EliminarLogicoAsync(int idr);
}