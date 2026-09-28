using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class Programa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public string? FechaCierre { get; set; }
    public string NumeroCohortes { get; set; } = string.Empty;
    public string CantGraduados { get; set; } = string.Empty;
    public string FechaActualizacion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int Facultad { get; set; }
    public bool Activo { get; set; }
}

public record Resultado<T>(bool Ok, T? Datos, List<string> Errores)
{
    public static Resultado<T> Bien(T datos)
        => new(true, datos, new());

    public static Resultado<T> Mal(params string[] errores)
        => new(false, default, errores.ToList());
}

public class ServicioPrograma
{
    private readonly HttpClient _http;

    public ServicioPrograma(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<Programa>>> Listar()
    {
        try
        {
            var datos = await _http.GetFromJsonAsync<List<Programa>>(
                "/api/programas"
            );

            return Resultado<List<Programa>>.Bien(
                datos ?? new List<Programa>()
            );
        }
        catch
        {
            return Resultado<List<Programa>>.Mal(
                "No fue posible cargar los programas."
            );
        }
    }

    public async Task<Resultado<Programa>> Obtener(int id)
    {
        try
        {
            var respuesta = await _http.GetAsync(
                $"/api/programas/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<Programa>.Mal(
                    "No se encontró el programa."
                );
            }

            var programa =
                await respuesta.Content.ReadFromJsonAsync<Programa>();

            return Resultado<Programa>.Bien(programa!);
        }
        catch
        {
            return Resultado<Programa>.Mal(
                "No fue posible obtener el programa."
            );
        }
    }

    public async Task<Resultado<bool>> Crear(Programa programa)
    {
        return await Enviar(
            HttpMethod.Post,
            "/api/programas",
            programa
        );
    }

    public async Task<Resultado<bool>> Reemplazar(
        int id,
        Programa programa)
    {
        var cuerpo = new
        {
            nombre = programa.Nombre,
            tipo = programa.Tipo,
            nivel = programa.Nivel,
            fechaCreacion = programa.FechaCreacion,
            fechaCierre = programa.FechaCierre,
            numeroCohortes = programa.NumeroCohortes,
            cantGraduados = programa.CantGraduados,
            fechaActualizacion = programa.FechaActualizacion,
            ciudad = programa.Ciudad,
            facultad = programa.Facultad
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/programas/{id}",
            cuerpo
        );
    }

    public async Task<Resultado<bool>> Actualizar(
        int id,
        Dictionary<string, object?> cambios)
    {
        return await Enviar(
            HttpMethod.Patch,
            $"/api/programas/{id}",
            cambios
        );
    }

    public async Task<Resultado<bool>> Eliminar(int id)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/programas/{id}",
            null
        );
    }

    private async Task<Resultado<bool>> Enviar(
        HttpMethod metodo,
        string ruta,
        object? cuerpo)
    {
        try
        {
            var peticion = new HttpRequestMessage(
                metodo,
                ruta
            );

            if (cuerpo is not null)
            {
                peticion.Content = JsonContent.Create(cuerpo);
            }

            var respuesta =
                await _http.SendAsync(peticion);

            if (respuesta.IsSuccessStatusCode)
            {
                return Resultado<bool>.Bien(true);
            }

            return Resultado<bool>.Mal(
                $"La operación no pudo completarse. Código: {(int)respuesta.StatusCode}"
            );
        }
        catch
        {
            return Resultado<bool>.Mal(
                "No fue posible comunicarse con la API."
            );
        }
    }
}