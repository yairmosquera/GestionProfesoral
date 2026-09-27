using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

/// <summary>
/// Contrato para la lógica de negocio de Línea de Investigación.
/// </summary>
public interface IServicioLineaInvestigacion
{
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync();
    Task<LineaInvestigacion> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(LineaInvestigacionCrear dto);
    Task ReemplazarAsync(int id, LineaInvestigacionReemplazo dto);
    Task ActualizarParcialAsync(int id, LineaInvestigacionActualizar dto);
    Task EliminarAsync(int id);
}