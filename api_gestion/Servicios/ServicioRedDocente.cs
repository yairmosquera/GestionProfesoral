
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioRedDocente : IServicioRedDocente
{
    private readonly IRepositorioRedDocente _repositorio;

    public ServicioRedDocente(
        IRepositorioRedDocente repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<RedDocente>> Listar()
    {
        return _repositorio.Listar();
    }

    public Task<RedDocente?> Consultar(int red, int docente)
    {
        return _repositorio.Consultar(red, docente);
    }

    public Task<IEnumerable<RedDocente>> PorDocente(int cedula)
    {
        return _repositorio.PorDocente(cedula);
    }

    public Task<RedDocente?> Crear(RedDocentePeticion peticion)
    {
        var relacion = ConstruirModelo(peticion);

        return _repositorio.Crear(relacion);
    }

    public Task<RedDocente?> Actualizar(
        int red,
        int docente,
        RedDocentePeticion peticion)
    {
        var relacion = ConstruirModelo(peticion);

        relacion.Red = red;
        relacion.Docente = docente;

        return _repositorio.Actualizar(relacion);
    }

    public Task<bool> Eliminar(int red, int docente)
    {
        return _repositorio.Eliminar(red, docente);
    }

    private static RedDocente ConstruirModelo(
        RedDocentePeticion peticion)
    {
        return new RedDocente
        {
            Red = peticion.Red,
            Docente = peticion.Docente,
            FechaInicio = peticion.FechaInicio.Date,
            FechaFin = string.IsNullOrWhiteSpace(peticion.FechaFin)
                ? null
                : peticion.FechaFin.Trim(),
            ActDestacadas = peticion.ActDestacadas,
            Activo = true
        };
    }
}
