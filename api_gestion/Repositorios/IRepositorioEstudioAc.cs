using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioEstudioAc
{
    Task<IEnumerable<EstudioAc>> Listar();
    Task<EstudioAc?> Consultar(int estudio, string areaConocimiento);
    Task<IEnumerable<EstudioAc>> AreasPorEstudio(int estudio);
    Task<EstudioAc?> Crear(EstudioAc relacion);
    Task<bool> Eliminar(int estudio, string areaConocimiento);
}
