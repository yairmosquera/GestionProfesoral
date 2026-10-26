using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioApoyoProfesoral
{
    Task<IEnumerable<ApoyoProfesoral>> Listar();
    Task<ApoyoProfesoral?> Consultar(int estudios);
    Task<ApoyoProfesoral?> Crear(ApoyoProfesoralPeticion peticion);
    Task<ApoyoProfesoral?> Actualizar(int estudios, ApoyoProfesoralPeticion peticion);
    Task<bool> Eliminar(int estudios);
}
