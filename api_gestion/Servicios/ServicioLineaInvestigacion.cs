using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para Línea de Investigación.
/// </summary>
public class ServicioLineaInvestigacion : IServicioLineaInvestigacion
{
    private readonly IRepositorioLineaInvestigacion _repositorio;

    public ServicioLineaInvestigacion(IRepositorioLineaInvestigacion repositorio)
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
            throw new NoEncontradoExcepcion($"No se encontró la línea de investigación con el ID: {id}");
        }
        return linea;
    }

    public async Task<int> CrearAsync(LineaInvestigacionCrear dto)
    {
        var nuevaLinea = new LineaInvestigacion
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Activo = true
        };

        return await _repositorio.CrearAsync(nuevaLinea);
    }

    public async Task ReemplazarAsync(int id, LineaInvestigacionReemplazo dto)
    {
        // Verificar que el registro exista antes de reemplazar
        _ = await ObtenerPorIdAsync(id);

        var lineaActualizada = new LineaInvestigacion
        {
            Id = id,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Activo = true
        };

        await _repositorio.ActualizarAsync(lineaActualizada);
    }

    public async Task ActualizarParcialAsync(int id, LineaInvestigacionActualizar dto)
    {
        // Obtener la entidad actual para actualizar solo los campos proporcionados (PATCH)
        var lineaExistente = await ObtenerPorIdAsync(id);

        if (dto.Nombre is not null)
        {
            lineaExistente.Nombre = dto.Nombre;
        }

        if (dto.Descripcion is not null)
        {
            lineaExistente.Descripcion = dto.Descripcion;
        }

        await _repositorio.ActualizarAsync(lineaExistente);
    }

    public async Task EliminarAsync(int id)
    {
        _ = await ObtenerPorIdAsync(id);
        await _repositorio.EliminarLogicoAsync(id);
    }
}