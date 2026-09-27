using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

/// <summary>
/// Contrato para la lógica de negocio de Término Clave.
/// </summary>
public interface IServicioTerminoClave
{
    Task<IEnumerable<TerminoClave>> ObtenerTodosAsync();
    Task<TerminoClave> ObtenerPorTerminoAsync(string termino);
    Task CrearAsync(TerminoClaveCrear dto);
    Task ReemplazarAsync(string termino, TerminoClaveReemplazo dto);
    Task ActualizarParcialAsync(string termino, TerminoClaveActualizar dto);
    Task EliminarAsync(string termino);
}