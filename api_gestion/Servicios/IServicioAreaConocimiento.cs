using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

/// <summary>
/// Interfaz para las operaciones de negocio de Área de Conocimiento.
/// </summary>
public interface IServicioAreaConocimiento
{
    Task<IEnumerable<AreaConocimiento>> ObtenerTodasAsync();

    Task<AreaConocimiento> ObtenerPorIdAsync(string id);

    Task<AreaConocimiento> CrearAsync(AreaConocimientoCrear peticion);

    Task<AreaConocimiento> ReemplazarAsync(
        string id,
        AreaConocimientoReemplazo peticion
    );

    Task<AreaConocimiento> ActualizarAsync(
        string id,
        AreaConocimientoActualizar peticion
    );

    Task EliminarAsync(string id);
}