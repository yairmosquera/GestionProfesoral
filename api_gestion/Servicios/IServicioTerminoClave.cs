using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

public interface IServicioTerminoClave
{
    Task<IEnumerable<TerminoClave>> ObtenerTodosAsync();

    Task<TerminoClave> ObtenerPorTerminoAsync(string termino);

    Task<TerminoClave> CrearAsync(
        TerminoClaveCrear peticion
    );

    Task<TerminoClave> ReemplazarAsync(
        string termino,
        TerminoClaveReemplazo peticion
    );

    Task<TerminoClave> ActualizarAsync(
        string termino,
        TerminoClaveActualizar peticion
    );

    Task EliminarAsync(string termino);
}