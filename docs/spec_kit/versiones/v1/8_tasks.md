# Tareas — Versión 1: Catálogos iniciales

> Lista de tareas técnicas y funcionales de la versión 1 del sistema
> Gestión Profesoral.
>
> Convenciones:
>
> - `[x]` Tarea completada.
> - `[ ]` Tarea pendiente.

---

# 1. Preparación del proyecto

- [x] Crear estructura inicial del repositorio.
- [x] Crear proyecto de API.
- [x] Crear proyecto frontend Blazor.
- [x] Crear estructura para scripts de base de datos.
- [x] Configurar Git.
- [x] Configurar `.gitignore`.
- [x] Crear `.env.example`.
- [x] Mantener `.env` fuera del repositorio.
- [x] Configurar Docker Compose.
- [x] Configurar SQL Server mediante Docker.

---

# 2. Base de datos

- [x] Levantar SQL Server.
- [x] Crear base de datos `gestion_local`.
- [x] Ejecutar scripts iniciales.
- [x] Verificar tabla `programa`.
- [x] Verificar tabla `area_conocimiento`.
- [x] Confirmar estructura real de `area_conocimiento`.
- [x] Confirmar que `programa.id` no utiliza `IDENTITY`.
- [x] Confirmar uso de campo `activo`.
- [x] Verificar persistencia de los datos.

---

# 3. Infraestructura de la API

- [x] Configurar ASP.NET Core.
- [x] Configurar conexión con SQL Server.
- [x] Configurar Dapper.
- [x] Configurar Swagger.
- [x] Crear estructura `Controllers`.
- [x] Crear estructura `Modelos`.
- [x] Crear estructura `Peticiones`.
- [x] Crear estructura `Repositorios`.
- [x] Crear estructura `Servicios`.
- [x] Configurar inyección de dependencias.
- [x] Comprobar compilación de la API.

---

# 4. Backend — Programa

## Modelo

- [x] Crear modelo `Programa`.
- [x] Agregar campo `Id`.
- [x] Agregar `Nombre`.
- [x] Agregar `Tipo`.
- [x] Agregar `Nivel`.
- [x] Agregar campos de fecha.
- [x] Agregar `NumeroCohortes`.
- [x] Agregar `CantGraduados`.
- [x] Agregar `Ciudad`.
- [x] Agregar `Facultad`.
- [x] Agregar `Activo`.

## Repositorio

- [x] Crear `IRepositorioPrograma`.
- [x] Crear `RepositorioPrograma`.
- [x] Implementar listado.
- [x] Implementar consulta por ID.
- [x] Implementar creación.
- [x] Incluir ID manual en creación.
- [x] Implementar actualización.
- [x] Implementar retiro lógico.
- [x] Utilizar consultas parametrizadas.

## Servicio

- [x] Crear interfaz de servicio de Programa.
- [x] Crear servicio de Programa.
- [x] Implementar operaciones CRUD.
- [x] Implementar actualización parcial.
- [x] Implementar retiro lógico.

## Controlador

- [x] Crear controlador de Programa.
- [x] Implementar GET.
- [x] Implementar GET por ID.
- [x] Implementar POST.
- [x] Implementar PUT.
- [x] Implementar PATCH.
- [x] Implementar DELETE.

---

# 5. Pruebas API — Programa

- [x] Probar GET de programas.
- [x] Probar GET por ID.
- [x] Probar POST.
- [x] Probar PUT.
- [x] Probar PATCH.
- [x] Probar DELETE.
- [x] Comprobar borrado lógico.
- [x] Comprobar que registros retirados no aparecen normalmente.

---

# 6. Backend — Área de Conocimiento

## Investigación inicial

- [x] Revisar estructura real de la tabla.
- [x] Identificar campo `gran_area`.
- [x] Identificar campo `area`.
- [x] Identificar campo `disciplina`.
- [x] Identificar campo `activo`.
- [x] Confirmar ID alfanumérico.
- [x] Confirmar longitud máxima del ID.
- [x] Confirmar longitud de `disciplina`.

## Modelo

- [x] Crear modelo `AreaConocimiento`.
- [x] Agregar `Id`.
- [x] Agregar `GranArea`.
- [x] Agregar `Area`.
- [x] Agregar `Disciplina`.
- [x] Agregar `Activo`.

