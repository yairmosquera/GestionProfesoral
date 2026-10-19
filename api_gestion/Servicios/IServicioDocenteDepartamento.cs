
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioDocenteDepartamento
{
    Task<IEnumerable<DocenteDepartamento>> Listar();

    Task<DocenteDepartamento?> Consultar(
        int docente, int departamento);

    Task<IEnumerable<DocenteDepartamento>> PorDocente(int cedula);

    Task<DocenteDepartamento?> Crear(
        DocenteDepartamentoPeticion peticion);

    Task<DocenteDepartamento?> Actualizar(
        int docente,
        int departamento,
        DocenteDepartamentoPeticion peticion);

    Task<bool> Eliminar(int docente, int departamento);
}
