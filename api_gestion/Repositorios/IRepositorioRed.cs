using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioRed
{
    Task<IEnumerable<Red>> ObtenerTodasAsync();

    Task<Red?> ObtenerPorIdAsync(int id);

    Task<int> CrearAsync(Red red);

    Task<bool> ActualizarAsync(Red red);

    Task<bool> EliminarLogicoAsync(int id);
}