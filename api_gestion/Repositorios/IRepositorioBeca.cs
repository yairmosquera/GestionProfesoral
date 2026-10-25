using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioBeca
{
    Task<IEnumerable<Beca>> Listar();
    Task<Beca?> Consultar(int estudios);
    Task<Beca?> Crear(Beca beca);
    Task<Beca?> Actualizar(Beca beca);
    Task<bool> Eliminar(int estudios);
}