## Repositorio

- [x] Crear interfaz de repositorio.
- [x] Crear repositorio.
- [x] Implementar listado.
- [x] Implementar consulta por ID.
- [x] Implementar creación.
- [x] Implementar actualización.
- [x] Implementar retiro lógico.
- [x] Utilizar SQL parametrizado.

## Servicio

- [x] Crear interfaz de servicio.
- [x] Crear servicio.
- [x] Implementar operaciones CRUD.
- [x] Implementar PATCH.
- [x] Implementar retiro lógico.

## Controlador

- [x] Crear controlador.
- [x] Implementar GET.
- [x] Implementar GET por ID.
- [x] Implementar POST.
- [x] Implementar PUT.
- [x] Implementar PATCH.
- [x] Implementar DELETE.

---

# 7. Pruebas API — Área de Conocimiento

- [x] Probar GET.
- [x] Probar GET por ID.
- [x] Probar POST.
- [x] Probar PUT.
- [x] Probar PATCH.
- [x] Probar DELETE.
- [x] Comprobar borrado lógico.
- [x] Comprobar listado después del retiro.

---

# 8. Frontend — Configuración

- [x] Configurar proyecto Blazor.
- [x] Configurar Razor Components.
- [x] Configurar componentes interactivos.
- [x] Habilitar `InteractiveServer`.
- [x] Configurar `HttpClient`.
- [x] Configurar `ApiUrl`.
- [x] Utilizar puerto independiente para el frontend.
- [x] Configurar `App.razor`.
- [x] Configurar `Routes.razor`.
- [x] Configurar `_Imports.razor`.
- [x] Crear página de inicio.
- [x] Comprobar ruta `/`.

---

# 9. Frontend — Programa

- [x] Crear `ServicioPrograma`.
- [x] Implementar método para listar.
- [x] Implementar método para obtener por ID.
- [x] Implementar método para crear.
- [x] Implementar método para reemplazar.
- [x] Implementar soporte HTTP para PATCH.
- [x] Implementar método para retirar.
- [x] Crear `Programas.razor`.
- [x] Crear formulario.
- [x] Crear tabla.
- [x] Implementar botón Guardar.
- [x] Implementar botón Editar.
- [x] Implementar botón Actualizar.
- [x] Implementar botón Retirar.
- [x] Mostrar mensajes de éxito.
- [x] Mostrar mensajes de error.
- [x] Actualizar listado después de operaciones.

> Nota: el servicio HTTP dispone de soporte para PATCH. La edición normal
> disponible actualmente en la interfaz utiliza PUT.

---

# 10. Pruebas frontend — Programa

- [x] Comprobar carga de la página.
- [x] Comprobar listado.
- [x] Crear un programa desde Blazor.
- [x] Editar un programa desde Blazor.
- [x] Actualizar un programa desde Blazor.
- [x] Retirar un programa desde Blazor.
- [x] Comprobar actualización de la tabla.

---

# 11. Frontend — Área de Conocimiento

- [x] Crear `ServicioAreaConocimiento`.
- [x] Registrar servicio HTTP.
- [x] Implementar listado.
- [x] Implementar consulta por ID.
- [x] Implementar creación.
- [x] Implementar reemplazo.
- [x] Implementar soporte HTTP para PATCH.
- [x] Implementar retiro.
- [x] Crear `AreasConocimiento.razor`.
- [x] Crear formulario.
- [x] Crear tabla.
- [x] Implementar Guardar.
- [x] Implementar Editar.
- [x] Implementar Actualizar.
- [x] Implementar Retirar.
- [x] Mostrar mensajes al usuario.

> Nota: el servicio dispone de soporte para PATCH. La edición normal de la
> pantalla utiliza PUT.

---

# 12. Pruebas frontend — Área de Conocimiento

- [x] Comprobar carga de la página.
- [x] Comprobar listado.
- [x] Crear un área desde Blazor.
- [x] Editar un área desde Blazor.
- [x] Actualizar un área desde Blazor.
- [x] Retirar un área desde Blazor.
- [x] Comprobar actualización del listado.

---

# 13. Interfaz gráfica

