
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioEstudioRealizado
{
    Task<IEnumerable<EstudioRealizado>> ObtenerTodosAsync();

    Task<EstudioRealizado?> ObtenerPorIdAsync(int id);

    Task<IEnumerable<EstudioRealizado>> ObtenerPorDocenteAsync(
        int cedula);

    Task<EstudioRealizado> CrearAsync(
        EstudioRealizadoCrearPeticion peticion);

    Task<EstudioRealizado?> ActualizarAsync(
        int id,
        EstudioRealizadoPeticion peticion);

    Task<bool> EliminarAsync(int id);
}
