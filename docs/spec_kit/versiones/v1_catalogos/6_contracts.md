# Contratos HTTP — Versión 1: Catálogos iniciales

> Este documento define los contratos HTTP utilizados por la versión 1
> de Gestión Profesoral.
>
> Los recursos incluidos son Programa y Área de Conocimiento.

---

# 1. Convenciones generales

La API utiliza JSON para el intercambio de información.

Durante el desarrollo local la dirección base es:

```text
http://localhost:5000
```

Los recursos de esta versión son:

```text
/api/programas
/api/areas-conocimiento
```

Los verbos utilizados son:

| Verbo  | Operación               |
| ------ | ----------------------- |
| GET    | Consultar               |
| POST   | Crear                   |
| PUT    | Reemplazar información  |
| PATCH  | Actualizar parcialmente |
| DELETE | Retirar lógicamente     |

DELETE representa un borrado lógico.

---

# 2. Programa — Listar

## Solicitud

```http
GET /api/programas
```

## Propósito

Obtiene los programas activos.

## Respuesta esperada

```json
[
  {
    "id": 101,
    "nombre": "Ingeniería Ambiental",
    "tipo": "Pregrado",
    "nivel": "Profesional",
    "fechaCreacion": "2026-01-01",
    "fechaCierre": null,
    "numeroCohortes": "1",
    "cantGraduados": "0",
    "fechaActualizacion": "2026-09-01",
    "ciudad": "Medellín",
    "facultad": 1,
    "activo": true
  }
]
```

Los registros con:

```text
activo = false
```

no deben aparecer en el listado normal.

---

# 3. Programa — Obtener por ID

## Solicitud

```http
GET /api/programas/{id}
```

Ejemplo:

```http
GET /api/programas/101
```

## Respuesta esperada

```json
{
  "id": 101,
  "nombre": "Ingeniería Ambiental",
  "tipo": "Pregrado",
  "nivel": "Profesional",
  "fechaCreacion": "2026-01-01",
  "fechaCierre": null,
  "numeroCohortes": "1",
  "cantGraduados": "0",
  "fechaActualizacion": "2026-09-01",
  "ciudad": "Medellín",
  "facultad": 1,
  "activo": true
}
```

Si el programa no existe, la API debe responder según el manejo de recurso
no encontrado definido por el backend.

---

# 4. Programa — Crear

## Solicitud

```http
POST /api/programas
Content-Type: application/json
```

## Cuerpo

```json
{
  "id": 101,
  "nombre": "Ingeniería Ambiental",
  "tipo": "Pregrado",
  "nivel": "Profesional",
  "fechaCreacion": "2026-01-01",
  "fechaCierre": null,
  "numeroCohortes": "1",
  "cantGraduados": "0",
  "fechaActualizacion": "2026-09-01",
  "ciudad": "Medellín",
  "facultad": 1
}
```

## Reglas

El ID debe proporcionarse porque:

```text
programa.id
```

no es `IDENTITY`.

`fechaCierre` puede ser nulo.

El nuevo registro debe quedar activo.

---

# 5. Programa — Reemplazar

## Solicitud

```http
PUT /api/programas/{id}
Content-Type: application/json
```

Ejemplo:

```http
PUT /api/programas/101
```

## Cuerpo

```json
{
  "nombre": "Ingeniería Ambiental",
  "tipo": "Pregrado",
  "nivel": "Profesional",
  "fechaCreacion": "2026-01-01",
  "fechaCierre": null,
  "numeroCohortes": "2",
  "cantGraduados": "10",
  "fechaActualizacion": "2026-09-27",
  "ciudad": "Medellín",
  "facultad": 1
}
```

## Regla

El identificador se obtiene desde la URL.

Por lo tanto, el cuerpo del PUT no necesita modificar:

```text
id
```

---

# 6. Programa — Actualización parcial

## Solicitud

```http
PATCH /api/programas/{id}
Content-Type: application/json
```

Ejemplo:

```http
PATCH /api/programas/101
```

## Ejemplo de cuerpo

```json
{
  "ciudad": "Medellín"
}
```

También podría modificarse más de un campo cuando el backend lo permita.

Ejemplo:

```json
{
  "ciudad": "Medellín",
  "numeroCohortes": "3"
}
```

## Regla

PATCH modifica únicamente los campos enviados.

Los campos no enviados conservan sus valores anteriores.

---

# 7. Programa — Retiro lógico

## Solicitud

```http
DELETE /api/programas/{id}
```

Ejemplo:

```http
DELETE /api/programas/101
```

## Comportamiento

La fila no debe eliminarse físicamente.

El resultado en la base debe ser equivalente a:

```sql
UPDATE programa
SET activo = 0
WHERE id = @Id;
```

Después de la operación el programa no debe aparecer en el listado de
programas activos.

---

# 8. Área de Conocimiento — Listar

## Solicitud

```http
GET /api/areas-conocimiento
```

## Propósito

Obtiene las áreas de conocimiento activas.

## Ejemplo de respuesta

```json
[
  {
    "id": "AC002",
    "granArea": "Ingeniería y Tecnología",
    "area": "Ingeniería Ambiental",
    "disciplina": "Gestión Ambiental",
    "activo": true
  }
]
```

Los registros retirados no deben aparecer.

---

# 9. Área de Conocimiento — Obtener por ID

## Solicitud

```http
GET /api/areas-conocimiento/{id}
```

Ejemplo:

```http
GET /api/areas-conocimiento/AC002
```

## Ejemplo de respuesta

```json
{
  "id": "AC002",
  "granArea": "Ingeniería y Tecnología",
  "area": "Ingeniería Ambiental",
  "disciplina": "Gestión Ambiental",
  "activo": true
}
```

---

