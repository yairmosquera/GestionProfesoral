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

| Identificador | Alcance | Estado documental |
| --- | --- | --- |
| [001-gestion-profesoral-v1](001-gestion-profesoral-v1/) | Catálogos básicos: Programas, Áreas de Conocimiento, Redes, Líneas de Investigación y Términos Clave. | Código localizado; integración vigente pendiente de comprobar. |
| [002-gestion-profesoral-v2](002-gestion-profesoral-v2/) | Docentes, flujos Docente Maestro, estudios y relaciones profesorales. | Código localizado; integración vigente pendiente de comprobar. |
| [003-gestion-profesoral-v3](003-gestion-profesoral-v3/) | Autenticación JWT, administración de usuarios, roles y protección de API/interfaz. | Código localizado; pruebas de seguridad e integración pendientes de acreditar. |

Cada carpeta canónica contiene `spec.md`, `plan.md` y `tasks.md`. Las tres
versiones tienen alcance propio; sus dependencias describen únicamente la
integración entre ellas y no trasladan requisitos de una versión a otra.

## Dependencias entre versiones

- v1 establece catálogos que v2 relaciona con docentes y estudios cuando las
  claves foráneas del esquema los requieren.
- v2 amplía los datos profesorales y sus relaciones; no modifica el alcance
  funcional propio de los catálogos de v1.
- v3 es una capa transversal: sus roles protegen endpoints de v1 y v2. La
  interfaz de esas versiones requiere una sesión para consumir la API protegida.
- La migración de seguridad de v3 declara que no altera las tablas de v1 ni v2.

## Antecedentes conservados

Las carpetas históricas `v1/` y `v2/` se conservan sin cambios para mantener
sus especificaciones, decisiones pendientes y material de trabajo. La
documentación canónica vigente para el alcance solicitado son los tres
directorios numerados de esta página. Las afirmaciones de propuesta de los
antecedentes no sustituyen el estado descrito en los documentos canónicos.
