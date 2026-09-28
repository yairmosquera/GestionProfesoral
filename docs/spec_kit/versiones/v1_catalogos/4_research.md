# Investigación y decisiones técnicas — Versión 1

> Este documento registra las decisiones técnicas, validaciones y problemas
> encontrados durante la implementación de la versión 1 de Gestión Profesoral.
>
> Las decisiones aquí documentadas complementan `2_spec.md` y `3_plan.md`.

---

# 1. Objetivo de la investigación

Durante la construcción de la versión 1 fue necesario validar el esquema real
de la base de datos, la comunicación entre los componentes y el comportamiento
de ASP.NET Core, Blazor, Docker y SQL Server.

Los principales temas investigados fueron:

- Esquema real de `programa`.
- Esquema real de `area_conocimiento`.
- Generación del ID de Programa.
- Borrado lógico.
- Acceso a SQL Server mediante Dapper.
- Comunicación entre frontend y API.
- Configuración de Blazor interactivo.
- Separación de puertos.
- Manejo de variables de entorno.
- Integración mediante Git.

---

# 2. Decisión — Utilizar SQL Server con Docker

## Problema

El proyecto necesita una instancia de SQL Server reproducible entre los
integrantes del equipo.

Depender de una instalación manual de SQL Server en cada computador podría
generar diferencias de configuración.

## Decisión

Ejecutar SQL Server mediante Docker utilizando la imagen oficial de SQL
Server 2022.

La persistencia se conserva mediante un volumen Docker.

## Resultado

El equipo puede levantar el motor de base de datos mediante Docker y utilizar
la misma estructura de base de datos.

---

# 3. Decisión — Base de datos `gestion_local`

Se utiliza la base:

```text
gestion_local
```

La creación y preparación de la base se realiza mediante los archivos
ubicados en:

```text
db/
```

Durante la ejecución se verificó la existencia de las tablas necesarias para
la primera etapa del proyecto.

Entre ellas:

```text
programa
area_conocimiento
linea_investigacion
red
termino_clave
```

La versión documentada trabaja inicialmente con:

```text
programa
area_conocimiento
```

---

# 4. Investigación — El ID de Programa no es automático

## Problema encontrado

Inicialmente se consideró que SQL Server podría generar automáticamente el
identificador de Programa.

Al realizar las pruebas se comprobó que:

```text
programa.id
```

no utiliza `IDENTITY`.

Por esta razón, intentar crear un programa sin proporcionar un ID no
correspondía con el esquema real.

## Decisión

El ID debe formar parte de la petición de creación.

El frontend también debe solicitarlo al usuario.

Ejemplo:

```json
{
  "id": 2,
  "nombre": "Ingeniería Ambiental"
}
```

## Consecuencia

El repositorio debe incluir explícitamente `id` dentro del `INSERT`.

No se utiliza:

```text
SCOPE_IDENTITY()
```

para obtener el identificador del Programa.

---

# 5. Investigación — Esquema de Área de Conocimiento

## Problema encontrado

Durante las primeras pruebas de creación de un Área de Conocimiento la API
respondió con un error interno.

Inicialmente se estaba trabajando con una representación incompleta del
esquema.

## Validación realizada

Se consultó la información de columnas de SQL Server.

Se comprobó que `area_conocimiento` contiene:

```text
id
gran_area
area
disciplina
activo
```

## Decisión

Actualizar:

- Modelo.
- Peticiones.
- Repositorio.
- Servicio.
- Frontend.

para trabajar con todos los campos requeridos.

## Resultado

Después de realizar el ajuste, la creación y el resto de las operaciones
CRUD funcionaron correctamente.

---

# 6. Investigación — Tipo del ID de Área de Conocimiento

## Problema

El identificador del Área de Conocimiento no debe tratarse como un número.

Los datos utilizan códigos alfanuméricos.

Ejemplos:

```text
AC001
AC002
```

## Decisión

Representar el identificador mediante:

```csharp
string
```

La longitud máxima utilizada es de seis caracteres.

## Consecuencia

Los métodos del recurso utilizan `string id`.

Ejemplo conceptual:

```csharp
Obtener(string id)
Eliminar(string id)
```

---

# 7. Investigación — Longitud de Disciplina

Durante la revisión del esquema se confirmó que el campo:

```text
disciplina
```

debe permitir hasta 150 caracteres.

## Decisión

Mantener la longitud definida por el esquema utilizado por el proyecto.

Esto evita truncar nombres de disciplinas que puedan superar longitudes más
pequeñas.

---

# 8. Decisión — Borrado lógico

## Requisito

