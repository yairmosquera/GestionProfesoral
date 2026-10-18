# Plan técnico — Versión 1: Catálogos iniciales

> Este documento describe el plan técnico utilizado para implementar la
> versión 1 del sistema Gestión Profesoral.
>
> El alcance funcional se encuentra definido en `2_spec.md` y las reglas
> generales del proyecto en `../../1_constitution.md`.

---

## 1. Objetivo técnico

Implementar una primera versión funcional del sistema utilizando una
arquitectura separada en tres componentes principales:

1. SQL Server para persistencia.
2. API REST con ASP.NET Core.
3. Frontend con Blazor.

Los primeros recursos implementados son:

- Programa.
- Área de Conocimiento.

El objetivo es comprobar el flujo completo:

```text
Usuario
   ↓
Frontend Blazor
   ↓
HTTP / JSON
   ↓
API ASP.NET Core
   ↓
Servicios
   ↓
Repositorios
   ↓
Dapper
   ↓
SQL Server
```

---

# 2. Tecnologías

## Backend

La API utiliza:

- C#.
- ASP.NET Core.
- .NET 8.
- Dapper.
- Microsoft.Data.SqlClient.
- Swagger / Swashbuckle.

## Frontend

El frontend utiliza:

- C#.
- Blazor.
- Razor Components.
- HttpClient.
- CSS.

## Persistencia

La persistencia utiliza:

- SQL Server 2022.
- Scripts SQL.
- Volumen Docker para conservar los datos.

## Infraestructura

Se utiliza:

- Docker.
- Docker Compose.
- Git.
- GitHub.

---

# 3. Estructura general

La estructura principal del proyecto es:

```text
GestionProfesoral/
│
├── api_gestion/
│   ├── Controllers/
│   ├── Excepciones/
│   ├── Modelos/
│   ├── Peticiones/
│   ├── Repositorios/
│   ├── Servicios/
│   ├── pruebas/
│   ├── ApiGestion.csproj
│   ├── Program.cs
│   └── Dockerfile
│
├── front_blazor/
│   ├── Components/
│   │   ├── Layout/
│   │   └── Pages/
│   ├── Servicios/
│   ├── wwwroot/
│   ├── FrontGestion.csproj
│   ├── Program.cs
│   └── Dockerfile
│
├── db/
│
├── docs/
│   └── spec_kit/
│
├── .env.example
├── .gitignore
└── docker-compose.yml
```

---

# 4. Plan de base de datos

## 4.1 Motor

Se utiliza SQL Server.

La base utilizada es:

```text
gestion_local
```

SQL Server se ejecuta mediante Docker.

---

## 4.2 Tablas de la versión

Los recursos desarrollados en esta versión utilizan:

```text
programa
area_conocimiento
```

Otras tablas pueden existir dentro de la base de datos, pero no forman parte
del trabajo documentado para estos dos recursos hasta que una especificación
las incorpore.

---

## 4.3 Borrado lógico

Ambos recursos utilizan el campo:

```text
activo
```

La eliminación debe ejecutarse mediante una actualización:

```text
activo = 0
```

No se debe utilizar un `DELETE FROM` para retirar registros funcionalmente.

---

# 5. Plan de la API

## 5.1 Separación por capas

Cada recurso debe implementar el siguiente flujo:

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

Cada capa mantiene una responsabilidad específica.

---

## 5.2 Modelos

Se utilizan modelos para representar las entidades manejadas por la API.

Los modelos principales de esta versión son:

```text
Programa
AreaConocimiento
```

Estos modelos representan la información obtenida desde SQL Server.

---

## 5.3 Peticiones

Las peticiones HTTP utilizan clases específicas cuando se requiere distinguir
entre creación, reemplazo y actualización parcial.

El objetivo es evitar utilizar directamente la entidad como contrato para
todas las operaciones.

---

## 5.4 Repositorios

Los repositorios son responsables exclusivamente del acceso a datos.

Deben:

- Abrir la conexión necesaria.
- Ejecutar SQL explícito.
- Utilizar Dapper.
- Utilizar parámetros.
- Mapear resultados.
- Filtrar registros inactivos cuando corresponda.

No deben contener lógica relacionada con la interfaz gráfica.

---

## 5.5 Servicios

Los servicios contienen las reglas de negocio.

Deben comunicarse con los repositorios mediante sus interfaces.

