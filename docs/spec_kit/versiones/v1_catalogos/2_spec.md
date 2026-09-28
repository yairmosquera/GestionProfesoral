# Especificación — Versión 1: Catálogos iniciales

> **Versión 1 — Gestión Profesoral**
>
> Esta versión implementa la primera funcionalidad completa del sistema,
> incluyendo API REST, persistencia en SQL Server y frontend Blazor.
>
> Ante cualquier diferencia, se deben respetar las reglas establecidas en
> `../../1_constitution.md`.

---

## 1. Propósito

El propósito de la versión 1 es implementar los primeros catálogos del
sistema Gestión Profesoral y establecer el patrón que utilizarán los demás
recursos del proyecto.

Los recursos incluidos inicialmente son:

- Programa.
- Área de Conocimiento.

La versión permite comprobar el flujo completo:

```text
Usuario
   ↓
Frontend Blazor
   ↓
HTTP / JSON
   ↓
API ASP.NET Core
   ↓
Servicio
   ↓
Repositorio
   ↓
Dapper
   ↓
SQL Server
```