Los registros no deben eliminarse físicamente de SQL Server.

## Alternativa descartada

```sql
DELETE FROM programa
WHERE id = @Id;
```

Esta operación eliminaría permanentemente la fila.

## Solución

Utilizar:

```sql
UPDATE programa
SET activo = 0
WHERE id = @Id
  AND activo = 1;
```

El mismo principio se aplica a Área de Conocimiento.

## Resultado

Después de retirar un registro:

- La fila continúa en SQL Server.
- `activo` queda en `0`.
- El registro deja de aparecer en los listados normales.

---

# 9. Decisión — Dapper en lugar de Entity Framework

## Objetivo

Mantener las consultas SQL visibles y controlar explícitamente el acceso a
datos.

## Decisión

Utilizar:

```text
Dapper
Microsoft.Data.SqlClient
```

## Regla

Las consultas deben utilizar parámetros.

Ejemplo:

```sql
SELECT *
FROM programa
WHERE id = @Id;
```

No se deben construir consultas concatenando datos proporcionados por el
usuario.

---

# 10. Decisión — Separación por capas

Se decidió mantener la API organizada mediante:

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
SQL Server
```

## Razón

La separación permite:

- Evitar SQL dentro de los controladores.
- Mantener reglas de negocio fuera del repositorio.
- Facilitar pruebas.
- Facilitar mantenimiento.
- Sustentar claramente la responsabilidad de cada clase.

---

# 11. Investigación — Conexión desde Windows hacia SQL Server Docker

## Situación

Durante el desarrollo, la API puede ejecutarse directamente desde Windows
mientras SQL Server se encuentra dentro de Docker.

## Decisión

Cuando la API se ejecuta desde el equipo anfitrión, SQL Server se alcanza
mediante el puerto publicado en `localhost`.

Conceptualmente:

```text
API en Windows
      ↓
localhost:puerto-publicado
      ↓
SQL Server Docker
```

Cuando ambos componentes se ejecuten dentro de Docker Compose, la
comunicación podrá realizarse utilizando el nombre del servicio de SQL Server
dentro de la red Docker.

## Conclusión

`localhost` y el nombre del servicio Docker no son intercambiables en todos
los escenarios; dependen de dónde se esté ejecutando la aplicación.

---

# 12. Investigación — Conflicto de puertos entre API y frontend

## Problema encontrado

Inicialmente el frontend intentó utilizar el mismo puerto que la API.

Esto produjo un error equivalente a:

```text
address already in use
```

porque la API ya estaba escuchando en ese puerto.

## Decisión

Durante el desarrollo local se utilizan puertos separados:

```text
API:       5000
Frontend:  5100
```

## Resultado

Ambas aplicaciones pueden ejecutarse simultáneamente.

---

# 13. Investigación — Configuración de `ApiUrl`

El frontend necesita conocer la dirección de la API.

## Decisión

La dirección se configura mediante:

```json
{
  "ApiUrl": "http://localhost:5000"
}
```

El valor se obtiene desde la configuración del frontend.

## Beneficio

Los servicios no necesitan repetir la dirección completa de la API en cada
operación.

---

# 14. Decisión — Un servicio HTTP por recurso

## Alternativa considerada

Crear un servicio genérico que reciba el nombre de cualquier tabla.

## Problema

Un servicio genérico aumenta el acoplamiento con la estructura interna de la
base de datos y reduce la claridad de los contratos.

## Decisión

Utilizar:

```text
ServicioPrograma
ServicioAreaConocimiento
```

Cada uno conoce únicamente las rutas de su recurso.

## Resultado

La intención del código es más clara y cada recurso puede evolucionar
independientemente.

---

# 15. Investigación — Blazor mostraba la página pero los botones no actuaban

## Problema encontrado

La página de Programa se visualizaba correctamente, pero al presionar el
botón Guardar no se ejecutaba la operación.

La página estaba siendo renderizada sin interactividad.

## Decisión

Habilitar Blazor Server interactivo.

En las páginas que necesitan interacción se utiliza:

```razor
@rendermode InteractiveServer
```

La aplicación registra:

```csharp
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
```

y:

```csharp
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

## Resultado

Los eventos:

```razor
@onclick
```

comenzaron a ejecutarse correctamente.

---

# 16. Investigación — `InteractiveServer` no reconocido

## Problema

Después de agregar:

```razor
@rendermode InteractiveServer
```

el proyecto inicialmente no reconocía el nombre `InteractiveServer`.

## Decisión

Importar los modos de renderizado mediante `_Imports.razor`:

```razor
@using static Microsoft.AspNetCore.Components.Web.RenderMode
```

## Resultado

