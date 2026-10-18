# 5. Modelo de datos — v2

## Catálogos existentes de v1
`programa(id INT)`, `area_conocimiento(id VARCHAR(6))`, `red(idr INT)`, `linea_investigacion(id INT IDENTITY)`, `termino_clave(termino VARCHAR(30))`.

## Nuevas tablas y sus referencias
| Tabla | Clave primaria | Claves foráneas |
|---|---|---|
| docente | cedula INT | linea_investigacion_principal → linea_investigacion.id (opcional) |
| estudios_realizados | id INT | docente → docente.cedula |
| docente_departamento | docente, departamento | docente → docente.cedula; departamento → programa.id |
| intereses_futuros | docente, termino_clave | docente → docente.cedula; termino_clave → termino_clave.termino |
| evaluacion_docente | id INT IDENTITY | docente → docente.cedula |
| reconocimiento | id INT IDENTITY | docente → docente.cedula |
| experiecia | id INT IDENTITY | docente → docente.cedula |
| red_docente | red, docente | red → red.idr; docente → docente.cedula |
| estudio_ac | estudio, area_conocimiento | estudio → estudios_realizados.id; area_conocimiento → area_conocimiento.id |
| apoyo_profesoral | estudios | estudios → estudios_realizados.id |
| beca | estudios | estudios → estudios_realizados.id |

**Importante:** para `estudio_ac.area_conocimiento` se deberá conservar compatibilidad con `VARCHAR(6)` del repositorio propio, o definir una migración controlada. No usar `INT` para referenciar `VARCHAR(6)`.

## Relaciones maestro-detalle
`docente` → `estudios_realizados`, `evaluacion_docente`, `experiecia`, `reconocimiento`.
`estudios_realizados` → `apoyo_profesoral`, `beca`.

## Estado del modelo
Claves y relaciones revisadas contra el script del profesor. Tipos de columnas restantes y política de `activo` requieren cerrar las clarificaciones antes de generar DDL ejecutable.
