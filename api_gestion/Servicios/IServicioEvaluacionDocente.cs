
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioEvaluacionDocente
{
    Task<IEnumerable<EvaluacionDocente>> ObtenerTodosAsync();

    Task<EvaluacionDocente?> ObtenerPorIdAsync(int id);

    Task<IEnumerable<EvaluacionDocente>> ObtenerPorDocenteAsync(
        int cedula);

    Task<EvaluacionDocente> CrearAsync(
        EvaluacionDocentePeticion peticion);

    Task<EvaluacionDocente?> ActualizarAsync(
        int id,
        EvaluacionDocentePeticion peticion);

    Task<bool> EliminarAsync(int id);
}