Las páginas pudieron utilizar:

```razor
@rendermode InteractiveServer
```

sin errores de compilación.

---

# 17. Investigación — Página principal respondía 404

## Problema

Las rutas:

```text
/programas
/areas-conocimiento
```

funcionaban, pero acceder a:

```text
/
```

respondía 404.

## Causa

No existía una página Razor con:

```razor
@page "/"
```

## Decisión

Configurar `Home.razor` como página inicial.

## Resultado

La ruta raíz funciona y permite navegar hacia los módulos disponibles.

---

# 18. Decisión — Layout común

Para evitar repetir la estructura visual en cada página se utiliza:

```text
MainLayout.razor
```

La navegación se concentra en:

```text
NavMenu.razor
```

## Opciones iniciales

```text
Inicio
Programas
Áreas de Conocimiento
```

Esto permite que todas las páginas compartan la misma estructura.

---

# 19. Decisión — Modelos independientes entre frontend y API

Aunque ambos proyectos utilizan C#, el frontend no referencia directamente
los modelos de la API.

## Razón

El único contrato entre ambos componentes debe ser HTTP/JSON.

Esto evita que:

```text
Frontend
   ↓
dependa directamente del ensamblado de la API
```

## Resultado

El frontend define sus propios modelos de datos para interpretar las
respuestas recibidas.

---

# 20. Investigación — Respuesta de la API en el frontend

Durante las pruebas se confirmó que el frontend puede realizar solicitudes
HTTP hacia:

```text
/api/programas
```

y:

```text
/api/areas-conocimiento
```

Los servicios utilizan `HttpClient` y serialización JSON.

## Resultado

Se comprobó el flujo:

```text
Blazor
  ↓
HttpClient
  ↓
API
  ↓
SQL Server
```

---

# 21. Decisión — Manejo de errores del frontend

Las llamadas HTTP se encapsulan dentro de los servicios.

Cuando ocurre un problema, el servicio devuelve un resultado que permite
mostrar un mensaje comprensible en la interfaz.

El objetivo es evitar que una excepción de comunicación provoque que toda la
página deje de funcionar.

---

# 22. Investigación — Archivos `.env`

El proyecto utiliza variables de entorno para información sensible.

## Riesgo

Subir `.env` al repositorio puede exponer credenciales.

## Decisión

Mantener:

```text
.env
```

fuera de Git mediante `.gitignore`.

Mantener:

```text
.env.example
```

dentro del repositorio como referencia.

## Regla

La documentación no debe incluir contraseñas reales.

---

# 23. Investigación — Integración mediante Git

El proyecto es desarrollado por varios integrantes.

Cada integrante utiliza su propia rama.

Durante la integración pueden aparecer conflictos cuando dos ramas modifican
el mismo archivo.

## Decisión

Los conflictos deben resolverse revisando ambos cambios y conservando las
dependencias necesarias.

Después de resolver un conflicto se debe ejecutar nuevamente:

```text
dotnet restore
dotnet build
```

antes de confirmar la integración.

---

# 24. Decisiones finales de la versión

| Tema                      | Decisión                        |
| ------------------------- | ------------------------------- |
| Backend                   | ASP.NET Core .NET 8             |
| Frontend                  | Blazor                          |
| Base de datos             | SQL Server                      |
| Acceso a datos            | Dapper                          |
| ORM completo              | No                              |
| SQL                       | Explícito y parametrizado       |
| Borrado                   | Lógico mediante `activo`        |
| ID Programa               | Proporcionado por la aplicación |
| ID Área                   | Texto, máximo 6 caracteres      |
| Comunicación frontend/API | HTTP + JSON                     |
| Servicio frontend         | Uno por recurso                 |
| Secretos                  | Variables de entorno            |
| Desarrollo local API      | Puerto 5000                     |
| Desarrollo local frontend | Puerto 5100                     |

---

# 25. Conclusión

La implementación de la versión 1 permitió identificar diferencias entre las
suposiciones iniciales y el comportamiento real del sistema.

Las decisiones más importantes fueron:

1. Respetar el esquema real de SQL Server.
2. Proporcionar manualmente el ID de Programa.
3. Incorporar `gran_area` y `area` al recurso Área de Conocimiento.
4. Mantener borrado lógico.
5. Separar frontend, API y base de datos.
6. Utilizar un servicio HTTP por recurso.
7. Habilitar correctamente Blazor interactivo.
8. Mantener API y frontend en puertos independientes.
9. Mantener los secretos fuera del repositorio.

Estas decisiones constituyen la base técnica para continuar desarrollando los
siguientes recursos del sistema.
