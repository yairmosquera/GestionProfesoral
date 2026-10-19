
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioEstudioRealizado
{
    Task<IEnumerable<EstudioRealizado>> ObtenerTodosAsync();

    Task<EstudioRealizado?> ObtenerPorIdAsync(int id);

    Task<IEnumerable<EstudioRealizado>> ObtenerPorDocenteAsync(
        int cedula);

    Task<EstudioRealizado> CrearAsync(
        EstudioRealizado estudio);

    Task<EstudioRealizado?> ActualizarAsync(
        EstudioRealizado estudio);

    Task<bool> EliminarAsync(int id);
}
