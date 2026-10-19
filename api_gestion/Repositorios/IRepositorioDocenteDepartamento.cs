
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioDocenteDepartamento
{
    Task<IEnumerable<DocenteDepartamento>> Listar();

    Task<DocenteDepartamento?> Consultar(
        int docente, int departamento);

    Task<IEnumerable<DocenteDepartamento>> PorDocente(
        int cedula);

    Task<DocenteDepartamento?> Crear(
        DocenteDepartamento relacion);

    Task<DocenteDepartamento?> Actualizar(
        DocenteDepartamento relacion);

    Task<bool> Eliminar(int docente, int departamento);
}
