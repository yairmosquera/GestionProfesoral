using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

/// <summary>
/// Interfaz para las operaciones de acceso a datos de Área de Conocimiento usando Dapper.
/// </summary>
public interface IRepositorioAreaConocimiento
{
    Task<IEnumerable<AreaConocimiento>> ObtenerTodasAsync();
    Task<AreaConocimiento?> ObtenerPorIdAsync(string id);
    Task<string> CrearAsync(AreaConocimiento areaConocimiento);
    Task<bool> ActualizarAsync(AreaConocimiento areaConocimiento);
    Task<bool> EliminarLogicoAsync(string id);
}