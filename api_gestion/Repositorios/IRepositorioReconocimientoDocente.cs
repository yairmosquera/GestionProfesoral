
using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioReconocimientoDocente
{
    Task<IEnumerable<ReconocimientoDocente>> Listar();

    Task<ReconocimientoDocente?> Consultar(int id);

    Task<IEnumerable<ReconocimientoDocente>> PorDocente(int cedula);

    Task<ReconocimientoDocente?> Crear(ReconocimientoDocente reconocimiento);

    Task<ReconocimientoDocente?> Actualizar(ReconocimientoDocente reconocimiento);

    Task<bool> Eliminar(int id);
}
