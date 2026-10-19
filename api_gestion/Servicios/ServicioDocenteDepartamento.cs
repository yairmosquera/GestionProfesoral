
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioDocenteDepartamento
    : IServicioDocenteDepartamento
{
    private readonly IRepositorioDocenteDepartamento _repositorio;

    public ServicioDocenteDepartamento(
        IRepositorioDocenteDepartamento repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<DocenteDepartamento>> Listar()
    {
        return _repositorio.Listar();
    }

    public Task<DocenteDepartamento?> Consultar(
        int docente, int departamento)
    {
        return _repositorio.Consultar(docente, departamento);
    }

    public Task<IEnumerable<DocenteDepartamento>> PorDocente(
        int cedula)
    {
        return _repositorio.PorDocente(cedula);
    }

    public Task<DocenteDepartamento?> Crear(
        DocenteDepartamentoPeticion peticion)
    {
        var relacion = ConstruirModelo(peticion);

        return _repositorio.Crear(relacion);
    }

    public Task<DocenteDepartamento?> Actualizar(
        int docente,
        int departamento,
        DocenteDepartamentoPeticion peticion)
    {
        var relacion = ConstruirModelo(peticion);

        relacion.Docente = docente;
        relacion.Departamento = departamento;

        return _repositorio.Actualizar(relacion);
    }

    public Task<bool> Eliminar(int docente, int departamento)
    {
        return _repositorio.Eliminar(docente, departamento);
    }

    private static DocenteDepartamento ConstruirModelo(
        DocenteDepartamentoPeticion peticion)
    {
        return new DocenteDepartamento
        {
            Docente = peticion.Docente,
            Departamento = peticion.Departamento,
            Dedicacion = peticion.Dedicacion.Trim(),
            Modalidad = peticion.Modalidad.Trim(),
            FechaIngreso = peticion.FechaIngreso.Date,
            FechaSalida = peticion.FechaSalida?.Date,
            Activo = true
        };
    }
}