# 10. Área de Conocimiento — Crear

## Solicitud

```http
POST /api/areas-conocimiento
Content-Type: application/json
```

## Cuerpo

```json
{
  "id": "AC002",
  "granArea": "Ingeniería y Tecnología",
  "area": "Ingeniería Ambiental",
  "disciplina": "Gestión Ambiental"
}
```

## Campos requeridos

```text
id
granArea
area
disciplina
```

El campo `activo` no necesita ser solicitado al usuario durante el registro.

El nuevo registro debe quedar activo.

---

# 11. Área de Conocimiento — Reemplazar

## Solicitud

```http
PUT /api/areas-conocimiento/{id}
Content-Type: application/json
```

Ejemplo:

```http
PUT /api/areas-conocimiento/AC002
```

## Cuerpo

```json
{
  "granArea": "Ingeniería y Tecnología",
  "area": "Ciencias Ambientales",
  "disciplina": "Gestión y Evaluación Ambiental"
}
```

## Regla

El ID se obtiene de la URL y no se modifica.

---

# 12. Área de Conocimiento — Actualización parcial

## Solicitud

```http
PATCH /api/areas-conocimiento/{id}
Content-Type: application/json
```

Ejemplo:

```http
PATCH /api/areas-conocimiento/AC002
```

## Cuerpo de ejemplo

```json
{
  "disciplina": "Gestión de Recursos Naturales"
}
```

Solo los campos enviados deben modificarse.

---

# 13. Área de Conocimiento — Retiro lógico

## Solicitud

```http
DELETE /api/areas-conocimiento/{id}
```

Ejemplo:

```http
DELETE /api/areas-conocimiento/AC002
```

## Comportamiento

La operación debe producir un cambio equivalente a:

```sql
UPDATE area_conocimiento
SET activo = 0
WHERE id = @Id;
```

La fila permanece almacenada, pero deja de aparecer en los listados
normales.

---

# 14. Contrato del frontend — Programa

El frontend utiliza:

```text
ServicioPrograma
```

para comunicarse con:

```text
/api/programas
```

La correspondencia es:

| Acción frontend            | HTTP       |
| -------------------------- | ---------- |
| `Listar()`                 | GET        |
| `Obtener(id)`              | GET por ID |
| `Crear(programa)`          | POST       |
| `Reemplazar(id, programa)` | PUT        |
| `Actualizar(id, cambios)`  | PATCH      |
| `Eliminar(id)`             | DELETE     |

La página que consume el servicio es:

```text
/programas
```

---

# 15. Contrato del frontend — Área de Conocimiento

El frontend utiliza:

```text
ServicioAreaConocimiento
```

para comunicarse con:

```text
/api/areas-conocimiento
```

La correspondencia es:

| Acción frontend           | HTTP       |
| ------------------------- | ---------- |
| `Listar()`                | GET        |
| `Obtener(id)`             | GET por ID |
| `Crear(area)`             | POST       |
| `Reemplazar(id, area)`    | PUT        |
| `Actualizar(id, cambios)` | PATCH      |
| `Eliminar(id)`            | DELETE     |

La página que consume el servicio es:

```text
/areas-conocimiento
```

---

# 16. Códigos HTTP

Los códigos exactos son responsabilidad del contrato implementado por los
controladores.

De manera general, la versión utiliza respuestas exitosas para las
operaciones completadas y respuestas de error cuando:

- El recurso no existe.
- Los datos enviados no son válidos.
- La operación no puede completarse.
- Ocurre un error interno.

El frontend no debe mostrar códigos HTTP técnicos como mensaje principal al
usuario cuando pueda proporcionar una explicación comprensible.

---

# 17. Error de comunicación desde el frontend

Cuando el frontend no puede comunicarse con la API, los servicios deben
capturar el problema y devolver un resultado controlado.

Ejemplo de mensaje:

```text
No fue posible comunicarse con la API.
```

Para los listados:

```text
No fue posible cargar los programas.
```

o:

```text
No fue posible cargar las áreas de conocimiento.
```

---

# 18. Seguridad del contrato

Los cuerpos JSON nunca deben contener:

- Contraseñas de SQL Server.
- Cadenas de conexión.
- Secretos de infraestructura.
- Variables privadas del servidor.

La API expone únicamente la información necesaria para el recurso.

---

# 19. Resumen de endpoints

| Método | Ruta                           | Operación           |
| ------ | ------------------------------ | ------------------- |
| GET    | `/api/programas`               | Listar programas    |
| GET    | `/api/programas/{id}`          | Obtener programa    |
| POST   | `/api/programas`               | Crear programa      |
| PUT    | `/api/programas/{id}`          | Reemplazar programa |
| PATCH  | `/api/programas/{id}`          | Actualizar programa |
| DELETE | `/api/programas/{id}`          | Retirar programa    |
| GET    | `/api/areas-conocimiento`      | Listar áreas        |
| GET    | `/api/areas-conocimiento/{id}` | Obtener área        |
| POST   | `/api/areas-conocimiento`      | Crear área          |
| PUT    | `/api/areas-conocimiento/{id}` | Reemplazar área     |
| PATCH  | `/api/areas-conocimiento/{id}` | Actualizar área     |
| DELETE | `/api/areas-conocimiento/{id}` | Retirar área        |

---

# 20. Regla final

El contrato HTTP constituye la frontera entre frontend y backend.

Por lo tanto:

```text
Frontend
   ↓
HTTP + JSON
   ↓
API
```

El frontend no debe depender de:

```text
Repositorio
SQL
Dapper
Cadena de conexión
```

y la API no debe depender de componentes visuales de Blazor.

Cualquier modificación futura de estos contratos deberá documentarse en la
versión que introduzca el cambio.
