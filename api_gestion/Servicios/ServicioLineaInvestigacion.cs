using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio de Línea de Investigación.
/// </summary>
public class ServicioLineaInvestigacion : IServicioLineaInvestigacion
{
    private readonly IRepositorioLineaInvestigacion _repositorio;

    public ServicioLineaInvestigacion(
        IRepositorioLineaInvestigacion repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodasAsync()
    {
        return await _repositorio.ObtenerTodasAsync();
    }

    public async Task<LineaInvestigacion> ObtenerPorIdAsync(int id)
    {
        var linea = await _repositorio.ObtenerPorIdAsync(id);

        if (linea is null)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la línea de investigación con ID {id}."
            );
        }

        return linea;
    }

    public async Task<LineaInvestigacion> CrearAsync(
        LineaInvestigacionCrear peticion)
    {
        var linea = new LineaInvestigacion
        {
            Nombre = peticion.Nombre,
            Descripcion = peticion.Descripcion,
            Activo = true
        };

        linea.Id = await _repositorio.CrearAsync(linea);

        return linea;
    }

    public async Task<LineaInvestigacion> ReemplazarAsync(
        int id,
        LineaInvestigacionReemplazo peticion)
    {
        var linea = await ObtenerPorIdAsync(id);

        linea.Nombre = peticion.Nombre;
        linea.Descripcion = peticion.Descripcion;

        var actualizado =
            await _repositorio.ActualizarAsync(linea);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la línea de investigación con ID {id}."
            );
        }

        return linea;
    }

    public async Task<LineaInvestigacion> ActualizarAsync(
        int id,
        LineaInvestigacionActualizar peticion)
    {
        var linea = await ObtenerPorIdAsync(id);

        if (peticion.Nombre is not null)
            linea.Nombre = peticion.Nombre;

        if (peticion.Descripcion is not null)
            linea.Descripcion = peticion.Descripcion;

        var actualizado =
            await _repositorio.ActualizarAsync(linea);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la línea de investigación con ID {id}."
            );
        }

        return linea;
    }

    public async Task EliminarAsync(int id)
    {
        var eliminado =
            await _repositorio.EliminarLogicoAsync(id);

        if (!eliminado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró la línea de investigación con ID {id}."
            );
        }
    }
}