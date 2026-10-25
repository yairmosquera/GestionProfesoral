using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioBeca
{
    Task<IEnumerable<Beca>> Listar();
    Task<Beca?> Consultar(int estudios);
    Task<Beca?> Crear(BecaPeticion peticion);
    Task<Beca?> Actualizar(int estudios, BecaPeticion peticion);
    Task<bool> Eliminar(int estudios);
}
