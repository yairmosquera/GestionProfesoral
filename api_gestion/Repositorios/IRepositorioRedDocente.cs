
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioRedDocente
{
    Task<IEnumerable<RedDocente>> Listar();

    Task<RedDocente?> Consultar(int red, int docente);

    Task<IEnumerable<RedDocente>> PorDocente(int cedula);

    Task<RedDocente?> Crear(RedDocente relacion);

    Task<RedDocente?> Actualizar(RedDocente relacion);

    Task<bool> Eliminar(int red, int docente);
}