Entre sus responsabilidades están:

- Preparar los modelos.
- Validar operaciones de negocio.
- Determinar si un recurso existe.
- Gestionar reemplazos.
- Gestionar actualizaciones parciales.
- Solicitar el retiro lógico.

---

## 5.6 Controladores

Los controladores exponen las operaciones HTTP.

Los recursos principales son:

```text
Programas
AreasConocimiento
```

Los controladores reciben las solicitudes, llaman al servicio correspondiente
y generan la respuesta HTTP.

---

# 6. Plan del recurso Programa

La implementación de Programa se divide en:

```text
Modelo Programa
      ↓
Peticiones Programa
      ↓
IRepositorioPrograma
      ↓
RepositorioPrograma
      ↓
IServicioPrograma
      ↓
ServicioPrograma
      ↓
ProgramaController
```

Las operaciones contempladas son:

```text
GET     listar
GET     obtener por ID
POST    crear
PUT     reemplazar
PATCH   actualizar parcialmente
DELETE  retirar
```

El identificador de Programa debe suministrarse durante la creación porque
la columna utilizada por el proyecto no genera automáticamente el ID.

---

# 7. Plan del recurso Área de Conocimiento

La implementación sigue el mismo patrón:

```text
Modelo AreaConocimiento
      ↓
Peticiones AreaConocimiento
      ↓
IRepositorioAreaConocimiento
      ↓
RepositorioAreaConocimiento
      ↓
IServicioAreaConocimiento
      ↓
ServicioAreaConocimiento
      ↓
AreaConocimientoController
```

El modelo utiliza:

```text
Id
GranArea
Area
Disciplina
Activo
```

El identificador es textual y admite códigos de hasta seis caracteres.

---

# 8. Plan del frontend

El frontend se mantiene separado de la API.

La API no se referencia directamente desde el proyecto Blazor.

La comunicación se realiza mediante:

```text
Blazor
   ↓
HttpClient
   ↓
JSON
   ↓
API
```

---

## 8.1 Servicios HTTP

Se utiliza un servicio por recurso.

```text
ServicioPrograma
ServicioAreaConocimiento
```

Cada servicio conoce únicamente los endpoints correspondientes a su recurso.

---

## 8.2 Página Programa

Ruta:

```text
/programas
```

La página debe:

- Cargar los programas activos.
- Mostrar un formulario.
- Crear programas.
- Permitir seleccionar un registro.
- Editar información.
- Retirar registros.
- Actualizar la tabla después de una operación.
- Mostrar mensajes al usuario.

---

## 8.3 Página Área de Conocimiento

Ruta:

```text
/areas-conocimiento
```

La página debe:

- Cargar áreas activas.
- Crear áreas.
- Editar áreas.
- Retirar áreas.
- Actualizar la tabla.
- Mostrar mensajes al usuario.

---

## 8.4 Interactividad

Las páginas que realizan operaciones requieren renderizado interactivo de
Blazor Server.

Se utiliza:

```razor
@rendermode InteractiveServer
```

La aplicación registra:

```csharp
AddRazorComponents()
AddInteractiveServerComponents()
```

y habilita el modo interactivo al mapear los componentes.

---

# 9. Plan de navegación

Se utiliza un diseño común mediante:

```text
MainLayout.razor
```

La navegación se encuentra en:

```text
NavMenu.razor
```

Las opciones iniciales son:

```text
Inicio
Programas
Áreas de Conocimiento
```

Las rutas son:

```text
/
/programas
/areas-conocimiento
```

---

# 10. Plan de presentación

Los estilos principales se almacenan en:

```text
wwwroot/app.css
wwwroot/marca.css
```

La interfaz debe proporcionar:

- Menú lateral.
- Encabezado.
- Formularios organizados.
- Tablas.
- Botones de acción.
- Mensajes de éxito.
- Mensajes de error.
- Diseño adaptable a diferentes tamaños de pantalla.

La presentación no debe modificar las reglas de negocio.

---

# 11. Configuración de la comunicación

La dirección base de la API se configura mediante:

```text
ApiUrl
```

El frontend registra un `HttpClient` independiente para cada servicio.

Ejemplo conceptual:

```text
ServicioPrograma
        ↓
     ApiUrl
        ↓
 /api/programas
```

y:

