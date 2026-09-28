using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

/// <summary>
/// Interfaz para las operaciones de negocio de Programa.
/// </summary>
public interface IServicioPrograma
{
    Task<IEnumerable<Programa>> ObtenerTodasAsync();

    Task<Programa> ObtenerPorIdAsync(int id);

    Task<Programa> CrearAsync(ProgramaCrear peticion);

    Task<Programa> ReemplazarAsync(int id, ProgramaReemplazo peticion);

    Task<Programa> ActualizarAsync(int id, ProgramaActualizar peticion);

    Task EliminarAsync(int id);
}