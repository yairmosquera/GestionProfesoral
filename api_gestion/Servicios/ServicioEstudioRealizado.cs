
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioEstudioRealizado
    : IServicioEstudioRealizado
{
    private readonly IRepositorioEstudioRealizado _repositorio;

    public ServicioEstudioRealizado(
        IRepositorioEstudioRealizado repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<EstudioRealizado>> ObtenerTodosAsync()
    {
        return _repositorio.ObtenerTodosAsync();
    }

    public Task<EstudioRealizado?> ObtenerPorIdAsync(int id)
    {
        return _repositorio.ObtenerPorIdAsync(id);
    }

    public Task<IEnumerable<EstudioRealizado>> ObtenerPorDocenteAsync(
        int cedula)
    {
        return _repositorio.ObtenerPorDocenteAsync(cedula);
    }

    public Task<EstudioRealizado> CrearAsync(
        EstudioRealizadoCrearPeticion peticion)
    {
        var estudio = Convertir(peticion);
        return _repositorio.CrearAsync(estudio);
    }

    public Task<EstudioRealizado?> ActualizarAsync(
        int id,
        EstudioRealizadoPeticion peticion)
    {
        var estudio = Convertir(peticion);
        estudio.Id = id;

        return _repositorio.ActualizarAsync(estudio);
    }

    public Task<bool> EliminarAsync(int id)
    {
        return _repositorio.EliminarAsync(id);
    }

    private static EstudioRealizado Convertir(
        EstudioRealizadoCrearPeticion p)
    {
        return new EstudioRealizado
        {
            Titulo = p.Titulo.Trim(),
            Universidad = p.Universidad.Trim(),
            Fecha = p.Fecha,
            Tipo = p.Tipo.Trim(),
            Ciudad = p.Ciudad.Trim(),
            Docente = p.Docente,
            InsAcreditada = p.InsAcreditada,
            Metodologia = p.Metodologia.Trim(),
            PerfilEgresado = p.PerfilEgresado.Trim(),
            Pais = p.Pais.Trim(),
            Activo = true
        };
    }
}
