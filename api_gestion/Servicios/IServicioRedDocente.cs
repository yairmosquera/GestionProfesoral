
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioRedDocente
{
    Task<IEnumerable<RedDocente>> Listar();

    Task<RedDocente?> Consultar(int red, int docente);

    Task<IEnumerable<RedDocente>> PorDocente(int cedula);

    Task<RedDocente?> Crear(RedDocentePeticion peticion);

    Task<RedDocente?> Actualizar(
        int red, int docente, RedDocentePeticion peticion);

    Task<bool> Eliminar(int red, int docente);
}
