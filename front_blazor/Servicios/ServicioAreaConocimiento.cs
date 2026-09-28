using System.Net.Http.Json;

namespace FrontGestion.Servicios;

public class AreaConocimiento
{
    public string Id { get; set; } = string.Empty;
    public string GranArea { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class ServicioAreaConocimiento
{
    private readonly HttpClient _http;

    public ServicioAreaConocimiento(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resultado<List<AreaConocimiento>>> Listar()
    {
        try
        {
            var datos = await _http.GetFromJsonAsync<List<AreaConocimiento>>(
                "/api/areas-conocimiento"
            );

            return Resultado<List<AreaConocimiento>>.Bien(
                datos ?? new List<AreaConocimiento>()
            );
        }
        catch
        {
            return Resultado<List<AreaConocimiento>>.Mal(
                "No fue posible cargar las áreas de conocimiento."
            );
        }
    }

    public async Task<Resultado<AreaConocimiento>> Obtener(string id)
    {
        try
        {
            var respuesta = await _http.GetAsync(
                $"/api/areas-conocimiento/{id}"
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                return Resultado<AreaConocimiento>.Mal(
                    "No se encontró el área de conocimiento."
                );
            }

            var area =
                await respuesta.Content.ReadFromJsonAsync<AreaConocimiento>();

            return Resultado<AreaConocimiento>.Bien(area!);
        }
        catch
        {
            return Resultado<AreaConocimiento>.Mal(
                "No fue posible obtener el área de conocimiento."
            );
        }
    }

    public async Task<Resultado<bool>> Crear(AreaConocimiento area)
    {
        var cuerpo = new
        {
            id = area.Id,
            granArea = area.GranArea,
            area = area.Area,
            disciplina = area.Disciplina
        };

        return await Enviar(
            HttpMethod.Post,
            "/api/areas-conocimiento",
            cuerpo
        );
    }

    public async Task<Resultado<bool>> Reemplazar(
        string id,
        AreaConocimiento area)
    {
        var cuerpo = new
        {
            granArea = area.GranArea,
            area = area.Area,
            disciplina = area.Disciplina
        };

        return await Enviar(
            HttpMethod.Put,
            $"/api/areas-conocimiento/{id}",
            cuerpo
        );
    }

    public async Task<Resultado<bool>> Actualizar(
        string id,
        Dictionary<string, object?> cambios)
    {
        return await Enviar(
            HttpMethod.Patch,
            $"/api/areas-conocimiento/{id}",
            cambios
        );
    }

    public async Task<Resultado<bool>> Eliminar(string id)
    {
        return await Enviar(
            HttpMethod.Delete,
            $"/api/areas-conocimiento/{id}",
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

            var respuesta = await _http.SendAsync(peticion);

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