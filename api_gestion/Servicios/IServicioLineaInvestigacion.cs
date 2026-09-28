using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioLineaInvestigacion
{
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync();

    Task<LineaInvestigacion> ObtenerPorIdAsync(int id);

    Task<LineaInvestigacion> CrearAsync(
        LineaInvestigacionCrear peticion
    );

    Task<LineaInvestigacion> ReemplazarAsync(
        int id,
        LineaInvestigacionReemplazo peticion
    );

    Task<LineaInvestigacion> ActualizarAsync(
        int id,
        LineaInvestigacionActualizar peticion
    );

    Task EliminarAsync(int id);
}