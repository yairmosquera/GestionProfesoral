# Checklist final — Versión 1: Catálogos iniciales

> Lista de verificación final para la versión 1 del sistema Gestión Profesoral.
>
> Este documento debe revisarse antes de considerar la versión como terminada.
>
> Convenciones:
>
> - `[x]` Verificado.
> - `[ ]` Pendiente de verificar.

---

# 1. Estructura del proyecto

- [x] Existe el proyecto `api_gestion`.
- [x] Existe el proyecto `front_blazor`.
- [x] Existe la carpeta `db`.
- [x] Existe la documentación `docs/spec_kit`.
- [x] Existe `docker-compose.yml`.
- [x] Existe `.gitignore`.
- [x] Existe `.env.example`.
- [x] `.env` no debe almacenarse en Git.

---

# 2. Base de datos

- [x] SQL Server funciona mediante Docker.
- [x] Existe la base `gestion_local`.
- [x] Existe la tabla `programa`.
- [x] Existe la tabla `area_conocimiento`.
- [x] Se verificó la estructura real de Programa.
- [x] Se verificó la estructura real de Área de Conocimiento.
- [x] Programa utiliza borrado lógico.
- [x] Área de Conocimiento utiliza borrado lógico.
- [x] El campo `activo` controla el estado del registro.
- [x] `programa.id` no es generado automáticamente.
- [x] `area_conocimiento.id` es alfanumérico.

---

# 3. Arquitectura de la API

Comprobar el flujo:

```text
Controller
    ↓
IServicio
    ↓
Servicio
    ↓
IRepositorio
    ↓
Repositorio
    ↓
Dapper
    ↓
SQL Server
```

Verificación:

- [x] Existen controladores.
- [x] Existen servicios.
- [x] Existen interfaces de servicios.
- [x] Existen repositorios.
- [x] Existen interfaces de repositorios.
- [x] Se utiliza Dapper.
- [x] Se utiliza SQL parametrizado.
- [x] Los controladores no acceden directamente a SQL Server.
- [x] El frontend no accede directamente a SQL Server.

---

# 4. Programa — API

Comprobar:

```text
GET     /api/programas
GET     /api/programas/{id}
POST    /api/programas
PUT     /api/programas/{id}
PATCH   /api/programas/{id}
DELETE  /api/programas/{id}
```

Estado:

- [x] GET funciona.
- [x] GET por ID funciona.
- [x] POST funciona.
- [x] PUT funciona.
- [x] PATCH funciona.
- [x] DELETE funciona.
- [x] El ID se proporciona durante la creación.
- [x] Los registros nuevos quedan activos.
- [x] DELETE realiza retiro lógico.
- [x] Los registros retirados dejan de aparecer normalmente.

---

# 5. Área de Conocimiento — API

Comprobar:

```text
GET     /api/areas-conocimiento
GET     /api/areas-conocimiento/{id}
POST    /api/areas-conocimiento
PUT     /api/areas-conocimiento/{id}
PATCH   /api/areas-conocimiento/{id}
DELETE  /api/areas-conocimiento/{id}
```

Estado:

- [x] GET funciona.
- [x] GET por ID funciona.
- [x] POST funciona.
- [x] PUT funciona.
- [x] PATCH funciona.
- [x] DELETE funciona.
- [x] Se utiliza `GranArea`.
- [x] Se utiliza `Area`.
- [x] Se utiliza `Disciplina`.
- [x] El identificador se maneja como texto.
- [x] DELETE realiza retiro lógico.

---

# 6. Programa — Frontend

Ruta:

```text
/programas
```

Verificación:

- [x] La página carga correctamente.
- [x] La tabla muestra programas.
- [x] El formulario permite registrar.
- [x] El campo ID está disponible al crear.
- [x] Guardar funciona.
- [x] Editar funciona.
- [x] Actualizar funciona.
- [x] Retirar funciona.
- [x] El listado se actualiza después de las operaciones.
- [x] Se muestran mensajes al usuario.
- [x] El frontend consume la API mediante `ServicioPrograma`.

---

# 7. Área de Conocimiento — Frontend

Ruta:

```text
/areas-conocimiento
```

