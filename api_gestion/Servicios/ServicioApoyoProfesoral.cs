using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioApoyoProfesoral : IServicioApoyoProfesoral
{
    private readonly IRepositorioApoyoProfesoral _repositorio;

    public ServicioApoyoProfesoral(IRepositorioApoyoProfesoral repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<ApoyoProfesoral>> Listar() => _repositorio.Listar();

    public Task<ApoyoProfesoral?> Consultar(int estudios)
        => _repositorio.Consultar(estudios);

    public Task<ApoyoProfesoral?> Crear(ApoyoProfesoralPeticion peticion)
        => _repositorio.Crear(ConstruirModelo(peticion));

    public Task<ApoyoProfesoral?> Actualizar(
        int estudios, ApoyoProfesoralPeticion peticion)
    {
        var apoyo = ConstruirModelo(peticion);
        apoyo.Estudios = estudios;
        return _repositorio.Actualizar(apoyo);
    }

    public Task<bool> Eliminar(int estudios) => _repositorio.Eliminar(estudios);

    private static ApoyoProfesoral ConstruirModelo(
        ApoyoProfesoralPeticion peticion) => new()
    {
        Estudios = peticion.Estudios,
        ConApoyo = peticion.ConApoyo,
        Institucion = peticion.Institucion.Trim(),
        Tipo = peticion.Tipo.Trim(),
        Activo = true
    };
}
