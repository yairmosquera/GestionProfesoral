using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para Término Clave.
/// </summary>
public class ServicioTerminoClave : IServicioTerminoClave
{
    private readonly IRepositorioTerminoClave _repositorio;

    public ServicioTerminoClave(IRepositorioTerminoClave repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<TerminoClave>> ObtenerTodosAsync()
    {
        return await _repositorio.ObtenerTodosAsync();
    }

    public async Task<TerminoClave> ObtenerPorTerminoAsync(string termino)
    {
        var terminoClave = await _repositorio.ObtenerPorTerminoAsync(termino);
        if (terminoClave is null)
        {
            throw new NoEncontradoExcepcion($"No se encontró el término clave: '{termino}'");
        }
        return terminoClave;
    }

    public async Task CrearAsync(TerminoClaveCrear dto)
    {
        var nuevoTermino = new TerminoClave
        {
            Termino = dto.Termino,
            TerminoIngles = dto.TerminoIngles,
            Activo = true
        };

        await _repositorio.CrearAsync(nuevoTermino);
    }

    public async Task ReemplazarAsync(string termino, TerminoClaveReemplazo dto)
    {
        _ = await ObtenerPorTerminoAsync(termino);

        var terminoActualizado = new TerminoClave
        {
            Termino = termino,
            TerminoIngles = dto.TerminoIngles,
            Activo = true
        };

        await _repositorio.ActualizarAsync(terminoActualizado);
    }

    public async Task ActualizarParcialAsync(string termino, TerminoClaveActualizar dto)
    {
        var terminoExistente = await ObtenerPorTerminoAsync(termino);

        if (dto.TerminoIngles is not null)
        {
            terminoExistente.TerminoIngles = dto.TerminoIngles;
        }

        await _repositorio.ActualizarAsync(terminoExistente);
    }

    public async Task EliminarAsync(string termino)
    {
        _ = await ObtenerPorTerminoAsync(termino);
        await _repositorio.EliminarLogicoAsync(termino);
    }
}