Verificación:

- [x] La página carga correctamente.
- [x] La tabla muestra las áreas.
- [x] El formulario permite registrar.
- [x] El formulario contiene ID.
- [x] El formulario contiene Gran área.
- [x] El formulario contiene Área.
- [x] El formulario contiene Disciplina.
- [x] Guardar funciona.
- [x] Editar funciona.
- [x] Actualizar funciona.
- [x] Retirar funciona.
- [x] El listado se actualiza.
- [x] El frontend consume la API mediante `ServicioAreaConocimiento`.

---

# 8. Blazor

- [x] Razor Components está configurado.
- [x] Interactive Server está configurado.
- [x] Las páginas interactivas utilizan `@rendermode InteractiveServer`.
- [x] Los botones ejecutan eventos correctamente.
- [x] Existe `App.razor`.
- [x] Existe `Routes.razor`.
- [x] Existe `_Imports.razor`.
- [x] Existe `MainLayout.razor`.
- [x] Existe `NavMenu.razor`.

---

# 9. Navegación

Comprobar las rutas:

```text
/
/programas
/areas-conocimiento
```

Estado:

- [x] Inicio funciona.
- [x] Programas funciona.
- [x] Áreas de Conocimiento funciona.
- [x] El menú permite navegar entre las páginas.
- [x] La ruta `/` no responde 404.

---

# 10. Comunicación frontend/API

Configuración local:

```text
Frontend
http://localhost:5100

        ↓ HTTP / JSON

API
http://localhost:5000
```

Verificación:

- [x] `ApiUrl` está configurada.
- [x] `ServicioPrograma` utiliza HttpClient.
- [x] `ServicioAreaConocimiento` utiliza HttpClient.
- [x] Programa recibe información desde la API.
- [x] Área recibe información desde la API.
- [x] Las operaciones modifican realmente SQL Server.

---

# 11. Separación de responsabilidades

- [x] Blazor no contiene consultas SQL.
- [x] Los controladores no contienen la lógica de acceso directo a datos.
- [x] Los repositorios administran SQL.
- [x] Los servicios administran las operaciones del dominio.
- [x] El frontend utiliza servicios HTTP.
- [x] Frontend y API se comunican mediante JSON.

---

# 12. Borrado lógico

Para Programa:

```text
DELETE /api/programas/{id}
        ↓
activo = 0
```

Para Área:

```text
DELETE /api/areas-conocimiento/{id}
        ↓
activo = 0
```

Verificación:

- [x] No se elimina físicamente Programa.
- [x] No se elimina físicamente Área de Conocimiento.
- [x] Los registros retirados permanecen en SQL Server.
- [x] Los registros retirados dejan de aparecer en listados normales.

---

# 13. Seguridad

- [x] Las contraseñas no están escritas en el código documentado.
- [x] `.env` se utiliza para configuración sensible.
- [x] `.env.example` sirve como referencia.
- [x] La documentación no contiene contraseñas reales.
- [ ] Verificar nuevamente que `.env` no esté preparado para commit.
- [ ] Verificar el historial antes de la entrega final si alguna credencial fue agregada accidentalmente.

---

# 14. Docker

- [x] Existe configuración Docker.
- [x] SQL Server puede ejecutarse en Docker.
- [x] Existe persistencia mediante volumen.
- [x] La base puede utilizarse desde la API.
- [ ] Realizar prueba final desde un entorno limpio antes de la entrega.

---

# 15. Compilación final

Antes de cerrar la versión ejecutar:

```bash
dotnet restore api_gestion/ApiGestion.csproj
dotnet build api_gestion/ApiGestion.csproj
```

Después:

```bash
dotnet restore front_blazor/FrontGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
```

Checklist:

- [ ] API compila después de la integración final.
- [ ] Frontend compila después de la integración final.
- [ ] No existen errores de compilación.
- [ ] Revisar advertencias importantes.

> Estas tareas permanecen pendientes hasta realizar la integración final del
> trabajo de todos los integrantes.

---

# 16. Prueba funcional final

Ejecutar:

```bash
docker compose up -d
```

Después ejecutar la API:

