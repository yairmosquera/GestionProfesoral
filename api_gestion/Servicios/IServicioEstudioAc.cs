using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioEstudioAc
{
    Task<IEnumerable<EstudioAc>> Listar();
    Task<EstudioAc?> Consultar(int estudio, string areaConocimiento);
    Task<IEnumerable<EstudioAc>> AreasPorEstudio(int estudio);
    Task<EstudioAc?> Crear(EstudioAcPeticion peticion);
    Task<bool> Eliminar(int estudio, string areaConocimiento);
}
