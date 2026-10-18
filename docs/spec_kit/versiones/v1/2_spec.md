# Especificación — Versión 1: Catálogos iniciales

> **Versión 1 — Gestión Profesoral**
>
> Esta versión implementa los primeros catálogos funcionales del sistema
> Gestión Profesoral, incluyendo API REST, persistencia en SQL Server y
> frontend desarrollado con Blazor.
>
> Ante cualquier diferencia entre la implementación y esta especificación,
> se deben respetar las reglas establecidas en `../../1_constitution.md`.

---

## 1. Propósito

El propósito de la versión 1 es implementar los primeros catálogos del
sistema Gestión Profesoral y establecer una arquitectura base reutilizable
para las siguientes versiones del proyecto.

Esta versión debe permitir administrar información básica utilizada
posteriormente por otros módulos relacionados con profesores, investigación,
producción académica y demás componentes del sistema.

Los recursos incluidos en esta versión son:

- Programa.
- Área de Conocimiento.
- Red.
- Línea de Investigación.
- Término Clave.

Cada recurso debe implementar las operaciones necesarias para consultar,
registrar, actualizar y retirar información.

---

## 2. Objetivos de la versión

Los objetivos principales de esta versión son:

1. Implementar los primeros catálogos del sistema.
2. Establecer la arquitectura base del backend.
3. Implementar persistencia utilizando SQL Server.
4. Utilizar Dapper para el acceso a datos.
5. Implementar una API REST utilizando ASP.NET Core.
6. Implementar una interfaz web utilizando Blazor.
7. Mantener separados el frontend y la API.
8. Implementar eliminación lógica mediante el campo `activo`.
9. Utilizar variables de entorno para información sensible.
10. Establecer un patrón reutilizable para las siguientes versiones.

---

## 3. Alcance funcional

La versión 1 comprende la administración de los siguientes recursos.

### 3.1 Programa

Permite administrar los programas académicos disponibles dentro del sistema.

Información principal:

- ID.
- Nombre.
- Tipo.
- Nivel.
- Fecha de creación.
- Fecha de cierre.
- Número de cohortes.
- Cantidad de graduados.
- Fecha de actualización.
- Ciudad.
- Facultad.
- Estado activo.

Operaciones:

- Consultar programas.
- Consultar un programa.
- Registrar programa.
- Actualizar programa.
- Retirar programa.

El retiro debe realizarse mediante eliminación lógica.

---

### 3.2 Área de Conocimiento

Permite administrar las áreas de conocimiento utilizadas para clasificar
información académica.

Información principal:

- ID.
- Gran área.
- Área.
- Disciplina.
- Estado activo.

Operaciones:

- Consultar áreas de conocimiento.
- Consultar un área específica.
- Registrar área.
- Actualizar área.
- Retirar área.

---

### 3.3 Red

Permite administrar las redes académicas o de investigación.

Información principal:

- ID.
- Nombre.
- URL.
- País.
- Estado activo.

Operaciones:

- Consultar redes.
- Consultar una red.
- Registrar red.
- Actualizar red.
- Retirar red.

El identificador de la red debe ser proporcionado durante el registro.

---

### 3.4 Línea de Investigación

Permite administrar las líneas de investigación utilizadas dentro del
sistema Gestión Profesoral.

Información principal:

- ID.
- Nombre.
- Descripción.
- Estado activo.

Operaciones:

- Consultar líneas de investigación.
- Consultar una línea específica.
- Registrar línea.
- Actualizar línea.
- Retirar línea.

El identificador es generado automáticamente por SQL Server.

---

### 3.5 Término Clave

Permite administrar términos utilizados para clasificar o identificar
información académica y de investigación.

Información principal:

- Término.
- Término en inglés.
- Estado activo.

Operaciones:

- Consultar términos clave.
- Consultar un término.
- Registrar término.
- Actualizar término.
- Retirar término.

El campo `termino` funciona como identificador principal del recurso.

---

## 4. Arquitectura

La aplicación debe mantener separación entre frontend, API, lógica de negocio
y persistencia.

El flujo general es:

```text
Usuario
   ↓
Frontend Blazor
   ↓
HTTP / JSON
   ↓
Controllers
   ↓
Servicios
   ↓
Repositorios
   ↓
Dapper
   ↓
SQL Server
```

El frontend no debe acceder directamente a SQL Server.