- [x] Crear `MainLayout`.
- [x] Crear `NavMenu`.
- [x] Agregar Inicio.
- [x] Agregar Programas al menú.
- [x] Agregar Áreas de Conocimiento al menú.
- [x] Crear estilos generales.
- [x] Crear estilos del menú lateral.
- [x] Crear estilos para formularios.
- [x] Crear estilos para tablas.
- [x] Crear estilos para botones.
- [x] Crear estilos para mensajes.
- [x] Agregar comportamiento responsive básico.

---

# 14. Integración

- [x] Comprobar comunicación Blazor → API.
- [x] Comprobar comunicación API → SQL Server.
- [x] Comprobar flujo Programa completo.
- [x] Comprobar flujo Área de Conocimiento completo.
- [x] Comprobar persistencia.
- [x] Comprobar borrado lógico.

Flujo comprobado:

```text
Usuario
   ↓
Blazor
   ↓
HttpClient
   ↓
API
   ↓
Servicio
   ↓
Repositorio
   ↓
Dapper
   ↓
SQL Server
```

---

# 15. Documentación Spec Kit

- [x] Crear estructura `docs/spec_kit`.
- [x] Crear `1_constitution.md`.
- [x] Crear `0_mapa_versiones.md`.
- [x] Crear `2_spec.md`.
- [x] Crear `3_plan.md`.
- [x] Crear `4_research.md`.
- [x] Crear `5_data_model.md`.
- [x] Crear `6_contracts.md`.
- [x] Crear `7_quickstart.md`.
- [x] Crear `8_tasks.md`.
- [ ] Crear `9_checklist.md`.

---

# 16. Trabajo del equipo

Además de Programa y Área de Conocimiento, la base inicial contiene otros
catálogos asignados dentro del equipo:

```text
linea_investigacion
red
termino_clave
```

Estos recursos deben documentarse como completados únicamente después de
confirmar que su implementación se encuentra integrada y probada.

Por esta razón no se marcan como terminados dentro de este documento.

---

# 17. Integración final con la rama principal

Antes de cerrar la versión:

- [ ] Confirmar cambios finales del otro integrante.
- [ ] Integrar los recursos desarrollados por el equipo.
- [ ] Resolver conflictos de Git si existen.
- [ ] Compilar API después de la integración.
- [ ] Compilar frontend después de la integración.
- [ ] Ejecutar pruebas completas después del merge.
- [ ] Revisar que `.env` no haya sido agregado al repositorio.
- [ ] Revisar que no existan archivos temporales innecesarios.
- [ ] Confirmar funcionamiento de todos los recursos de la versión.
- [ ] Confirmar que la rama principal contiene la versión funcional.

---

# 18. Verificación Git

Antes de subir cambios:

```bash
git status
```

Posteriormente:

```bash
dotnet build api_gestion/ApiGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
```

Cuando todo sea correcto:

```bash
git add .
git status
```

Se debe revisar nuevamente la lista antes del commit.

Después:

```bash
git commit -m "Documenta version 1 con Spec Kit"
```

y finalmente:

```bash
git push
```

> No ejecutar una integración con `main` únicamente por completar este
> documento. La integración debe realizarse siguiendo el flujo acordado por
> el equipo.

---

# 19. Criterio de finalización

La versión podrá marcarse como finalizada cuando:

- [x] Programa esté implementado.
- [x] Programa esté probado.
- [x] Área de Conocimiento esté implementada.
- [x] Área de Conocimiento esté probada.
- [x] Frontend consuma la API.
- [x] La API persista en SQL Server.
- [x] El borrado lógico funcione.
- [ ] Los recursos del equipo estén integrados.
- [ ] La compilación final posterior a la integración sea exitosa.
- [ ] Las pruebas finales del equipo sean exitosas.
- [ ] `9_checklist.md` esté completado.
- [ ] La versión esté integrada correctamente en la rama principal.

---

# 20. Estado actual

Estado de los recursos documentados:

```text
Programa
API:       COMPLETADA
Frontend:  COMPLETADO
Pruebas:   COMPLETADAS

Área de Conocimiento
API:       COMPLETADA
Frontend:  COMPLETADO
Pruebas:   COMPLETADAS
```

Estado global de la versión:

```text
EN DESARROLLO / PENDIENTE DE INTEGRACIÓN FINAL DEL EQUIPO
```

La versión no debe marcarse como completamente finalizada hasta realizar la
integración y ejecutar el checklist final.
