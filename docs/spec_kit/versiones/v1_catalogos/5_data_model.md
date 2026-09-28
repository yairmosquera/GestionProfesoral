# Modelo de datos — Versión 1: Catálogos iniciales

> Este documento describe las entidades de base de datos utilizadas por la
> versión 1 de Gestión Profesoral.
>
> El esquema físico se encuentra en SQL Server y la aplicación accede a los
> datos mediante Dapper.

---

# 1. Base de datos

La aplicación utiliza:

```text
gestion_local
```

Motor:

```text
Microsoft SQL Server
```

Las entidades documentadas en esta versión son:

```text
programa
area_conocimiento
```

---

# 2. Entidad Programa

La tabla:

```text
programa
```

almacena la información de los programas académicos.

## 2.1 Campos

| Campo SQL             | Tipo utilizado | C#        | Descripción                        |
| --------------------- | -------------- | --------- | ---------------------------------- |
| `id`                  | INT            | `int`     | Identificador del programa         |
| `nombre`              | Texto          | `string`  | Nombre del programa                |
| `tipo`                | Texto          | `string`  | Tipo de programa                   |
| `nivel`               | Texto          | `string`  | Nivel académico                    |
| `fecha_creacion`      | Texto          | `string`  | Fecha de creación                  |
| `fecha_cierre`        | Texto / NULL   | `string?` | Fecha de cierre                    |
| `numero_cohortes`     | Texto          | `string`  | Número de cohortes                 |
| `cant_graduados`      | Texto          | `string`  | Cantidad de graduados              |
| `fecha_actualizacion` | Texto          | `string`  | Fecha de actualización             |
| `ciudad`              | Texto          | `string`  | Ciudad del programa                |
| `facultad`            | INT            | `int`     | Identificador numérico de facultad |
| `activo`              | BIT            | `bool`    | Estado lógico del registro         |

---

## 2.2 Llave primaria

La llave primaria es:

```text
id
```

El campo no se genera automáticamente mediante `IDENTITY`.

Por esta razón, al crear un Programa la aplicación debe proporcionar el
identificador.

Ejemplo:

```json
{
  "id": 101,
  "nombre": "Ingeniería Ambiental"
}
```

---

## 2.3 Fecha de cierre

El campo:

```text
fecha_cierre
```

puede representar un valor ausente.

Esto permite que un programa que todavía se encuentre abierto no tenga una
fecha de cierre.

En C# se representa mediante:

```csharp
public string? FechaCierre { get; set; }
```

---

## 2.4 Fechas

En la estructura utilizada actualmente por el proyecto los campos de fecha
se manejan como texto.

Los campos son:

```text
fecha_creacion
fecha_cierre
fecha_actualizacion
```

La versión 1 no realiza cálculos, diferencias ni ordenamientos cronológicos
basados en estos campos.

---

## 2.5 Estado lógico

El campo:

```text
activo
```

determina si el registro se encuentra disponible para las operaciones
normales.

```text
activo = 1 → registro activo
activo = 0 → registro retirado
```

Los listados normales deben consultar únicamente registros activos.

---

# 3. Modelo C# de Programa

La representación utilizada por la aplicación contiene:

```csharp
public class Programa
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public string Nivel { get; set; } = string.Empty;

    public string FechaCreacion { get; set; } = string.Empty;

    public string? FechaCierre { get; set; }

    public string NumeroCohortes { get; set; } = string.Empty;

    public string CantGraduados { get; set; } = string.Empty;

    public string FechaActualizacion { get; set; } = string.Empty;

    public string Ciudad { get; set; } = string.Empty;

    public int Facultad { get; set; }

    public bool Activo { get; set; } = true;
}
```

---

# 4. Correspondencia Programa SQL → C# → JSON

| SQL Server            | C#                   | JSON                 |
| --------------------- | -------------------- | -------------------- |
| `id`                  | `Id`                 | `id`                 |
| `nombre`              | `Nombre`             | `nombre`             |
| `tipo`                | `Tipo`               | `tipo`               |
| `nivel`               | `Nivel`              | `nivel`              |
| `fecha_creacion`      | `FechaCreacion`      | `fechaCreacion`      |
| `fecha_cierre`        | `FechaCierre`        | `fechaCierre`        |
| `numero_cohortes`     | `NumeroCohortes`     | `numeroCohortes`     |
| `cant_graduados`      | `CantGraduados`      | `cantGraduados`      |
| `fecha_actualizacion` | `FechaActualizacion` | `fechaActualizacion` |
| `ciudad`              | `Ciudad`             | `ciudad`             |
| `facultad`            | `Facultad`           | `facultad`           |
| `activo`              | `Activo`             | `activo`             |

---

# 5. Ejemplo de Programa

Ejemplo representativo:

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

Los valores anteriores son únicamente un ejemplo de estructura y no implican
datos obligatorios de producción.

---

# 6. Entidad Área de Conocimiento

La tabla:

```text
area_conocimiento
```

