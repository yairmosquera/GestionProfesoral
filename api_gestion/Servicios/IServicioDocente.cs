
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioDocente
{
    Task<IEnumerable<Docente>> ObtenerTodosAsync();
    Task<Docente?> ObtenerPorIdAsync(int cedula);
    Task<Docente> CrearAsync(DocentePeticion peticion);
    Task<Docente?> ActualizarAsync(
        int cedula, DocentePeticion peticion);
    Task<bool> EliminarAsync(int cedula);
}
