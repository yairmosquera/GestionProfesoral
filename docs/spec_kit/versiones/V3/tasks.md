# Tareas de validación y cierre — Gestión Profesoral V3

**Feature:** `003-gestion-profesoral-v3`  
**Estado:** Lista de trabajo; ninguna tarea pendiente se presume validada sin evidencias.

## Convenciones
- `[ ]` pendiente de ejecutar o demostrar.
- `[x]` documentación elaborada; no implica prueba de ejecución.
- **P0:** bloqueante para producción. **P1:** necesaria para cierre. **P2:** mejora posterior.

## Fase 1 — Inventario y protección de secretos
- [ ] **T001 [P0]** Revisar rama y estado de Git (`git status`, `git branch --show-current`, `git diff`). **Aceptación:** archivos locales identificados sin pérdidas.
- [ ] **T002 [P0]** Revisar `.gitignore`, cambios staged y configuraciones sensibles. **Aceptación:** ningún token, hash, contraseña, cadena de conexión real, `bin/` u `obj/` queda preparado para subir.
- [ ] **T003 [P1]** Validar la lista de controladores locales y métodos HTTP, considerando `[Authorize]` aplicado a clases. **Aceptación:** inventario exhaustivo y diferencias con GitHub documentadas.

## Fase 2 — Backend JWT y autorización
- [ ] **T004 [P0]** Revisar configuración JWT en API: clave, emisor, audiencia, validación de firma y expiración. **Aceptación:** parámetros seguros y pruebas de token inválido/expirado.
- [ ] **T005 [P0]** Confirmar autorización ADMIN exclusiva en `AdministracionUsuariosController.cs` y `RegistroUsuariosController.cs`. **Aceptación:** pruebas de roles y HTTP 403.
- [ ] **T006 [P0]** Confirmar `AutenticacionController.cs`: login anónimo, `/me` autenticado y ausencia de hashes. **Aceptación:** ST-01 a ST-04 aprobados.
- [ ] **T007 [P0]** Validar lectura con ADMIN/GESTOR/CONSULTA y escrituras solo ADMIN/GESTOR en controladores académicos. **Aceptación:** ST-10 a ST-13 aprobados.
- [ ] **T008 [P0]** Verificar invalidez o bloqueo de token frente a usuarios desactivados y cambios de rol, según la implementación de validación vigente. **Aceptación:** ST-19 documentado.
- [ ] **T009 [P1]** Comprobar restricciones de último ADMIN activo al retirar rol y desactivar. **Aceptación:** ST-17 y ST-18 aprobados.

## Fase 3 — Frontend, login y sesión
- [ ] **T010 [P0]** Comprobar registro scoped de `ServicioSesionJwt` y consistencia del estado entre Login, Layout y servicios. **Aceptación:** navegación con sesión activa sin pérdida inesperada.
- [ ] **T011 [P0]** Revisar `App.razor`, `Routes.razor`, `MainLayout.razor` y página `/login`. **Aceptación:** ruta académica sin sesión envía a `/login`, sin bucles.
- [ ] **T012 [P0]** Validar botón Cerrar sesión y método `CerrarSesion`. **Aceptación:** no se muestra contenido privado luego del logout; documentar que el JWT puede seguir vigente en servidor hasta vencer.
- [ ] **T013 [P1]** Inspeccionar servicios HttpClient privados y Bearer por solicitud. **Aceptación:** no hay llamadas protegidas que omitan el JWT.
- [ ] **T014 [P1]** Verificar presentación de errores 401/403 y acciones CRUD bloqueadas en CONSULTA. **Aceptación:** mensajes claros y sin modificaciones prohibidas.
- [ ] **T015 [P1]** Verificar recarga y reconexión de Blazor. **Aceptación:** comportamiento y limitaciones de persistencia documentadas.

## Fase 4 — Pruebas y compatibilidad
- [ ] **T016 [P0]** Ejecutar `dotnet build .\api_gestion\ApiGestion.csproj` y `dotnet build .\front_blazor\FrontGestion.csproj`. **Aceptación:** ambos builds exitosos.
- [ ] **T017 [P0]** Ejecutar la matriz ST-01–ST-16 de `security-test-plan.md`. **Aceptación:** pruebas críticas aprobadas con evidencia sin secretos.
- [ ] **T018 [P1]** Hacer pruebas de regresión de Docente Maestro y módulos V1/V2. **Aceptación:** consultas y operaciones permitidas continúan funcionando.

## Fase 5 — Documentación y preparación de hosting
- [x] **T019 [P1]** Crear nueve documentos de la feature 003. **Aceptación:** los nueve archivos presentes en esta carpeta.
- [ ] **T020 [P1]** Actualizar README raíz con V1/V2/V3, arquitectura, dependencias, cómo ejecutar y reglas de seguridad. **Aceptación:** nuevo colaborador puede configurar sin secretos.
- [ ] **T021 [P0]** Preparar configuración Release y producción de API, Blazor, SQL Server y HTTPS. **Aceptación:** configuración documentada y valores sensibles externos al repositorio.
- [ ] **T022 [P1]** Revisar `git diff --cached`, commit de trabajo y push a rama correcta mediante autorización humana. **Aceptación:** commits revisados, sin secretos y sin borrar trabajo local.
- [ ] **T023 [P0]** Ejecutar checklist previo a despliegue en MonsterASP.NET. **Aceptación:** no publicar si las pruebas de seguridad o compilaciones fallan.

## Dependencias
`T001–T003 → T004–T015 → T016–T018 → T020–T023`.

## Notas
Estas tareas corresponden solo a la capa de seguridad y preparación de V3. Las funcionalidades académicas de V1/V2 se prueban aquí por regresión, pero sus especificaciones son independientes. No hacer merge, push ni publicar automáticamente sin revisión.
