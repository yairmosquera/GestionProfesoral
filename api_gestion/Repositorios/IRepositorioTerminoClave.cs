using ApiGestion.Modelos;

namespace ApiGestion.Repositorios;

public interface IRepositorioTerminoClave
{
    Task<IEnumerable<TerminoClave>> ObtenerTodosAsync();

    Task<TerminoClave?> ObtenerPorTerminoAsync(string termino);

    Task<string> CrearAsync(TerminoClave terminoClave);

    Task<bool> ActualizarAsync(TerminoClave terminoClave);

    Task<bool> EliminarLogicoAsync(string termino);
}