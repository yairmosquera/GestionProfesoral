# 6. Contratos API — v2 (propuesta pendiente de aprobación)

Para cada recurso nuevo, mantener las operaciones:
- `GET /api/{recurso}` listar
- `GET /api/{recurso}/{id}` consultar
- `POST /api/{recurso}` crear → 201
- `PUT /api/{recurso}/{id}` reemplazar → 200
- `PATCH /api/{recurso}/{id}` actualización parcial, si aplica → 200
- `DELETE /api/{recurso}/{id}` retirar → 204

Usar rutas legibles como `docentes`, `estudios-realizados`, `evaluaciones-docente`. Conservar sin cambios las rutas existentes de v1.

## Claves compuestas
Para `docente_departamento`, `intereses_futuros`, `red_docente` y `estudio_ac`, especificar ambas claves en la ruta de consulta/retiro. Su asociación no debe tener un botón de edición de claves: retirar y reasignar.

## Respuestas de errores
- 400: solicitud mal formada / validación básica.
- 404: recurso no encontrado.
- 409: FK inexistente, PK/pareja duplicada o bloqueo por relación activa (si se acuerda).
- 500: error inesperado; registrar detalle interno, no mostrar traza al usuario.

## Pendiente
Documentar en detalle JSON y contratos por entidad una vez aprobados tipos, campos, rutas y reglas de borrado. No inventar ejemplos incompatibles con el modelo.
