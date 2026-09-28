# 🎓 Sistema de Gestión Profesoral

Sistema web para la gestión de información académica y profesoral, desarrollado como proyecto académico.

La solución utiliza una arquitectura separada entre:

- Base de datos SQL Server.
- API REST con ASP.NET Core.
- Frontend con Blazor.
- Docker para infraestructura.
- Dapper para acceso a datos.

---

## 📌 Arquitectura

El sistema utiliza la siguiente arquitectura:

```text
┌──────────────────────┐
│       Usuario        │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│   Frontend Blazor    │
│ localhost:5100       │
└──────────┬───────────┘
           │ HTTP / JSON
           ▼
┌──────────────────────┐
│   API ASP.NET Core   │
│ localhost:5000       │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│      Servicios       │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│     Repositorios     │
│       Dapper         │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│      SQL Server      │
│       Docker         │
└──────────────────────┘
```

El frontend nunca accede directamente a SQL Server.

---

# 🛠️ Tecnologías utilizadas

## Backend

- C#
- .NET 8
- ASP.NET Core
- API REST
- Dapper
- Microsoft.Data.SqlClient
- Swagger

## Frontend

- Blazor
- Razor Components
- Interactive Server
- HttpClient
- HTML
- CSS

## Base de datos

- Microsoft SQL Server 2022
- Docker

## Herramientas

- Docker Compose
- Git
- GitHub
- Visual Studio Code

---

# 📂 Estructura del proyecto

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
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```

---

# 📚 Recursos del sistema

En la primera etapa se implementaron los recursos:

### Programa

Permite administrar los programas académicos.

Operaciones disponibles:

- Consultar.
- Consultar por ID.
- Crear.
- Reemplazar.
- Actualizar parcialmente.
- Retirar.

### Área de Conocimiento

Permite administrar las áreas y disciplinas de conocimiento.

Operaciones disponibles:

- Consultar.
- Consultar por ID.
- Crear.
- Reemplazar.
- Actualizar parcialmente.
- Retirar.

Los registros utilizan borrado lógico mediante el campo:

```text
activo
```

Por lo tanto:

```text
activo = 1 → Activo
activo = 0 → Retirado
```

Los registros retirados permanecen almacenados en SQL Server.

---

# 🚀 Ejecución rápida

## 1. Requisitos

Se necesita tener instalado:

- Git
- Docker Desktop
- .NET 8 SDK

Verificar .NET:

```bash
dotnet --version
```

Verificar Docker:

```bash
docker --version
docker compose version
```

---

# 2. Configurar variables de entorno

El proyecto contiene:

```text
.env.example
```

Crear una copia:

```powershell
Copy-Item .env.example .env
```

Después configurar las variables necesarias.

> ⚠️ `.env` puede contener información sensible y no debe subirse a Git.

---

# 3. Levantar SQL Server

Desde la raíz:

```bash
docker compose up -d
```

Comprobar:

```bash
docker compose ps
```

La base utilizada es:

```text
gestion_local
```

---

# 4. Ejecutar la API

Restaurar dependencias:

```bash
dotnet restore api_gestion/ApiGestion.csproj
```

Compilar:

```bash
dotnet build api_gestion/ApiGestion.csproj
```

Ejecutar:

```bash
dotnet run --project api_gestion/ApiGestion.csproj
```

Durante el desarrollo local:

```text
http://localhost:5000
```

---

# 📖 Swagger

La documentación interactiva de la API se encuentra en:

```text
http://localhost:5000/swagger/index.html
```

Desde Swagger es posible ejecutar las operaciones REST sin utilizar el
frontend.

---

# 🔌 API REST

## Programa

Ruta base:

```text
/api/programas
```

Endpoints:

| Método | Ruta                  | Descripción           |
| ------ | --------------------- | --------------------- |
| GET    | `/api/programas`      | Listar programas      |
| GET    | `/api/programas/{id}` | Consultar programa    |
| POST   | `/api/programas`      | Crear programa        |
| PUT    | `/api/programas/{id}` | Reemplazar programa   |
| PATCH  | `/api/programas/{id}` | Actualización parcial |
| DELETE | `/api/programas/{id}` | Retirar programa      |

---

## Ejemplo — Listar programas

```http
GET /api/programas
```

Ejemplo de respuesta:

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
    "fechaActualizacion": "2026-09-27",
    "ciudad": "Medellín",
    "facultad": 1,
    "activo": true
  }
]
```

