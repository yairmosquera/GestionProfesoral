using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioLineaInvestigacion
{
    Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync();

    Task<LineaInvestigacion?> ObtenerPorIdAsync(int id);

    Task<int> CrearAsync(LineaInvestigacion linea);

    Task<bool> ActualizarAsync(LineaInvestigacion linea);

    Task<bool> EliminarLogicoAsync(int id);
}