Toda operación sobre los datos debe realizarse a través de la API REST.

---

## 5. Backend

El backend debe desarrollarse utilizando:

- ASP.NET Core.
- .NET 8.
- Controllers.
- Servicios.
- Repositorios.
- Interfaces.
- Dapper.
- SQL Server.

La estructura esperada es:

```text
api_gestion/
│
├── Controllers/
├── Modelos/
├── Peticiones/
├── Repositorios/
├── Servicios/
├── Excepciones/
├── pruebas/
├── Program.cs
└── ApiGestion.csproj
```

---

## 6. Patrón de implementación

Cada recurso debe mantener, cuando corresponda, la siguiente separación:

```text
Controller
    ↓
IServicio
    ↓
Servicio
    ↓
IRepositorio
    ↓
Repositorio
    ↓
Dapper
    ↓
SQL Server
```

### Controller

Responsable de:

- Recibir solicitudes HTTP.
- Validar el formato de las solicitudes.
- Invocar el servicio correspondiente.
- Retornar respuestas HTTP.

### Servicio

Responsable de:

- Aplicar reglas de negocio.
- Coordinar operaciones.
- Validar existencia de recursos.
- Invocar los repositorios.

### Repositorio

Responsable de:

- Ejecutar consultas SQL.
- Utilizar Dapper.
- Consultar información.
- Insertar información.
- Actualizar información.
- Realizar eliminación lógica.

---

## 7. API REST

Los recursos principales deben estar disponibles mediante los siguientes
endpoints base:

```text
/api/programas
/api/areas-conocimiento
/api/redes
/api/lineas-investigacion
/api/terminos-clave
```

Los endpoints deben utilizar los verbos HTTP apropiados:

```text
GET
POST
PUT
PATCH
DELETE
```

La implementación exacta de cada operación debe respetar los contratos
definidos en `6_contracts.md`.

---

## 8. Persistencia

La persistencia debe realizarse utilizando SQL Server.

Base de datos utilizada:

```text
gestion_local
```

Tablas correspondientes a esta versión:

```text
programa
area_conocimiento
red
linea_investigacion
termino_clave
```

El acceso a la base de datos debe realizarse mediante Dapper.

No se debe utilizar Entity Framework para esta versión.

---

## 9. Eliminación lógica

Los registros no deben eliminarse físicamente de la base de datos cuando
se utiliza la operación Retirar.

La eliminación debe realizarse modificando:

```text
activo = 0
```

Los registros activos mantienen:

```text
activo = 1
```

Las consultas utilizadas por la aplicación deben considerar únicamente
los registros activos cuando corresponda.

---

## 10. Frontend

El frontend debe desarrollarse utilizando Blazor.

Debe ejecutarse como una aplicación independiente de la API y comunicarse
con ella mediante HTTP.

La estructura principal es:

```text
front_blazor/
│
├── Components/
│   ├── Layout/
│   └── Pages/
│
├── Servicios/
├── Program.cs
└── FrontGestion.csproj
```

---

## 11. Pantallas

La aplicación debe proporcionar acceso a:

- Inicio.
- Programas.
- Áreas de Conocimiento.
- Redes.
- Líneas de Investigación.
- Términos Clave.

La página de inicio debe proporcionar acceso a los cinco catálogos.

Cada catálogo debe mostrar:

1. Título del recurso.
2. Formulario de registro.
3. Mensajes de éxito o error.
4. Tabla de registros existentes.
5. Acción Editar.
6. Acción Retirar.

Durante una edición, el formulario debe cargar la información existente y
permitir actualizarla.

---

## 12. Comunicación frontend/API

El frontend debe comunicarse exclusivamente con la API.

Ejemplo:

```text
Blazor
   ↓
HttpClient
   ↓
http://localhost:5000/api/...
   ↓
API
```

Para desarrollo local:

```text
API
http://localhost:5000

Frontend
http://localhost:5100
```

No debe existir acceso directo desde Blazor hacia SQL Server.

---

## 13. Configuración y seguridad

La cadena de conexión no debe almacenarse directamente en el código fuente.

La API obtiene la conexión mediante:

```text
DB_CONNECTION
```

Los valores sensibles deben mantenerse fuera del repositorio Git.

El archivo:

```text
.env
```

no debe versionarse.

Debe proporcionarse únicamente un archivo de ejemplo:

