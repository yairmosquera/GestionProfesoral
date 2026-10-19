
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioExperienciaDocente
{
    Task<IEnumerable<ExperienciaDocente>> Listar();

    Task<ExperienciaDocente?> Consultar(int id);

    Task<IEnumerable<ExperienciaDocente>> PorDocente(int cedula);

    Task<ExperienciaDocente?> Crear(ExperienciaDocente experiencia);

    Task<ExperienciaDocente?> Actualizar(ExperienciaDocente experiencia);

    Task<bool> Eliminar(int id);
}
