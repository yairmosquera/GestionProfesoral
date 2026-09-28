# Mapa de versiones — Gestión Profesoral

> Este documento describe la evolución planificada del proyecto Gestión
> Profesoral. Cada versión incorpora nuevas funcionalidades sin eliminar
> las reglas establecidas en `1_constitution.md`.

---

## Objetivo

El proyecto se desarrolla mediante versiones incrementales.

Cada versión debe entregar una parte funcional del sistema, incluyendo,
cuando corresponda:

- API REST.
- Reglas de negocio.
- Persistencia en SQL Server.
- Frontend Blazor.
- Pruebas funcionales.
- Documentación.

---

## Versiones

| Versión | Alcance                                                   | Estado        |
| ------- | --------------------------------------------------------- | ------------- |
| v1      | Catálogos iniciales sin relaciones complejas              | En desarrollo |
| v2      | Gestión de información profesoral y relaciones            | Pendiente     |
| v3      | Seguridad, autenticación y autorización                   | Pendiente     |
| v4      | Integración, despliegue y funcionalidades complementarias | Pendiente     |

---

# v1 — Catálogos iniciales

La primera versión establece la arquitectura base del proyecto.

En esta versión se implementan inicialmente los recursos:

- Programa.
- Área de Conocimiento.

Estos recursos permiten comprobar el funcionamiento completo de:

```text
Frontend Blazor
       ↓
API ASP.NET Core
       ↓
Servicios
       ↓
Repositorios
       ↓
Dapper
       ↓
SQL Server
```
