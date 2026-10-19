
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioReconocimientoDocente
    : IServicioReconocimientoDocente
{
    private readonly IRepositorioReconocimientoDocente _repositorio;

    public ServicioReconocimientoDocente(
        IRepositorioReconocimientoDocente repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<ReconocimientoDocente>> Listar()
    {
        return _repositorio.Listar();
    }

    public Task<ReconocimientoDocente?> Consultar(int id)
    {
        return _repositorio.Consultar(id);
    }

    public Task<IEnumerable<ReconocimientoDocente>> PorDocente(int cedula)
    {
        return _repositorio.PorDocente(cedula);
    }

    public Task<ReconocimientoDocente?> Crear(
        ReconocimientoDocentePeticion peticion)
    {
        var reconocimiento = ConstruirModelo(peticion);

        return _repositorio.Crear(reconocimiento);
    }

    public Task<ReconocimientoDocente?> Actualizar(
        int id,
        ReconocimientoDocentePeticion peticion)
    {
        var reconocimiento = ConstruirModelo(peticion);
        reconocimiento.Id = id;

        return _repositorio.Actualizar(reconocimiento);
    }

    public Task<bool> Eliminar(int id)
    {
        return _repositorio.Eliminar(id);
    }

    private static ReconocimientoDocente ConstruirModelo(
        ReconocimientoDocentePeticion peticion)
    {
        return new ReconocimientoDocente
        {
            Tipo = peticion.Tipo.Trim(),
            Fecha = peticion.Fecha.Date,
            Institucion = peticion.Institucion.Trim(),
            Nombre = peticion.Nombre.Trim(),
            Ambito = peticion.Ambito.Trim(),
            Docente = peticion.Docente,
            Activo = true
        };
    }
}
