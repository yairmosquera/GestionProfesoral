using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioInteresesFuturos : IServicioInteresesFuturos
{
    private readonly IRepositorioInteresesFuturos _repositorio;

    public ServicioInteresesFuturos(IRepositorioInteresesFuturos repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<InteresFuturo>> Listar() => _repositorio.Listar();

    public Task<InteresFuturo?> Consultar(int docente, string terminoClave)
        => _repositorio.Consultar(docente, terminoClave);

    public Task<IEnumerable<InteresFuturo>> PorDocente(int docente)
        => _repositorio.PorDocente(docente);

    public Task<InteresFuturo?> Crear(InteresFuturoPeticion peticion)
        => _repositorio.Crear(new InteresFuturo
        {
            Docente = peticion.Docente,
            TerminoClave = peticion.TerminoClave.Trim(),
            Activo = true
        });

    public Task<InteresFuturo?> Actualizar(
        int docenteOriginal,
        string terminoClaveOriginal,
        InteresFuturoPeticion peticion)
        => _repositorio.Actualizar(
            docenteOriginal,
            terminoClaveOriginal,
            new InteresFuturo
            {
                Docente = peticion.Docente,
                TerminoClave = peticion.TerminoClave.Trim(),
                Activo = true
            });

    public Task<bool> Eliminar(int docente, string terminoClave)
        => _repositorio.Eliminar(docente, terminoClave);
}
