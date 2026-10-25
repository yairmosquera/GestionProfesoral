using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioInteresesFuturos
{
    Task<IEnumerable<InteresFuturo>> Listar();
    Task<InteresFuturo?> Consultar(int docente, string terminoClave);
    Task<IEnumerable<InteresFuturo>> PorDocente(int docente);
    Task<InteresFuturo?> Crear(InteresFuturoPeticion peticion);
    Task<bool> Eliminar(int docente, string terminoClave);
}
