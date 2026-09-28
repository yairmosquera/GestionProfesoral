# Constitución del proyecto — Gestión Profesoral

> **Documento permanente.**
> Este documento establece las reglas generales para el desarrollo del
> proyecto Gestión Profesoral. Las especificaciones de cada versión se
> encuentran en `versiones/`.
>
> Cuando exista una diferencia entre una especificación de versión y esta
> constitución, se deberá revisar y documentar el cambio antes de modificar
> las reglas generales del proyecto.

---

## Artículo 1 — Desarrollo por versiones

El proyecto se construye mediante versiones incrementales.

Cada versión debe definir:

- Su alcance.
- Los recursos que incorpora.
- Los requisitos funcionales.
- Los contratos de la API.
- El modelo de datos utilizado.
- Las tareas necesarias.
- Los criterios de aceptación.
- Las pruebas requeridas.

Una versión se considera terminada cuando su API, frontend y persistencia
funcionan de manera integrada.

No se implementarán funcionalidades correspondientes a versiones futuras
hasta que sean necesarias.

---

## Artículo 2 — Arquitectura del sistema

El sistema está compuesto por tres procesos principales:

1. Base de datos SQL Server.
2. API REST desarrollada con ASP.NET Core.
3. Frontend desarrollado con Blazor.

La comunicación entre el frontend y la API se realiza mediante HTTP.

El frontend no debe acceder directamente a SQL Server.

La arquitectura general es:

```text
Usuario
   │
   ▼
Frontend Blazor
   │
   │ HTTP / JSON
   ▼
API ASP.NET Core
   │
   ▼
Servicio
   │
   ▼
Repositorio
   │
   │ Dapper / SQL parametrizado
   ▼
SQL Server
```
