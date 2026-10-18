# 3. Plan técnico — v2

1. Revisar v1: datos, funcionamiento, scripts y criterios de aceptación.
2. Resolver todas las clarificaciones de `2_spec.md`; documentar las decisiones.
3. Diseñar migraciones **no destructivas**, tomando `gestion_local` y los tipos del repositorio real como base. Respaldar la BD antes de aplicar cambios.
4. Implementar 5 procedimientos por cada tabla (y operaciones adicionales para maestro-detalle), con SQL parametrizado y transacciones para lotes.
5. Migrar repositorios Dapper: `CommandType.StoredProcedure`, sin SQL de negocio embebido.
6. Implementar las 11 entidades, sus DTO, servicios, controladores, mapeo de 409 y pantallas Blazor.
7. Crear al menos un trigger y probar INSERT, UPDATE y retiro lógico.
8. Probar errores esperados, claves opcionales y regresión de v1.
9. Documentar prompts reales, PR, evidencia y cerrar con tag `v2` sobre main.

## Arquitectura
Blazor → HTTP/JSON → Controller → Servicio → Repositorio Dapper → Procedimiento SQL Server.

## Dependencias
`linea_investigacion` → `docente` → `estudios_realizados` → `beca`, `apoyo_profesoral`, `estudio_ac`. Además, `docente` → evaluaciones, reconocimientos, experiencia y relaciones con Programa, Red y Término Clave.

## Compuerta 2 — chequeo de constitución
- [ ] Versiones incrementales; v1 preservada.
- [ ] API y frontend separados; sin acceso SQL desde Blazor.
- [ ] Reglas de negocio en servicios; persistencia en repositorios.
- [ ] Secretos en variables de entorno; `.env` fuera de Git.
- [ ] Borrado lógico coherente con decisiones documentadas.
- [ ] Cambios mediante rama personal → Pull Request → main.
**Estado: pendiente de revisión por integrantes.**
