# Inicio rápido y verificación — Gestión Profesoral V3

## Requisitos
- SDK .NET 8.
- SQL Server accesible y esquema/procedimientos de Gestión Profesoral desplegados.
- Configuraciones de la API y del frontend, con secretos fuera de control de versiones.
- Usuario administrador ya creado para administrar cuentas.

## 1. Revisar repositorio
Desde la raíz `GestionProfesoral`:
```powershell
git status
git branch --show-current
dotnet --info
```

## 2. Compilar
```powershell
dotnet build .\api_gestion\ApiGestion.csproj
dotnet build .\front_blazor\FrontGestion.csproj
```

## 3. Ejecutar
En terminales independientes:
```powershell
dotnet run --project .\api_gestion\ApiGestion.csproj
```
```powershell
dotnet run --project .\front_blazor\FrontGestion.csproj
```

La configuración local observada del frontend usa `http://localhost:5100`; la API usa la dirección configurada en `ApiUrl` (en una versión anterior se utilizó por defecto `http://localhost:5000`). Confirmar puertos en la salida real de ejecución.

## 4. Probar autenticación
1. Abrir `/login` e iniciar sesión como ADMIN.
2. Confirmar navegación a una página académica.
3. Comprobar que ADMIN puede entrar a administración de usuarios.
4. Cerrar sesión y confirmar redirección a login.
5. Acceder directamente a `/docentes-maestro` sin sesión y confirmar que no se muestra contenido privado.

## 5. Crear usuarios de prueba
**POST** `/api/administracion-usuarios/registrar` con JWT ADMIN:
```json
{
  "nombreUsuario": "gestor01",
  "correo": "gestor01@ejemplo.com",
  "nombreCompleto": "Usuario Gestor",
  "contrasena": "CAMBIAR_ESTA_CLAVE_DE_PRUEBA_2026"
}
```
El registro crea CONSULTA. Después asignar GESTOR con `POST /api/administracion-usuarios/{usuarioId}/roles`:
```json
{"rolCodigo":"GESTOR"}
```
Para una cuenta CONSULTA crear otra mediante registro y mantener el rol inicial. **No usar esas contraseñas de ejemplo en producción**.

## 6. Validar HTTP
- GET privado sin token → 401.
- Escritura académica CONSULTA → 403.
- Administración GESTOR o CONSULTA → 403.
- GET académico autenticado con rol permitido → 200.
- Login sin token → permitido; credenciales inválidas → 401.

## 7. Antes de desplegar
Verificar HTTPS, configuraciones JWT, SQL Server, `ApiUrl`, permisos del hosting, SignalR/Blazor Server, compilación Release y secretos en variables de entorno.
