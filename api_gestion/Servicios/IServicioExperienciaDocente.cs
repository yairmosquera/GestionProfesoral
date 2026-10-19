
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioExperienciaDocente
{
    Task<IEnumerable<ExperienciaDocente>> Listar();

    Task<ExperienciaDocente?> Consultar(int id);

    Task<IEnumerable<ExperienciaDocente>> PorDocente(int cedula);

    Task<ExperienciaDocente?> Crear(
        ExperienciaDocentePeticion peticion);

    Task<ExperienciaDocente?> Actualizar(
        int id,
        ExperienciaDocentePeticion peticion);

    Task<bool> Eliminar(int id);
}
