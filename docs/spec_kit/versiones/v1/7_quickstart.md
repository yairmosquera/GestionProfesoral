# Quickstart — Versión 1: Catálogos iniciales

> Guía rápida para configurar y ejecutar la versión 1 del sistema
> Gestión Profesoral en un ambiente local.

---

# 1. Requisitos previos

Antes de ejecutar el proyecto se requiere:

- Git.
- Docker Desktop.
- .NET 8 SDK.
- Visual Studio Code o Visual Studio.
- Un navegador web.

Para verificar .NET:

```bash
dotnet --version
```

Para verificar Docker:

```bash
docker --version
docker compose version
```

Para verificar Git:

```bash
git --version
```

---

# 2. Obtener el proyecto

Clonar el repositorio o abrir una copia existente.

Después, ubicarse en la raíz:

```powershell
cd GestionProfesoral
```

La estructura principal debe incluir:

```text
GestionProfesoral/
├── api_gestion/
├── front_blazor/
├── db/
├── docs/
├── docker-compose.yml
├── .env.example
└── .gitignore
```

---

# 3. Configurar variables de entorno

El proyecto utiliza variables de entorno para información sensible.

Si existe:

```text
.env.example
```

crear una copia llamada:

```text
.env
```

En PowerShell:

```powershell
Copy-Item .env.example .env
```

Completar las variables requeridas para el entorno local.

---

# 4. Regla de seguridad

El archivo:

```text
.env
```

contiene información que puede ser sensible.

Nunca debe agregarse al repositorio.

Antes de realizar un commit comprobar:

```bash
git status
```

`.env` no debe aparecer entre los archivos preparados para commit.

No se deben escribir contraseñas reales dentro de este documento.

---

# 5. Levantar SQL Server

Desde la raíz del proyecto ejecutar:

```bash
docker compose up -d
```

Después verificar:

```bash
docker compose ps
```

El contenedor de SQL Server debe aparecer en ejecución.

---

# 6. Verificar logs de Docker

Si SQL Server no inicia correctamente:

```bash
docker compose logs sqlserver
```

También se pueden seguir los logs:

```bash
docker compose logs -f sqlserver
```

Para salir de la visualización de logs:

```text
Ctrl + C
```

Esto no elimina el contenedor.

---

# 7. Base de datos

La base utilizada por el proyecto es:

```text
gestion_local
```

Los scripts necesarios se encuentran en:

```text
db/
```

La configuración del proyecto prepara la base de datos requerida para el
ambiente local.

Las tablas utilizadas inicialmente por esta versión son:

```text
programa
area_conocimiento
```

---

# 8. Restaurar dependencias de la API

Desde la raíz:

```bash
dotnet restore api_gestion/ApiGestion.csproj
```

Después compilar:

```bash
dotnet build api_gestion/ApiGestion.csproj
```

La compilación debe finalizar sin errores.

---

# 9. Ejecutar la API

Ejecutar:

```bash
dotnet run --project api_gestion/ApiGestion.csproj
```

Durante el desarrollo local la API utiliza:

```text
http://localhost:5000
```

Mantener esta terminal abierta.

---

# 10. Abrir Swagger

Con la API ejecutándose, abrir en el navegador:

```text
http://localhost:5000/swagger/index.html
```

Swagger permite comprobar los endpoints de la API.

---

# 11. Probar Programa desde Swagger

Los endpoints disponibles son:

```text
GET     /api/programas
GET     /api/programas/{id}
POST    /api/programas
PUT     /api/programas/{id}
PATCH   /api/programas/{id}
DELETE  /api/programas/{id}
```

---

# 12. Crear un Programa

Desde Swagger utilizar:

```text
POST /api/programas
```

Ejemplo:

```json
{
  "id": 901,
  "nombre": "Programa de prueba",
  "tipo": "Pregrado",
  "nivel": "Profesional",
  "fechaCreacion": "2026-09-01",
  "fechaCierre": null,
  "numeroCohortes": "1",
  "cantGraduados": "0",
  "fechaActualizacion": "2026-09-27",
  "ciudad": "Medellín",
  "facultad": 1
}
```

Utilizar un ID que no exista previamente.

---

# 13. Consultar Programa

Después de crearlo:

```text
GET /api/programas/901
```

También comprobar:

```text
GET /api/programas
```

El registro debe aparecer mientras se encuentre activo.

---

# 14. Actualizar Programa

Para reemplazo completo:

```text
PUT /api/programas/901
```

Para actualización parcial:

```text
PATCH /api/programas/901
```

Ejemplo de PATCH:

```json
{
  "ciudad": "Medellín"
}
```

---

# 15. Retirar Programa

Ejecutar:

```text
DELETE /api/programas/901
```

Después:

```text
GET /api/programas
```

El registro retirado no debe aparecer en el listado normal.

La fila debe permanecer en SQL Server con:

```text
activo = 0
```

---

# 16. Probar Área de Conocimiento

Los endpoints disponibles son:

```text
GET     /api/areas-conocimiento
GET     /api/areas-conocimiento/{id}
POST    /api/areas-conocimiento
PUT     /api/areas-conocimiento/{id}
PATCH   /api/areas-conocimiento/{id}
DELETE  /api/areas-conocimiento/{id}
```

---

# 17. Crear Área de Conocimiento

Utilizar:

```text
POST /api/areas-conocimiento
```

Ejemplo:

```json
{
  "id": "AT001",
  "granArea": "Área de prueba",
  "area": "Subárea de prueba",
  "disciplina": "Disciplina de prueba"
}
```

El ID utilizado para la prueba debe ser diferente de uno existente.

---

# 18. Consultar Área

Ejecutar:

```text
GET /api/areas-conocimiento/AT001
```

También:

```text
GET /api/areas-conocimiento
```

