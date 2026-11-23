
using System.ComponentModel.DataAnnotations;
using ApiGestion.Peticiones;
using ApiGestion.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiGestion.Controllers;

[ApiController]
[Route("api/docentes-maestro")]
public class DocenteMaestroController : ControllerBase
{
    private readonly RepositorioDocenteMaestro _repositorio;

    public DocenteMaestroController(
        RepositorioDocenteMaestro repositorio)
    {
        _repositorio = repositorio;
    }

    // ============================================
    // REGISTRAR DOCENTE Y TODOS SUS DETALLES
    // Solo ADMIN y GESTOR
    // ============================================

    [Authorize(Roles = "ADMIN,GESTOR")]
    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] DocenteMaestroPeticion peticion)
    {
        var errores = new List<string>();

        // Validar información del docente.
        if (peticion.Docente is null)
        {
            return BadRequest(new
            {
                mensaje = "Los datos del docente son obligatorios."
            });
        }

        Validar(peticion.Docente, "Docente", errores);

        if (peticion.Docente.Cedula <= 0)
        {
            errores.Add("Ingrese una cédula válida.");
        }

        if (peticion.Docente.FechaNacimiento == default ||
            peticion.Docente.FechaNacimiento.Date >= DateTime.Today)
        {
            errores.Add("La fecha de nacimiento no es válida.");
        }

        // Validar detalles obligatorios.
        if (peticion.Estudios is null ||
            peticion.Estudios.Count == 0)
        {
            errores.Add(
                "Debe registrar al menos un estudio realizado.");
        }
        else
        {
            ValidarLista(peticion.Estudios, "Estudio", errores);
        }

        if (peticion.Evaluaciones is null ||
            peticion.Evaluaciones.Count == 0)
        {
            errores.Add(
                "Debe registrar al menos una evaluación docente.");
        }
        else
        {
            ValidarLista(
                peticion.Evaluaciones, "Evaluación", errores);
        }

        if (peticion.Experiencias is null ||
            peticion.Experiencias.Count == 0)
        {
            errores.Add(
                "Debe registrar al menos una experiencia profesional.");
        }
        else
        {
            ValidarLista(
                peticion.Experiencias, "Experiencia", errores);
        }

        if (peticion.Reconocimientos is null ||
            peticion.Reconocimientos.Count == 0)
        {
            errores.Add(
                "Debe registrar al menos un reconocimiento académico.");
        }
        else
        {
            ValidarLista(
                peticion.Reconocimientos,
                "Reconocimiento",
                errores);
        }

        // No registrar si faltan datos.
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                mensaje = "Revise los datos obligatorios.",
                errores
            });
        }

        try
        {
            var docente = await _repositorio
                .CrearAsync(peticion);

            return Created(
                $"/api/docentes/{docente.Cedula}",
                new
                {
                    mensaje =
                        "Docente y detalles registrados correctamente.",
                    cedula = docente.Cedula,
                    docente,
                    cantidadEstudios = peticion.Estudios.Count,
                    cantidadEvaluaciones = peticion.Evaluaciones.Count,
                    cantidadExperiencias = peticion.Experiencias.Count,
                    cantidadReconocimientos =
                        peticion.Reconocimientos.Count
                });
        }
        catch (SqlException ex)
            when (ex.Number is 2627 or 2601)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un registro con esos datos."
            });
        }
        catch (SqlException ex)
            when (ex.Number is 547 or 51002)
        {
            return Conflict(new
            {
                mensaje =
                    "Una de las relaciones seleccionadas no existe o está inactiva."
            });
        }
        catch (SqlException ex)
            when (ex.Number == 51001)
        {
            return Conflict(new
            {
                mensaje =
                    "Existe un conflicto con los datos relacionados del docente."
            });
        }
    }

    // Validar propiedades con DataAnnotations.
    private static void Validar(
        object modelo,
        string nombre,
        List<string> errores)
    {
        var contexto = new ValidationContext(modelo);
        var resultados = new List<ValidationResult>();

        Validator.TryValidateObject(
            modelo,
            contexto,
            resultados,
            validateAllProperties: true);

        foreach (var resultado in resultados)
        {
            errores.Add(
                $"{nombre}: {resultado.ErrorMessage}");
        }
    }

    // Validar cada registro de una lista.
    private static void ValidarLista<T>(
        List<T>? elementos,
        string nombre,
        List<string> errores)
    {
        if (elementos is null)
            return;

        for (int i = 0; i < elementos.Count; i++)
        {
            if (elementos[i] is null)
            {
                errores.Add(
                    $"{nombre} {i + 1}: registro vacío.");
                continue;
            }

            Validar(
                elementos[i]!,
                $"{nombre} {i + 1}",
                errores);
        }
    }
}
