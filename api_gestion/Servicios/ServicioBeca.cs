using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioBeca : IServicioBeca
{
    private readonly IRepositorioBeca _repositorio;

    public ServicioBeca(IRepositorioBeca repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<Beca>> Listar() => _repositorio.Listar();

    public Task<Beca?> Consultar(int estudios) => _repositorio.Consultar(estudios);

    public Task<Beca?> Crear(BecaPeticion peticion)
        => _repositorio.Crear(ConstruirModelo(peticion));

    public Task<Beca?> Actualizar(int estudios, BecaPeticion peticion)
    {
        var beca = ConstruirModelo(peticion);
        beca.Estudios = estudios;
        return _repositorio.Actualizar(beca);
    }

    public Task<bool> Eliminar(int estudios) => _repositorio.Eliminar(estudios);

    private static Beca ConstruirModelo(BecaPeticion peticion) => new()
    {
        Estudios = peticion.Estudios,
        Tipo = peticion.Tipo.Trim(),
        Institucion = peticion.Institucion.Trim(),
        FechaInicio = peticion.FechaInicio.Date,
        FechaFin = peticion.FechaFin?.Date,
        Activo = true
    };
}