El área debe aparecer en el listado mientras esté activa.

---

# 19. Actualizar Área

Reemplazo:

```text
PUT /api/areas-conocimiento/AT001
```

Ejemplo:

```json
{
  "granArea": "Área actualizada",
  "area": "Subárea actualizada",
  "disciplina": "Disciplina actualizada"
}
```

Actualización parcial:

```text
PATCH /api/areas-conocimiento/AT001
```

Ejemplo:

```json
{
  "disciplina": "Nueva disciplina"
}
```

---

# 20. Retirar Área

Ejecutar:

```text
DELETE /api/areas-conocimiento/AT001
```

Después consultar:

```text
GET /api/areas-conocimiento
```

El registro no debe aparecer entre las áreas activas.

La fila debe permanecer almacenada con:

```text
activo = 0
```

---

# 21. Ejecutar el frontend

Mantener la API ejecutándose.

Abrir una segunda terminal en la raíz del proyecto.

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

---

# 22. Abrir el frontend

Durante el desarrollo local el frontend utiliza:

```text
http://localhost:5100
```

Abrir esa dirección en el navegador.

La página inicial debe mostrar:

```text
Sistema de Gestión Profesoral
```

---

# 23. Probar Programa desde Blazor

Ingresar a:

```text
http://localhost:5100/programas
```

Comprobar:

1. La tabla carga.
2. Se puede registrar un Programa.
3. El Programa aparece en la tabla.
4. Se puede seleccionar Editar.
5. Los datos aparecen en el formulario.
6. Se puede actualizar.
7. Se puede retirar.
8. El registro retirado desaparece del listado.

---

# 24. Probar Área de Conocimiento desde Blazor

Ingresar a:

```text
http://localhost:5100/areas-conocimiento
```

Comprobar:

1. La tabla carga.
2. Se puede registrar un Área.
3. El Área aparece en la tabla.
4. El botón Editar carga la información.
5. Se pueden modificar los datos.
6. El botón Retirar funciona.
7. El registro retirado desaparece.

---

# 25. Orden recomendado para iniciar el sistema

Utilizar este orden:

```text
1. Docker Desktop
       ↓
2. SQL Server
       ↓
3. API
       ↓
4. Frontend
       ↓
5. Navegador
```

En comandos:

```bash
docker compose up -d
```

Terminal para API:

```bash
dotnet run --project api_gestion/ApiGestion.csproj
```

Segunda terminal para frontend:

```bash
dotnet run --project front_blazor/FrontGestion.csproj
```

---

# 26. Detener la aplicación

Para detener API o frontend:

```text
Ctrl + C
```

en la terminal correspondiente.

Para detener los contenedores:

```bash
docker compose down
```

---

# 27. No eliminar los datos accidentalmente

Si únicamente se desea detener los contenedores:

```bash
docker compose down
```

No eliminar manualmente el volumen de SQL Server si se desea conservar la
información almacenada.

---

# 28. Problema — Puerto ocupado

Si aparece un mensaje similar a:

```text
address already in use
```

significa que otra aplicación está utilizando el puerto.

Comprobar que:

```text
API       → 5000
Frontend  → 5100
```

No se debe intentar ejecutar ambos proyectos sobre el mismo puerto.

---

# 29. Problema — Frontend abre pero no carga datos

Verificar primero que la API esté funcionando.

Abrir:

```text
http://localhost:5000/swagger/index.html
```

Si Swagger no abre, el problema debe resolverse primero en la API.

También verificar en:

```text
front_blazor/appsettings.json
```

que:

```json
"ApiUrl": "http://localhost:5000"
```

corresponda con la dirección utilizada por la API.

---

# 30. Problema — Los botones de Blazor no responden

Las páginas interactivas deben utilizar:

```razor
@rendermode InteractiveServer
```

La aplicación también debe registrar los componentes interactivos.

Si la página se visualiza pero los botones no ejecutan acciones, revisar esta
configuración.

---

# 31. Problema — Error de conexión con SQL Server

Comprobar:

```bash
docker compose ps
```

Después revisar:

```bash
docker compose logs sqlserver
```

También verificar las variables configuradas en `.env`.

No copiar contraseñas reales en mensajes, documentación o commits.

---

# 32. Verificación antes de realizar commit

Ejecutar:

```bash
git status
```

Revisar especialmente que no se estén agregando:

```text
.env
bin/
obj/
```

Después compilar:

```bash
dotnet build api_gestion/ApiGestion.csproj
dotnet build front_blazor/FrontGestion.csproj
```

Solo después de comprobar los cambios se debe realizar el commit.

---

# 33. Flujo de verificación rápida

Una prueba completa puede seguir este flujo:

```text
docker compose up -d
        ↓
Ejecutar API
        ↓
Abrir Swagger
        ↓
GET /api/programas
        ↓
GET /api/areas-conocimiento
        ↓
Ejecutar frontend
        ↓
Abrir /
        ↓
Abrir /programas
        ↓
Crear / Editar / Retirar
        ↓
Abrir /areas-conocimiento
        ↓
Crear / Editar / Retirar
        ↓
Verificar persistencia
```

---

# 34. Resultado esperado

Al finalizar esta guía deben estar funcionando simultáneamente:

```text
SQL Server
     ↑
     │
API ASP.NET Core
     ↑
     │ HTTP / JSON
     │
Frontend Blazor
     ↑
     │
  Usuario
```

Y deben estar disponibles:

```text
API:
http://localhost:5000

Swagger:
http://localhost:5000/swagger/index.html

Frontend:
http://localhost:5100

Programa:
http://localhost:5100/programas

Área de Conocimiento:
http://localhost:5100/areas-conocimiento
```

Con esto la versión 1 queda disponible para pruebas locales.
