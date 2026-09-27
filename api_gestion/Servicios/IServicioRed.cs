using ApiGestion.Modelos;
using ApiGestion.Peticiones;

namespace ApiGestion.Servicios;

/// <summary>
/// Contrato para la lógica de negocio de Red.
/// </summary>
public interface IServicioRed
{
    Task<IEnumerable<Red>> ObtenerTodasAsync();
    Task<Red> ObtenerPorIdAsync(int idr);
    Task CrearAsync(RedCrear dto);
    Task ReemplazarAsync(int idr, RedReemplazo dto);
    Task ActualizarParcialAsync(int idr, RedActualizar dto);
    Task EliminarAsync(int idr);
}