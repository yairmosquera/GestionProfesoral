using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio de Área de Conocimiento.
/// </summary>
public class ServicioAreaConocimiento : IServicioAreaConocimiento
{
    private readonly IRepositorioAreaConocimiento _repositorio;

    public ServicioAreaConocimiento(IRepositorioAreaConocimiento repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<AreaConocimiento>> ObtenerTodasAsync()
    {
        return await _repositorio.ObtenerTodasAsync();
    }

    public async Task<AreaConocimiento> ObtenerPorIdAsync(string id)
    {
        var areaConocimiento = await _repositorio.ObtenerPorIdAsync(id);

        if (areaConocimiento is null)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el área de conocimiento con ID {id}."
            );
        }

        return areaConocimiento;
    }

    public async Task<AreaConocimiento> CrearAsync(AreaConocimientoCrear peticion)
    {
        var areaConocimiento = new AreaConocimiento
        {
            Id = peticion.Id,
            GranArea = peticion.GranArea,
            Area = peticion.Area,
            Disciplina = peticion.Disciplina,
            Activo = true
        };

        areaConocimiento.Id = await _repositorio.CrearAsync(areaConocimiento);

        return areaConocimiento;
    }

    public async Task<AreaConocimiento> ReemplazarAsync(
        string id,
        AreaConocimientoReemplazo peticion)
    {
        var areaConocimiento = await ObtenerPorIdAsync(id);

        areaConocimiento.GranArea = peticion.GranArea;
        areaConocimiento.Area = peticion.Area;
        areaConocimiento.Disciplina = peticion.Disciplina;

        var actualizado = await _repositorio.ActualizarAsync(areaConocimiento);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el área de conocimiento con ID {id}."
            );
        }

        return areaConocimiento;
    }

    public async Task<AreaConocimiento> ActualizarAsync(
        string id,
        AreaConocimientoActualizar peticion)
    {
        var areaConocimiento = await ObtenerPorIdAsync(id);

        if (peticion.GranArea is not null)
            areaConocimiento.GranArea = peticion.GranArea;

        if (peticion.Area is not null)
            areaConocimiento.Area = peticion.Area;

        if (peticion.Disciplina is not null)
            areaConocimiento.Disciplina = peticion.Disciplina;

        var actualizado = await _repositorio.ActualizarAsync(areaConocimiento);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el área de conocimiento con ID {id}."
            );
        }

        return areaConocimiento;
    }

    public async Task EliminarAsync(string id)
    {
        var eliminado = await _repositorio.EliminarLogicoAsync(id);

        if (!eliminado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el área de conocimiento con ID {id}."
            );
        }
    }
}