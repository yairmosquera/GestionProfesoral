
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioEvaluacionDocente
{
    Task<IEnumerable<EvaluacionDocente>> ObtenerTodosAsync();

    Task<EvaluacionDocente?> ObtenerPorIdAsync(int id);

    Task<IEnumerable<EvaluacionDocente>> ObtenerPorDocenteAsync(
        int cedula);

    Task<EvaluacionDocente> CrearAsync(
        EvaluacionDocente evaluacion);

    Task<EvaluacionDocente?> ActualizarAsync(
        EvaluacionDocente evaluacion);

    Task<bool> EliminarAsync(int id);
}
