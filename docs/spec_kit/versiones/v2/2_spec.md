# 2. Especificación — v2 Gestión Profesoral

## Propósito y alcance
Ampliar los cinco catálogos de v1 a los 16 recursos del módulo sin romper los CRUD existentes. Se implementarán: `docente`, `estudios_realizados`, `docente_departamento`, `intereses_futuros`, `evaluacion_docente`, `reconocimiento`, `experiecia`, `red_docente`, `estudio_ac`, `apoyo_profesoral` y `beca`.

## Requisitos verificables
- **RF-V2-01**: Cada una de las 16 tablas dispone de listar, consultar, crear, modificar y retirar mediante procedimientos almacenados, con rutas REST específicas.
- **RF-V2-02**: Las relaciones de clave foránea se eligen mediante desplegables cargados por la API; la opción opcional envía `null`.
- **RF-V2-03**: Las parejas de tablas puente se crean y retiran por clave compuesta; un duplicado responde HTTP 409.
- **RF-V2-04**: Una clave foránea inexistente responde HTTP 409 con mensaje en español.
- **RF-V2-05**: Desde la página de docente se ven y administran estudios, evaluaciones, experiencia y reconocimientos; desde un estudio, beca y apoyo profesoral.
- **RF-V2-06**: La creación conjunta de maestro y varios detalles es atómica: un único envío a la API y una sola transacción en SQL Server.
- **RF-V2-07**: Hay por lo menos un disparador comprobable desde la UI, incluido el cambio por retiro lógico.
- **RF-V2-08**: Cuando exista columna `activo`, retirar conserva la fila y los listados la excluyen.
- **RF-V2-09**: La interfaz presenta mensajes comprensibles sin códigos HTTP ni jerga técnica.
- **RF-V2-10**: Los cinco CRUD de v1 siguen funcionando y pasan la regresión.

## Exclusiones
No construir JWT, roles ni usuarios (v3); tampoco dashboard, marca o despliegue público (v4).

## Clarificaciones — compuerta 1, pendientes de resolver en equipo
- [NECESITA ACLARACIÓN] ¿Se añadirá `activo` a las 11 tablas nuevas que no lo tienen en el script original? El proyecto v1 sí lo usa; documentar la decisión por tabla.
- [NECESITA ACLARACIÓN] ¿Se permitirá retirar lógicamente un docente con detalles activos? Definir comportamiento antes de programar servicios.
- [NECESITA ACLARACIÓN] `area_conocimiento.id` es `VARCHAR(6)` en el proyecto y `INT` en el modelo del profesor. Se propone conservar el identificador existente y tipar `estudio_ac.area_conocimiento` de forma compatible, sujeto a aprobación del equipo.
- [NECESITA ACLARACIÓN] Confirmar si la v1 fue cerrada formalmente, con pruebas de los cinco CRUD, PR y tag `v1`.
- [NECESITA ACLARACIÓN] Confirmar si se mantienen los nombres históricos `experiecia`, `nacionalidaad`, `docente_departamento.departamento` y tipos originales de `fecha_fin`; no corregirlos silenciosamente.
- [NECESITA ACLARACIÓN] Confirmar convención para respuestas JSON, rutas singulares/plurales, IDs y semántica de `PATCH`, para respetar clientes de v1.
- [NECESITA ACLARACIÓN] Seleccionar el disparador y si requerirá agregar campo derivado o tabla de auditoría.

**No aprobar esta especificación ni pasar a implementar hasta resolver estas decisiones.**
