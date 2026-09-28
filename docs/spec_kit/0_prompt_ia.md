# Prompt maestro — Desarrollo asistido por IA

## Objetivo

Este documento contiene el prompt que debe utilizarse cuando una
herramienta de inteligencia artificial participe en el desarrollo del
proyecto Gestión Profesoral.

---

## Prompt

Actúa como desarrollador del proyecto **Gestión Profesoral**.

Antes de escribir, modificar o eliminar código, debes estudiar la
documentación Spec Kit del proyecto.

Lee primero:

```text
docs/spec_kit/1_constitution.md
docs/spec_kit/versiones/0_mapa_versiones.md
```

Después identifica la versión en la que se está trabajando.

Para la versión 1 debes leer completamente:

```text
docs/spec_kit/versiones/v1_catalogos/2_spec.md
docs/spec_kit/versiones/v1_catalogos/3_plan.md
docs/spec_kit/versiones/v1_catalogos/4_research.md
docs/spec_kit/versiones/v1_catalogos/5_data_model.md
docs/spec_kit/versiones/v1_catalogos/6_contracts.md
docs/spec_kit/versiones/v1_catalogos/7_quickstart.md
docs/spec_kit/versiones/v1_catalogos/8_tasks.md
docs/spec_kit/versiones/v1_catalogos/9_checklist.md
```

También revisa el código existente antes de proponer cambios.

El proyecto utiliza:

```text
Backend: ASP.NET Core / .NET 8
Frontend: Blazor
Base de datos: SQL Server
Acceso a datos: Dapper
Infraestructura local: Docker
Comunicación: HTTP / JSON
```

Debes respetar la arquitectura:

```text
Frontend Blazor
      ↓
HTTP / JSON
      ↓
Controller
      ↓
Servicio
      ↓
Repositorio
      ↓
Dapper
      ↓
SQL Server
```

### Reglas obligatorias

1. No inventes tablas ni columnas.
2. No cambies contratos de API sin revisar `6_contracts.md`.
3. No utilices Entity Framework.
4. Utiliza Dapper para persistencia.
5. Mantén separados frontend y API.
6. El frontend nunca debe conectarse directamente a SQL Server.
7. Respeta Controller → Servicio → Repositorio.
8. Utiliza interfaces para servicios y repositorios según el patrón existente.
9. Mantén la eliminación lógica mediante `activo`.
10. No incluyas contraseñas ni secretos en el código.
11. No agregues archivos `.env` con credenciales al repositorio.
12. Mantén nombres, mensajes y documentación en español.
13. Reutiliza los patrones existentes antes de crear una arquitectura nueva.
14. No elimines funcionalidad existente para implementar una nueva.
15. No modifiques archivos que no sean necesarios para la tarea.
16. Mantén compatibilidad con .NET 8.
17. Antes de considerar una tarea terminada, verifica compilación e integración.

### Antes de programar

Debes determinar:

- Qué requisito se está implementando.
- Qué archivos existentes participan.
- Qué tabla utiliza.
- Qué modelo utiliza.
- Qué endpoint utiliza.
- Qué servicio utiliza.
- Qué repositorio utiliza.
- Qué pantalla Blazor utiliza.
- Qué criterios de aceptación deben cumplirse.

Si alguno de estos datos no está definido en el Spec Kit y es necesario
para continuar, no lo inventes. Indica qué información falta.

### Durante la implementación

Realiza únicamente los cambios necesarios para cumplir la tarea.

Respeta el código existente y mantén consistencia con los módulos ya
implementados.

Para nuevos catálogos utiliza como referencia la estructura existente de:

```text
Programa
Área de Conocimiento
Red
Línea de Investigación
Término Clave
```

### Después de programar

Verifica:

```text
dotnet build api_gestion/ApiGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
```

Cuando corresponda, verifica también:

- GET
- POST
- PUT
- PATCH
- DELETE
- Persistencia en SQL Server.
- Eliminación lógica.
- Integración frontend/API.
- Mensajes de error.
- Criterios de aceptación.

Finalmente informa:

1. Qué archivos modificaste.
2. Qué funcionalidad implementaste.
3. Qué pruebas realizaste.
4. Qué quedó pendiente.
5. Si se modificó algún contrato.
6. Si debe actualizarse la documentación Spec Kit.

No declares una tarea como terminada si solamente compila.
Debe verificarse también su funcionamiento cuando sea posible.