---

## Ejemplo — Crear programa

```http
POST /api/programas
Content-Type: application/json
```

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
  "fechaActualizacion": "2026-09-27",
  "ciudad": "Medellín",
  "facultad": 1
}
```

> `programa.id` no es generado automáticamente. Debe proporcionarse durante
> la creación.

---

## Ejemplo — Consultar programa

```http
GET /api/programas/101
```

---

## Ejemplo — Actualizar programa

```http
PUT /api/programas/101
Content-Type: application/json
```

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

---

## Ejemplo — Actualización parcial

```http
PATCH /api/programas/101
Content-Type: application/json
```

```json
{
  "ciudad": "Medellín"
}
```

---

## Ejemplo — Retirar programa

```http
DELETE /api/programas/101
```

Esta operación realiza borrado lógico:

```text
activo = 0
```

---

# 🧠 API — Áreas de Conocimiento

Ruta base:

```text
/api/areas-conocimiento
```

Endpoints:

| Método | Ruta                           | Descripción           |
| ------ | ------------------------------ | --------------------- |
| GET    | `/api/areas-conocimiento`      | Listar áreas          |
| GET    | `/api/areas-conocimiento/{id}` | Consultar área        |
| POST   | `/api/areas-conocimiento`      | Crear área            |
| PUT    | `/api/areas-conocimiento/{id}` | Reemplazar área       |
| PATCH  | `/api/areas-conocimiento/{id}` | Actualización parcial |
| DELETE | `/api/areas-conocimiento/{id}` | Retirar área          |

---

## Ejemplo — Crear Área de Conocimiento

```http
POST /api/areas-conocimiento
Content-Type: application/json
```

```json
{
  "id": "AC002",
  "granArea": "Ingeniería y Tecnología",
  "area": "Ingeniería Ambiental",
  "disciplina": "Gestión Ambiental"
}
```

El ID es alfanumérico.

Ejemplos:

```text
AC001
AC002
AC003
```

---

## Ejemplo — Consultar área

```http
GET /api/areas-conocimiento/AC002
```

Ejemplo de respuesta:

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

## Ejemplo — Actualizar área

```http
PUT /api/areas-conocimiento/AC002
Content-Type: application/json
```

```json
{
  "granArea": "Ingeniería y Tecnología",
  "area": "Ciencias Ambientales",
  "disciplina": "Gestión Ambiental"
}
```

---

## Ejemplo — PATCH

```http
PATCH /api/areas-conocimiento/AC002
Content-Type: application/json
```

```json
{
  "disciplina": "Gestión de Recursos Naturales"
}
```

---

## Ejemplo — Retirar área

```http
DELETE /api/areas-conocimiento/AC002
```

El registro permanece almacenado con:

```text
activo = 0
```

---

# 🌐 Ejecutar frontend

Con la API funcionando, abrir otra terminal.

Restaurar:

```bash
dotnet restore front_blazor/FrontGestion.csproj
```

Compilar:

```bash
dotnet build front_blazor/FrontGestion.csproj
```

Ejecutar:

```bash
dotnet run --project front_blazor/FrontGestion.csproj
```

El frontend estará disponible en:

```text
http://localhost:5100
```

---

# 🖥️ Rutas del frontend

| Ruta                  | Descripción          |
| --------------------- | -------------------- |
| `/`                   | Página principal     |
| `/programas`          | Gestión de programas |
| `/areas-conocimiento` | Gestión de áreas     |

Ejemplo:

```text
http://localhost:5100/programas
```

---

# 🔄 Flujo de una operación

Por ejemplo, al registrar un programa:

```text
Usuario
   ↓
Programas.razor
   ↓
ServicioPrograma
   ↓
POST /api/programas
   ↓
ProgramaController
   ↓
ServicioPrograma API
   ↓
RepositorioPrograma
   ↓
Dapper
   ↓
SQL Server
```

Después la página vuelve a consultar la información y actualiza la tabla.

---

# 🗑️ Borrado lógico

El sistema no elimina físicamente los registros de los catálogos.

Ejemplo conceptual:

```sql
UPDATE programa
SET activo = 0
WHERE id = @Id;
```

En lugar de:

```sql
DELETE FROM programa
WHERE id = @Id;
```

Esto permite conservar la información histórica.

---

# 🐳 Comandos Docker útiles

Levantar servicios:

```bash
docker compose up -d
```

Ver estado:

```bash
docker compose ps
```

Ver logs:

```bash
docker compose logs
```

Logs de SQL Server:

```bash
docker compose logs sqlserver
```

Detener:

```bash
docker compose down
```

---

# 🧪 Orden recomendado para probar

```text
1. Iniciar Docker Desktop
        ↓
