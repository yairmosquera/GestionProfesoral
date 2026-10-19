
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioEvaluacionDocente
    : IServicioEvaluacionDocente
{
    private readonly IRepositorioEvaluacionDocente _repositorio;

    public ServicioEvaluacionDocente(
        IRepositorioEvaluacionDocente repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<EvaluacionDocente>>
        ObtenerTodosAsync()
    {
        return _repositorio.ObtenerTodosAsync();
    }

    public Task<EvaluacionDocente?> ObtenerPorIdAsync(int id)
    {
        return _repositorio.ObtenerPorIdAsync(id);
    }

    public Task<IEnumerable<EvaluacionDocente>>
        ObtenerPorDocenteAsync(int cedula)
    {
        return _repositorio.ObtenerPorDocenteAsync(cedula);
    }

    public Task<EvaluacionDocente> CrearAsync(
        EvaluacionDocentePeticion peticion)
    {
        Validar(peticion);

        var evaluacion = Convertir(peticion);

        return _repositorio.CrearAsync(evaluacion);
    }

    public Task<EvaluacionDocente?> ActualizarAsync(
        int id,
        EvaluacionDocentePeticion peticion)
    {
        Validar(peticion);

        var evaluacion = Convertir(peticion);
        evaluacion.Id = id;

        return _repositorio.ActualizarAsync(evaluacion);
    }

    public Task<bool> EliminarAsync(int id)
    {
        return _repositorio.EliminarAsync(id);
    }

    private static void Validar(
        EvaluacionDocentePeticion peticion)
    {
        if (string.IsNullOrWhiteSpace(peticion.Semestre))
        {
            throw new ArgumentException(
                "El semestre es obligatorio.");
        }

        if (!float.IsFinite(peticion.Calificacion))
        {
            throw new ArgumentException(
                "La calificación debe ser un número válido.");
        }
    }

    private static EvaluacionDocente Convertir(
        EvaluacionDocentePeticion peticion)
    {
        return new EvaluacionDocente
        {
            Calificacion = peticion.Calificacion,
            Semestre = peticion.Semestre.Trim(),
            Docente = peticion.Docente,
            Activo = true
        };
    }
}