almacena la clasificación de las diferentes áreas y disciplinas de
conocimiento.

---

## 6.1 Campos

| Campo SQL    | Tipo SQL       | C#       | Obligatorio |
| ------------ | -------------- | -------- | ----------- |
| `id`         | `VARCHAR(6)`   | `string` | Sí          |
| `gran_area`  | `VARCHAR(60)`  | `string` | Sí          |
| `area`       | `VARCHAR(60)`  | `string` | Sí          |
| `disciplina` | `VARCHAR(150)` | `string` | Sí          |
| `activo`     | `BIT`          | `bool`   | Sí          |

---

## 6.2 Llave primaria

La llave utilizada para identificar el Área de Conocimiento es:

```text
id
```

Su tipo es:

```text
VARCHAR(6)
```

Por esta razón se permiten códigos alfanuméricos.

Ejemplos:

```text
AC001
AC002
```

---

# 7. Modelo C# de Área de Conocimiento

La representación utilizada por la aplicación es:

```csharp
public class AreaConocimiento
{
    public string Id { get; set; } = string.Empty;

    public string GranArea { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string Disciplina { get; set; } = string.Empty;

    public bool Activo { get; set; }
}
```

---

# 8. Correspondencia Área SQL → C# → JSON

| SQL Server   | C#           | JSON         |
| ------------ | ------------ | ------------ |
| `id`         | `Id`         | `id`         |
| `gran_area`  | `GranArea`   | `granArea`   |
| `area`       | `Area`       | `area`       |
| `disciplina` | `Disciplina` | `disciplina` |
| `activo`     | `Activo`     | `activo`     |

---

# 9. Ejemplo de Área de Conocimiento

```json
{
  "id": "AC002",
  "granArea": "Ingeniería y Tecnología",
  "area": "Ingeniería Ambiental",
  "disciplina": "Gestión Ambiental",
  "activo": true
}
```

Este objeto representa la estructura del recurso.

---

# 10. Reglas de creación de Programa

Para crear un Programa se proporciona:

```text
id
nombre
tipo
nivel
fechaCreacion
fechaCierre
numeroCohortes
cantGraduados
fechaActualizacion
ciudad
facultad
```

`fechaCierre` puede estar vacía cuando corresponda.

El campo:

```text
activo
```

es administrado por la aplicación y el registro nuevo debe quedar activo.

---

# 11. Reglas de creación de Área de Conocimiento

Para crear un Área se requieren:

```text
id
granArea
area
disciplina
```

No se debe permitir que estos campos obligatorios lleguen vacíos.

El registro nuevo debe quedar activo.

---

# 12. Reglas de actualización

## PUT

PUT representa el reemplazo de la información editable del recurso.

El identificador se recibe mediante la ruta.

Ejemplo:

```text
/api/programas/101
```

El ID no debe cambiar como consecuencia del reemplazo.

---

## PATCH

PATCH permite modificar únicamente determinados campos.

Ejemplo conceptual:

```json
{
  "ciudad": "Medellín"
}
```

Los demás valores deben permanecer sin cambios.

---

# 13. Borrado lógico

Programa y Área de Conocimiento utilizan borrado lógico.

La operación conceptual es:

```sql
UPDATE recurso
SET activo = 0
WHERE id = @Id;
```

No se debe realizar:

```sql
DELETE FROM recurso
WHERE id = @Id;
```

---

# 14. Consulta de registros activos

Los listados deben considerar:

```sql
WHERE activo = 1
```

Esto evita mostrar registros que ya fueron retirados.

---

# 15. Relaciones en la versión 1

Esta versión no incorpora nuevas relaciones únicamente por inferencia.

Aunque algunos campos puedan representar referencias hacia otros catálogos,
solo se implementarán relaciones cuando estén definidas por el esquema y por
la versión correspondiente.

Por ejemplo:

```text
programa.facultad
```

se mantiene como el valor numérico definido actualmente por el modelo.

---

# 16. Integridad de los datos

Las operaciones deben respetar:

- Tipo de dato del identificador.
- Longitud de los campos.
- Campos obligatorios.
- Llaves primarias.
- Estado lógico.
- Nulabilidad definida por el esquema.

La aplicación no debe inventar valores para completar información que no haya
sido proporcionada.

---

# 17. Resumen

## Programa

```text
PK: id
Tipo PK: INT
ID automático: No
Borrado: Lógico
Campo de estado: activo
```

## Área de Conocimiento

```text
PK: id
Tipo PK: VARCHAR(6)
ID automático: No
Borrado: Lógico
Campo de estado: activo
```

---

# 18. Conclusión

El modelo de datos de la versión 1 permite administrar los catálogos Programa
y Área de Conocimiento respetando el esquema existente de SQL Server.

La aplicación mantiene una correspondencia clara entre:

```text
SQL Server
    ↓
Modelo C#
    ↓
JSON
    ↓
Frontend
```

Los cambios futuros al modelo deberán documentarse en la versión que los
introduzca antes de incorporarse al sistema.
