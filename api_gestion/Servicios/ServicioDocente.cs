
using ApiGestion.Modelos;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;

namespace ApiGestion.Servicios;

public class ServicioDocente : IServicioDocente
{
    private readonly IRepositorioDocente _repositorio;

    public ServicioDocente(IRepositorioDocente repositorio)
    {
        _repositorio = repositorio;
    }

    // Obtener todos los docentes activos
    public Task<IEnumerable<Docente>> ObtenerTodosAsync()
    {
        return _repositorio.ObtenerTodosAsync();
    }

    // Obtener docente por cédula
    public Task<Docente?> ObtenerPorIdAsync(int cedula)
    {
        return _repositorio.ObtenerPorIdAsync(cedula);
    }

    // Crear docente
    public async Task<Docente> CrearAsync(
        DocentePeticion peticion)
    {
        var docente = Convertir(peticion);

        return await _repositorio.CrearAsync(docente);
    }

    // Actualizar docente
    public async Task<Docente?> ActualizarAsync(
        int cedula,
        DocentePeticion peticion)
    {
        var docente = Convertir(peticion);

        docente.Cedula = cedula;

        return await _repositorio.ActualizarAsync(docente);
    }

    // Retirar docente
    public Task<bool> EliminarAsync(int cedula)
    {
        return _repositorio.EliminarAsync(cedula);
    }

    // Convertir petición a modelo
    private static Docente Convertir(DocentePeticion p)
    {
        return new Docente
        {
            Cedula = p.Cedula,
            Nombres = p.Nombres.Trim(),
            Apellidos = p.Apellidos.Trim(),
            Genero = p.Genero,
            Cargo = p.Cargo,
            FechaNacimiento = p.FechaNacimiento,
            Correo = p.Correo,
            Telefono = p.Telefono,
            UrlCvlac = p.UrlCvlac,
            Escalafon = p.Escalafon,
            Perfil = p.Perfil,
            CatMinciencia = p.CatMinciencia,
            ConvMinciencia = p.ConvMinciencia,
            Nacionalidaad = p.Nacionalidaad,
            LineaInvestigacionPrincipal =
                p.LineaInvestigacionPrincipal,
            Activo = true
        };
    }
}
