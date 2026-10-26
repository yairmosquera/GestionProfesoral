using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioEstudioAc : IServicioEstudioAc
{
    private readonly IRepositorioEstudioAc _repositorio;

    public ServicioEstudioAc(IRepositorioEstudioAc repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<EstudioAc>> Listar() => _repositorio.Listar();

    public Task<EstudioAc?> Consultar(int estudio, string areaConocimiento)
        => _repositorio.Consultar(estudio, areaConocimiento);

    public Task<IEnumerable<EstudioAc>> AreasPorEstudio(int estudio)
        => _repositorio.AreasPorEstudio(estudio);

    public Task<EstudioAc?> Crear(EstudioAcPeticion peticion)
        => _repositorio.Crear(new EstudioAc
        {
            Estudio = peticion.Estudio,
            AreaConocimiento = peticion.AreaConocimiento.Trim(),
            Activo = true
        });

    public Task<bool> Eliminar(int estudio, string areaConocimiento)
        => _repositorio.Eliminar(estudio, areaConocimiento);
}