2. docker compose up -d
        ↓
3. Ejecutar API
        ↓
4. Abrir Swagger
        ↓
5. Probar GET de los recursos
        ↓
6. Ejecutar frontend
        ↓
7. Abrir página principal
        ↓
8. Probar Programa
        ↓
9. Probar Área de Conocimiento
```

---

# ⚠️ Problemas comunes

## Swagger no abre

Verificar que la API esté ejecutándose:

```bash
dotnet run --project api_gestion/ApiGestion.csproj
```

---

## Frontend abre pero no carga información

Comprobar primero:

```text
http://localhost:5000/swagger/index.html
```

Si Swagger no funciona, el frontend tampoco podrá consultar la API.

También verificar:

```text
front_blazor/appsettings.json
```

y confirmar:

```json
"ApiUrl": "http://localhost:5000"
```

---

## Puerto ocupado

La configuración local utiliza:

```text
API       → 5000
Frontend  → 5100
SQL       → 1433
```

Cada aplicación debe utilizar un puerto diferente.

---

## Los botones de Blazor no funcionan

Las páginas interactivas utilizan:

```razor
@rendermode InteractiveServer
```

También debe estar habilitado:

```csharp
AddInteractiveServerComponents()
```

---

## SQL Server no responde

Ejecutar:

```bash
docker compose ps
```

y:

```bash
docker compose logs sqlserver
```

---

# 📑 Documentación Spec Kit

La documentación técnica y funcional se encuentra en:

```text
docs/spec_kit/
```

Estructura:

```text
docs/spec_kit/
├── 1_constitution.md
└── versiones/
    ├── 0_mapa_versiones.md
    └── v1_catalogos/
        ├── 2_spec.md
        ├── 3_plan.md
        ├── 4_research.md
        ├── 5_data_model.md
        ├── 6_contracts.md
        ├── 7_quickstart.md
        ├── 8_tasks.md
        └── 9_checklist.md
```

Documentos principales:

| Documento             | Propósito                     |
| --------------------- | ----------------------------- |
| `1_constitution.md`   | Reglas generales del proyecto |
| `0_mapa_versiones.md` | Evolución del sistema         |
| `2_spec.md`           | Requisitos funcionales        |
| `3_plan.md`           | Plan técnico                  |
| `4_research.md`       | Investigación y decisiones    |
| `5_data_model.md`     | Modelo de datos               |
| `6_contracts.md`      | Contratos de la API           |
| `7_quickstart.md`     | Guía de ejecución             |
| `8_tasks.md`          | Tareas de la versión          |
| `9_checklist.md`      | Verificación final            |

---

# 🔐 Seguridad

No almacenar en Git:

```text
.env
Contraseñas
Cadenas de conexión con credenciales reales
Secretos
```

Antes de realizar un commit:

```bash
git status
```

Revisar cuidadosamente los archivos que serán enviados.

---

# 🌿 Git

El proyecto utiliza ramas para separar el trabajo de los integrantes.

Flujo recomendado:

```text
Rama personal
      ↓
Desarrollo
      ↓
Pruebas
      ↓
Commit
      ↓
Push
      ↓
Pull Request
      ↓
main
```

Antes de subir cambios:

```bash
dotnet build api_gestion/ApiGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
git status
```

---

# 📌 Estado de la versión

Actualmente se encuentran implementados y probados:

```text
Programa
├── Backend       ✔
├── API CRUD      ✔
├── Frontend      ✔
└── Borrado lógico ✔

Área de Conocimiento
├── Backend       ✔
├── API CRUD      ✔
├── Frontend      ✔
└── Borrado lógico ✔
```

La integración de los demás recursos desarrollados por el equipo debe
verificarse antes de considerar cerrada completamente la versión.

---

# 👥 Proyecto académico

**Proyecto:** Gestión Profesoral

**Tecnologías principales:**

```text
.NET 8
ASP.NET Core
Blazor
SQL Server
Dapper
Docker
Git / GitHub
```

Para información técnica detallada consultar:

```text
docs/spec_kit/
```
