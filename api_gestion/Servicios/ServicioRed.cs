using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para Red.
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

    public async Task<Red> ObtenerPorIdAsync(int idr)
    {
        var red = await _repositorio.ObtenerPorIdAsync(idr);
        if (red is null)
        {
            throw new NoEncontradoExcepcion($"No se encontró la red con el ID: {idr}");
        }
        return red;
    }

    public async Task CrearAsync(RedCrear dto)
    {
        var nuevaRed = new Red
        {
            Idr = dto.Idr,
            Nombre = dto.Nombre,
            Url = dto.Url,
            Pais = dto.Pais,
            Activo = true
        };

        await _repositorio.CrearAsync(nuevaRed);
    }

    public async Task ReemplazarAsync(int idr, RedReemplazo dto)
    {
        _ = await ObtenerPorIdAsync(idr);

        var redActualizada = new Red
        {
            Idr = idr,
            Nombre = dto.Nombre,
            Url = dto.Url,
            Pais = dto.Pais,
            Activo = true
        };

        await _repositorio.ActualizarAsync(redActualizada);
    }

    public async Task ActualizarParcialAsync(int idr, RedActualizar dto)
    {
        var redExistente = await ObtenerPorIdAsync(idr);

        if (dto.Nombre is not null)
        {
            redExistente.Nombre = dto.Nombre;
        }

        if (dto.Url is not null)
        {
            redExistente.Url = dto.Url;
        }

        if (dto.Pais is not null)
        {
            redExistente.Pais = dto.Pais;
        }

        await _repositorio.ActualizarAsync(redExistente);
    }

    public async Task EliminarAsync(int idr)
    {
        _ = await ObtenerPorIdAsync(idr);
        await _repositorio.EliminarLogicoAsync(idr);
    }
}