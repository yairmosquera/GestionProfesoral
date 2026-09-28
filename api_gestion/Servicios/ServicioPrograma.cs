using ApiGestion.Excepciones;
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

/// <summary>
/// Implementación de la lógica de negocio de Programa.
/// </summary>
public class ServicioPrograma : IServicioPrograma
{
    private readonly IRepositorioPrograma _repositorio;

    public ServicioPrograma(IRepositorioPrograma repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Programa>> ObtenerTodasAsync()
    {
        return await _repositorio.ObtenerTodasAsync();
    }

    public async Task<Programa> ObtenerPorIdAsync(int id)
    {
        var programa = await _repositorio.ObtenerPorIdAsync(id);

        if (programa is null)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el programa con ID {id}."
            );
        }

        return programa;
    }

    public async Task<Programa> CrearAsync(ProgramaCrear peticion)
    {
        var programa = new Programa
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            Tipo = peticion.Tipo,
            Nivel = peticion.Nivel,
            FechaCreacion = peticion.FechaCreacion,
            FechaCierre = peticion.FechaCierre,
            NumeroCohortes = peticion.NumeroCohortes,
            CantGraduados = peticion.CantGraduados,
            FechaActualizacion = peticion.FechaActualizacion,
            Ciudad = peticion.Ciudad,
            Facultad = peticion.Facultad,
            Activo = true
        };

        programa.Id = await _repositorio.CrearAsync(programa);

        return programa;
    }

    public async Task<Programa> ReemplazarAsync(
        int id,
        ProgramaReemplazo peticion)
    {
        var programa = await ObtenerPorIdAsync(id);

        programa.Nombre = peticion.Nombre;
        programa.Tipo = peticion.Tipo;
        programa.Nivel = peticion.Nivel;
        programa.FechaCreacion = peticion.FechaCreacion;
        programa.FechaCierre = peticion.FechaCierre;
        programa.NumeroCohortes = peticion.NumeroCohortes;
        programa.CantGraduados = peticion.CantGraduados;
        programa.FechaActualizacion = peticion.FechaActualizacion;
        programa.Ciudad = peticion.Ciudad;
        programa.Facultad = peticion.Facultad;

        var actualizado = await _repositorio.ActualizarAsync(programa);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el programa con ID {id}."
            );
        }

        return programa;
    }

    public async Task<Programa> ActualizarAsync(
        int id,
        ProgramaActualizar peticion)
    {
        var programa = await ObtenerPorIdAsync(id);

        if (peticion.Nombre is not null)
            programa.Nombre = peticion.Nombre;

        if (peticion.Tipo is not null)
            programa.Tipo = peticion.Tipo;

        if (peticion.Nivel is not null)
            programa.Nivel = peticion.Nivel;

        if (peticion.FechaCreacion is not null)
            programa.FechaCreacion = peticion.FechaCreacion;

        if (peticion.FechaCierre is not null)
            programa.FechaCierre = peticion.FechaCierre;

        if (peticion.NumeroCohortes is not null)
            programa.NumeroCohortes = peticion.NumeroCohortes;

        if (peticion.CantGraduados is not null)
            programa.CantGraduados = peticion.CantGraduados;

        if (peticion.FechaActualizacion is not null)
            programa.FechaActualizacion = peticion.FechaActualizacion;

        if (peticion.Ciudad is not null)
            programa.Ciudad = peticion.Ciudad;

        if (peticion.Facultad.HasValue)
            programa.Facultad = peticion.Facultad.Value;

        var actualizado = await _repositorio.ActualizarAsync(programa);

        if (!actualizado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el programa con ID {id}."
            );
        }

        return programa;
    }

    public async Task EliminarAsync(int id)
    {
        var eliminado = await _repositorio.EliminarLogicoAsync(id);

        if (!eliminado)
        {
            throw new NoEncontradoExcepcion(
                $"No se encontró el programa con ID {id}."
            );
        }
    }
}