using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio de Red.
/// </summary>
public class ServicioRed : IServicioRed
{
    private readonly IRepositorioRed _repositorio;

    public ServicioRed(IRepositorioRed repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Red>> ObtenerTodasAsync()
    {
        return await _repositorio.ObtenerTodasAsync();
    }

    public async Task<Red> ObtenerPorIdAsync(int id)
    {
        var red = await _repositorio.ObtenerPorIdAsync(id);

        if (red is null)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la red con ID {id}."
            );
        }

        return red;
    }

    public async Task<Red> CrearAsync(RedCrear peticion)
    {
        var red = new Red
        {
            Idr = peticion.Idr,
            Nombre = peticion.Nombre,
            Url = peticion.Url,
            Pais = peticion.Pais,
            Activo = true
        };

        red.Idr = await _repositorio.CrearAsync(red);

        return red;
    }

    public async Task<Red> ReemplazarAsync(
        int id,
        RedReemplazo peticion)
    {
        var red = await ObtenerPorIdAsync(id);

        red.Nombre = peticion.Nombre;
        red.Url = peticion.Url;
        red.Pais = peticion.Pais;

        var actualizado =
            await _repositorio.ActualizarAsync(red);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la red con ID {id}."
            );
        }

        return red;
    }

    public async Task<Red> ActualizarAsync(
        int id,
        RedActualizar peticion)
    {
        var red = await ObtenerPorIdAsync(id);

        if (peticion.Nombre is not null)
            red.Nombre = peticion.Nombre;

        if (peticion.Url is not null)
            red.Url = peticion.Url;

        if (peticion.Pais is not null)
            red.Pais = peticion.Pais;

        var actualizado =
            await _repositorio.ActualizarAsync(red);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la red con ID {id}."
            );
        }

        return red;
    }

    public async Task EliminarAsync(int id)
    {
        var eliminado =
            await _repositorio.EliminarLogicoAsync(id);

        if (!eliminado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la red con ID {id}."
            );
        }
    }
}