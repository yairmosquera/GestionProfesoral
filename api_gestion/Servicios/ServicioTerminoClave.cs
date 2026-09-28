using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio de Término Clave.
/// </summary>
public class ServicioTerminoClave : IServicioTerminoClave
{
    private readonly IRepositorioTerminoClave _repositorio;

    public ServicioTerminoClave(
        IRepositorioTerminoClave repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<TerminoClave>> ObtenerTodosAsync()
    {
        return await _repositorio.ObtenerTodosAsync();
    }

    public async Task<TerminoClave> ObtenerPorTerminoAsync(string termino)
    {
        var terminoClave =
            await _repositorio.ObtenerPorTerminoAsync(termino);

        if (terminoClave is null)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el término clave '{termino}'."
            );
        }

        return terminoClave;
    }

    public async Task<TerminoClave> CrearAsync(
        TerminoClaveCrear peticion)
    {
        var terminoClave = new TerminoClave
        {
            Termino = peticion.Termino,
            TerminoIngles = peticion.TerminoIngles,
            Activo = true
        };

        terminoClave.Termino =
            await _repositorio.CrearAsync(terminoClave);

        return terminoClave;
    }

    public async Task<TerminoClave> ReemplazarAsync(
        string termino,
        TerminoClaveReemplazo peticion)
    {
        var terminoClave =
            await ObtenerPorTerminoAsync(termino);

        terminoClave.TerminoIngles =
            peticion.TerminoIngles;

        var actualizado =
            await _repositorio.ActualizarAsync(terminoClave);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el término clave '{termino}'."
            );
        }

        return terminoClave;
    }

    public async Task<TerminoClave> ActualizarAsync(
        string termino,
        TerminoClaveActualizar peticion)
    {
        var terminoClave =
            await ObtenerPorTerminoAsync(termino);

        if (peticion.TerminoIngles is not null)
        {
            terminoClave.TerminoIngles =
                peticion.TerminoIngles;
        }

        var actualizado =
            await _repositorio.ActualizarAsync(terminoClave);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el término clave '{termino}'."
            );
        }

        return terminoClave;
    }

    public async Task EliminarAsync(string termino)
    {
        var eliminado =
            await _repositorio.EliminarLogicoAsync(termino);

        if (!eliminado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el término clave '{termino}'."
            );
        }
    }
}