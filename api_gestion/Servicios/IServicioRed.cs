using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioRed
{
    Task<IEnumerable<Red>> ObtenerTodasAsync();

    Task<Red> ObtenerPorIdAsync(int id);

    Task<Red> CrearAsync(RedCrear peticion);

    Task<Red> ReemplazarAsync(
        int id,
        RedReemplazo peticion
    );

    Task<Red> ActualizarAsync(
        int id,
        RedActualizar peticion
    );

    Task EliminarAsync(int id);
}