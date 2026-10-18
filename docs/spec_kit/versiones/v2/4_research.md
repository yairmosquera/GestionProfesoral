# 4. Investigación y decisiones técnicas

## Fuentes consultadas
- Profesor: `ProyectosDeAula/docs/0_METODOLOGIA.md`
- Profesor: `ProyectosDeAula/docs/VERSION_2.md`
- Profesor: `ProyectosDeAula/docs/modulo_gestion_profesoral.md`
- Profesor: `ProyectosDeAula/db_scripts/sqlserver/gestion_profesoral.ss.sql`
- Equipo: `docs/spec_kit/versiones/v1_catalogos/2_spec.md`
- Equipo: `db/gestion_profesoral.sql`

## Decisiones técnicas preliminares
- Mantener ASP.NET Core 8, Dapper, SQL Server y Blazor.
- Sustituir los SQL de los repositorios por procedimientos almacenados.
- No ejecutar `DROP TABLE` de los scripts iniciales.
- Usar `SqlException.Number` para clasificar conflictos de integridad SQL Server, en vez de retornar 500.
- Las operaciones de creación de varios detalles requieren transacción; estudiar parámetros tipo tabla (TVP) o JSON procesado en procedimiento.
- El trigger debe contar o auditar también modificaciones efectuadas por retiro lógico.

## Riesgos
- Esquemas diferentes entre ejemplo y solución (ID de área).
- Relación principal de docente a línea de investigación opcional.
- La tabla `experiecia` tiene errata histórica que afecta nombres de clases y SQL.
- La estructura actual reúne API y frontend en un repositorio, contrario a la separación solicitada.

No se ha confirmado compilación ni conexión en un ambiente SQL real.
