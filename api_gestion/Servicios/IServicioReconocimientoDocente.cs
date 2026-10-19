
using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioReconocimientoDocente
{
    Task<IEnumerable<ReconocimientoDocente>> Listar();

    Task<ReconocimientoDocente?> Consultar(int id);

    Task<IEnumerable<ReconocimientoDocente>> PorDocente(int cedula);

    Task<ReconocimientoDocente?> Crear(
        ReconocimientoDocentePeticion peticion);

    Task<ReconocimientoDocente?> Actualizar(
        int id,
        ReconocimientoDocentePeticion peticion);

    Task<bool> Eliminar(int id);
}
