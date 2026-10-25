using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioApoyoProfesoral
{
    Task<IEnumerable<ApoyoProfesoral>> Listar();
    Task<ApoyoProfesoral?> Consultar(int estudios);
    Task<ApoyoProfesoral?> Crear(ApoyoProfesoral apoyo);
    Task<ApoyoProfesoral?> Actualizar(ApoyoProfesoral apoyo);
    Task<bool> Eliminar(int estudios);
}