```text
ServicioAreaConocimiento
        ↓
     ApiUrl
        ↓
 /api/areas-conocimiento
```

---

# 12. Configuración sensible

Los secretos se mantienen fuera del repositorio mediante:

```text
.env
```

El archivo debe permanecer ignorado por Git.

Como referencia de configuración se utiliza:

```text
.env.example
```

No se deben almacenar contraseñas reales en la documentación del Spec Kit.

---

# 13. Plan de Docker

SQL Server se ejecuta mediante Docker.

El proyecto utiliza:

```text
docker-compose.yml
```

La base mantiene persistencia mediante un volumen.

El script ubicado en:

```text
db/
```

se utiliza para preparar la base requerida por el proyecto.

El diseño debe permitir posteriormente ejecutar los diferentes componentes
en contenedores independientes.

---

# 14. Orden de implementación

La versión se implementa en el siguiente orden:

## Etapa 1 — Infraestructura

- Preparar estructura del repositorio.
- Configurar Git.
- Configurar `.gitignore`.
- Configurar variables de entorno.
- Configurar Docker.
- Levantar SQL Server.

## Etapa 2 — Base de datos

- Crear `gestion_local`.
- Ejecutar el script del proyecto.
- Verificar las tablas.
- Revisar tipos de datos reales.

## Etapa 3 — API Programa

- Crear modelo.
- Crear peticiones.
- Crear interfaz de repositorio.
- Crear repositorio.
- Crear interfaz de servicio.
- Crear servicio.
- Crear controlador.
- Registrar dependencias.
- Compilar.
- Probar los endpoints.

## Etapa 4 — API Área de Conocimiento

- Revisar el esquema real.
- Crear modelo.
- Crear peticiones.
- Crear repositorio.
- Crear servicio.
- Crear controlador.
- Registrar dependencias.
- Compilar.
- Probar los endpoints.

## Etapa 5 — Frontend Programa

- Configurar Blazor.
- Configurar HttpClient.
- Crear `ServicioPrograma`.
- Crear `Programas.razor`.
- Habilitar interactividad.
- Probar creación.
- Probar edición.
- Probar retiro.

## Etapa 6 — Frontend Área de Conocimiento

- Crear `ServicioAreaConocimiento`.
- Registrar el servicio.
- Crear `AreasConocimiento.razor`.
- Probar creación.
- Probar edición.
- Probar retiro.

## Etapa 7 — Navegación y diseño

- Crear página Inicio.
- Configurar `MainLayout`.
- Configurar `NavMenu`.
- Agregar estilos.
- Comprobar navegación.

## Etapa 8 — Documentación

- Constitución.
- Mapa de versiones.
- Especificación.
- Plan técnico.
- Investigación.
- Modelo de datos.
- Contratos.
- Guía de ejecución.
- Tareas.
- Checklist.

---

# 15. Estrategia de pruebas

Cada recurso debe probarse primero desde la API.

Secuencia:

```text
POST
 ↓
GET
 ↓
GET por ID
 ↓
PUT
 ↓
PATCH
 ↓
DELETE
```

Después se debe repetir el flujo funcional desde Blazor.

Finalmente se debe verificar el efecto del borrado lógico directamente en
SQL Server cuando sea necesario.

---

# 16. Control de versiones

Cada integrante trabaja en su propia rama.

Los cambios deben guardarse únicamente después de:

```text
Compilar
   ↓
Probar
   ↓
Revisar git status
   ↓
git add
   ↓
git commit
   ↓
git push
```

La integración con la rama principal se realiza posteriormente mediante el
flujo acordado por el equipo.

---

# 17. Resultado esperado

Al terminar la versión debe existir una aplicación donde el usuario pueda
entrar a:

```text
/
```

navegar hacia:

```text
/programas
/areas-conocimiento
```

y administrar ambos catálogos desde la interfaz.

Cada modificación debe recorrer:

```text
Frontend
   ↓
API
   ↓
Servicio
   ↓
Repositorio
   ↓
SQL Server
```

sin que el frontend tenga acceso directo a la base de datos.

---

# 18. Condición para continuar

No se debe considerar finalizada la versión únicamente porque los proyectos
compilen.

Antes de continuar con la siguiente versión se debe verificar:

- API funcional.
- Frontend funcional.
- Persistencia correcta.
- Borrado lógico.
- Navegación.
- Configuración.
- Documentación.
- Checklist final.
