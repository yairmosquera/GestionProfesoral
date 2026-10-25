using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioInteresesFuturos
{
    Task<IEnumerable<InteresFuturo>> Listar();
    Task<InteresFuturo?> Consultar(int docente, string terminoClave);
    Task<IEnumerable<InteresFuturo>> PorDocente(int docente);
    Task<InteresFuturo?> Crear(InteresFuturo interes);
    Task<bool> Eliminar(int docente, string terminoClave);
}