```bash
dotnet run --project api_gestion/ApiGestion.csproj
```

Y en otra terminal:

```bash
dotnet run --project front_blazor/FrontGestion.csproj
```

Comprobar:

- [ ] Swagger abre correctamente.
- [ ] Frontend abre correctamente.
- [ ] Programa carga.
- [ ] Área de Conocimiento carga.
- [ ] Crear Programa funciona después de la integración.
- [ ] Editar Programa funciona después de la integración.
- [ ] Retirar Programa funciona después de la integración.
- [ ] Crear Área funciona después de la integración.
- [ ] Editar Área funciona después de la integración.
- [ ] Retirar Área funciona después de la integración.

---

# 17. Integración del equipo

Existen recursos adicionales desarrollados por otros integrantes.

Antes de cerrar la versión:

- [ ] Integrar cambios pendientes del equipo.
- [ ] Resolver conflictos.
- [ ] Revisar `linea_investigacion`.
- [ ] Revisar `red`.
- [ ] Revisar `termino_clave`.
- [ ] Confirmar que los recursos integrados funcionan.
- [ ] Confirmar que no se dañaron Programa y Área de Conocimiento.

---

# 18. Git

Antes del commit final:

```bash
git status
```

Comprobar:

- [ ] No aparece `.env`.
- [ ] No aparecen contraseñas.
- [ ] No se agregan archivos temporales innecesarios.
- [ ] Los archivos del Spec Kit aparecen correctamente.
- [ ] Los cambios corresponden a la versión.

Después:

```bash
git add .
git status
```

Revisar nuevamente antes del commit.

---

# 19. Documentación Spec Kit

La estructura esperada es:

```text
docs/
└── spec_kit/
    ├── 1_constitution.md
    └── versiones/
        ├── 0_mapa_versiones.md
        └── v1_catalogos/
            ├── 2_spec.md
            ├── 3_plan.md
            ├── 4_research.md
            ├── 5_data_model.md
            ├── 6_contracts.md
            ├── 7_quickstart.md
            ├── 8_tasks.md
            └── 9_checklist.md
```

Verificación:

- [x] Constitución creada.
- [x] Mapa de versiones creado.
- [x] Especificación creada.
- [x] Plan creado.
- [x] Investigación creada.
- [x] Modelo de datos creado.
- [x] Contratos creados.
- [x] Quickstart creado.
- [x] Tareas creadas.
- [x] Checklist creado.

---

# 20. Revisión de consistencia

Antes de entregar, verificar que todos los documentos utilicen las rutas
reales:

```text
/api/programas
/api/areas-conocimiento
```

y no rutas tomadas directamente de proyectos de referencia.

Comprobar:

- [ ] `2_spec.md` coincide con la implementación.
- [ ] `3_plan.md` coincide con la arquitectura.
- [ ] `4_research.md` coincide con las decisiones tomadas.
- [ ] `5_data_model.md` coincide con SQL Server.
- [ ] `6_contracts.md` coincide con Swagger.
- [ ] `7_quickstart.md` permite ejecutar el proyecto.
- [ ] `8_tasks.md` refleja el estado real.
- [ ] `9_checklist.md` refleja la verificación final.

---

# 21. Criterio de cierre

La versión 1 puede marcarse como:

```text
COMPLETADA
```

únicamente cuando todos los puntos obligatorios pendientes de este checklist
hayan sido verificados.

Mientras existan tareas de integración del equipo, el estado debe mantenerse
como:

```text
EN DESARROLLO / PENDIENTE DE INTEGRACIÓN FINAL
```

---

# 22. Estado actual

## Programa

```text
Backend:       COMPLETADO
API CRUD:      PROBADO
Frontend:      COMPLETADO
Integración:   PROBADA
```

## Área de Conocimiento

```text
Backend:       COMPLETADO
API CRUD:      PROBADO
Frontend:      COMPLETADO
Integración:   PROBADA
```

## Equipo

```text
Integración final: PENDIENTE
```

Por lo tanto, el estado actual de la versión es:

```text
PENDIENTE DE VERIFICACIÓN E INTEGRACIÓN FINAL DEL EQUIPO
```