```text
.env.example
```

sin contraseñas reales.

---

## 14. Docker

SQL Server debe poder ejecutarse mediante Docker.

El proyecto debe permitir iniciar la infraestructura necesaria mediante:

```bash
docker compose up -d
```

Los datos de SQL Server deben persistir mediante un volumen Docker.

La eliminación y recreación accidental del volumen debe evitarse cuando
existan datos que deban conservarse.

---

## 15. Requisitos funcionales

### RF-01

El sistema debe permitir registrar programas.

### RF-02

El sistema debe permitir consultar programas activos.

### RF-03

El sistema debe permitir actualizar programas.

### RF-04

El sistema debe permitir retirar programas mediante eliminación lógica.

### RF-05

El sistema debe permitir administrar áreas de conocimiento.

### RF-06

El sistema debe permitir administrar redes.

### RF-07

El sistema debe permitir administrar líneas de investigación.

### RF-08

El sistema debe permitir administrar términos clave.

### RF-09

El frontend debe consumir la información mediante la API REST.

### RF-10

Los registros retirados no deben aparecer en los listados normales.

---

## 16. Requisitos no funcionales

### RNF-01 — Arquitectura

Frontend, API y base de datos deben mantenerse separados.

### RNF-02 — Persistencia

SQL Server debe ser el sistema de persistencia.

### RNF-03 — Acceso a datos

El acceso a datos debe realizarse utilizando Dapper.

### RNF-04 — Seguridad

Las credenciales no deben almacenarse en Git.

### RNF-05 — Mantenibilidad

Los recursos deben seguir el patrón Controller → Servicio → Repositorio.

### RNF-06 — Interfaz

Las pantallas de los catálogos deben mantener una estructura visual
consistente.

### RNF-07 — Idioma

Los nombres visibles para el usuario, mensajes y documentación deben
mantenerse en español.

---

## 17. Criterios de aceptación

La versión 1 se considera funcional cuando:

- La API inicia correctamente.
- SQL Server está disponible.
- El frontend inicia correctamente.
- Programa puede registrarse, consultarse, editarse y retirarse.
- Área de Conocimiento puede registrarse, consultarse, editarse y retirarse.
- Red puede registrarse, consultarse, editarse y retirarse.
- Línea de Investigación puede registrarse, consultarse, editarse y retirarse.
- Término Clave puede registrarse, consultarse, editarse y retirarse.
- Los retiros son lógicos.
- El frontend consume exclusivamente la API.
- Los datos persisten en SQL Server.
- Swagger permite comprobar los endpoints.
- No existen credenciales reales almacenadas en Git.
- La solución compila sin errores.

---

## 18. Fuera del alcance

Esta versión no pretende implementar todavía todos los módulos del sistema
Gestión Profesoral.

Cualquier funcionalidad no definida expresamente en esta especificación
debe considerarse fuera del alcance de la versión 1 y deberá ser planificada
en una versión posterior.

No se deben agregar nuevos recursos únicamente por decisión del
desarrollador o de una herramienta de inteligencia artificial.

---

## 19. Definición de terminado

Una funcionalidad se considera terminada cuando:

1. Está implementada en SQL Server cuando requiere persistencia.
2. Tiene repositorio.
3. Tiene servicio.
4. Tiene controller.
5. Está disponible mediante la API.
6. Puede probarse mediante Swagger.
7. Está integrada con el frontend cuando corresponde.
8. Puede registrarse, consultarse y modificarse correctamente.
9. Implementa retiro lógico cuando corresponde.
10. No rompe funcionalidades existentes.
11. Cumple la Constitución del proyecto.
12. Cumple los contratos definidos para la versión.

---

## 20. Regla para desarrollo asistido por IA

Una herramienta de inteligencia artificial que participe en el desarrollo
de esta versión debe leer antes de programar:

1. `1_constitution.md`
2. `2_spec.md`
3. `3_plan.md`
4. `4_research.md`
5. `5_data_model.md`
6. `6_contracts.md`
7. `7_quickstart.md`
8. `8_tasks.md`
9. `9_checklist.md`

La IA no debe inventar tablas, campos, endpoints, dependencias ni reglas
que contradigan estos documentos.

Cuando exista una contradicción o falte información necesaria para
implementar una funcionalidad, debe indicarlo antes de modificar el código.
