using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Término Clave usando Dapper.
/// </summary>
public interface IRepositorioTerminoClave
{
    /// <summary>
    /// Obtiene todos los términos clave activos.
    /// </summary>
    Task<IEnumerable<TerminoClave>> ObtenerTodosAsync();

    /// <summary>
    /// Obtiene un término clave por su nombre/clave primaria si está activo.
    /// </summary>
    Task<TerminoClave?> ObtenerPorTerminoAsync(string termino);

    /// <summary>
    /// Crea un nuevo término clave en la base de datos.
    /// </summary>
    Task<bool> CrearAsync(TerminoClave terminoClave);

    /// <summary>
    /// Actualiza los datos de un término clave existente.
    /// </summary>
    Task<bool> ActualizarAsync(TerminoClave terminoClave);

    /// <summary>
    /// Realiza un borrado lógico estableciendo activo = 0.
    /// </summary>
    Task<bool> EliminarLogicoAsync(string termino);
}