
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioExperienciaDocente
    : IServicioExperienciaDocente
{
    private readonly IRepositorioExperienciaDocente _repositorio;

    public ServicioExperienciaDocente(
        IRepositorioExperienciaDocente repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<ExperienciaDocente>> Listar()
    {
        return _repositorio.Listar();
    }

    public Task<ExperienciaDocente?> Consultar(int id)
    {
        return _repositorio.Consultar(id);
    }

    public Task<IEnumerable<ExperienciaDocente>> PorDocente(
        int cedula)
    {
        return _repositorio.PorDocente(cedula);
    }

    public Task<ExperienciaDocente?> Crear(
        ExperienciaDocentePeticion peticion)
    {
        var experiencia = ConstruirModelo(peticion);

        return _repositorio.Crear(experiencia);
    }

    public Task<ExperienciaDocente?> Actualizar(
        int id,
        ExperienciaDocentePeticion peticion)
    {
        var experiencia = ConstruirModelo(peticion);

        experiencia.Id = id;

        return _repositorio.Actualizar(experiencia);
    }

    public Task<bool> Eliminar(int id)
    {
        return _repositorio.Eliminar(id);
    }

    private static ExperienciaDocente ConstruirModelo(
        ExperienciaDocentePeticion peticion)
    {
        return new ExperienciaDocente
        {
            NombreCargo = peticion.NombreCargo.Trim(),
            Institucion = peticion.Institucion.Trim(),
            Tipo = peticion.Tipo.Trim(),
            FechaInicio = peticion.FechaInicio.Date,
            FechaFin = peticion.FechaFin?.Date,
            Docente = peticion.Docente,
            Activo = true
        };
    }
}
