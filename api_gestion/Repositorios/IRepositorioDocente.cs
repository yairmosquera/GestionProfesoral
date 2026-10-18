
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioDocente
{
    Task<IEnumerable<Docente>> ObtenerTodosAsync();
    Task<Docente?> ObtenerPorIdAsync(int cedula);
    Task<Docente> CrearAsync(Docente docente);
    Task<Docente?> ActualizarAsync(Docente docente);
    Task<bool> EliminarAsync(int cedula);
}
