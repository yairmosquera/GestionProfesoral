# Especificación funcional — Gestión Profesoral V3

**Identificador:** `003-gestion-profesoral-v3`  
**Estado:** Implementación desarrollada; validación integral pendiente  
**Alcance:** Seguridad y acceso. Las funciones académicas de V1/V2 siguen siendo su responsabilidad; V3 las protege, no las redefine.

## 1. Objetivo
Incorporar autenticación mediante JWT, administración segura de usuarios, permisos basados en roles y navegación protegida en el sistema existente, manteniendo los contratos y funcionalidades académicas de V1 y V2.

## 2. Actores y permisos
| Actor | Consultas de módulos académicos | Crear/editar/eliminar datos académicos | Gestionar usuarios y roles |
|---|---|---|---|
| ADMIN | Sí | Sí | Sí |
| GESTOR | Sí | Sí | No |
| CONSULTA | Sí | No | No |
| Anónimo | No | No | No |

El endpoint de login es público. `GET /api/auth/me` requiere autenticación, pero no un rol específico.

## 3. Historias de usuario
- **US-01 — Inicio de sesión:** Como usuario activo, quiero autenticarme con nombre de usuario y contraseña para acceder según mis permisos.
- **US-02 — Acceso no autenticado:** Como visitante sin sesión, debo ser dirigido a `/login` al abrir una ruta protegida del frontend.
- **US-03 — Consulta:** Como usuario CONSULTA, puedo visualizar módulos académicos sin poder modificar datos.
- **US-04 — Gestión académica:** Como ADMIN o GESTOR, puedo crear, actualizar y retirar registros mediante acciones autorizadas.
- **US-05 — Administración:** Como ADMIN, puedo listar usuarios/roles, registrar cuentas, asignar/retirar roles y activar/desactivar usuarios.
- **US-06 — Cierre de sesión:** Como usuario autenticado, puedo cerrar la sesión del frontend y regresar al login.
- **US-07 — Perfil:** Como usuario autenticado, puedo consultar mis datos y roles activos sin recibir hashes de contraseña.

## 4. Requisitos funcionales
- **RF-01:** `POST /api/auth/login` acepta credenciales, rechaza inválidas con HTTP 401 y devuelve el resultado de autenticación definido por la API.
- **RF-02:** Los endpoints académicos exigen token válido con rol ADMIN, GESTOR o CONSULTA; las escrituras solo admiten ADMIN o GESTOR.
- **RF-03:** La administración de usuarios y el registro de nuevas cuentas exigen ADMIN.
- **RF-04:** El registro crea inicialmente un usuario con rol CONSULTA.
- **RF-05:** La API valida el rol y estado vigente del usuario según la lógica JWT del proyecto; comprobar su comportamiento con pruebas.
- **RF-06:** El frontend adjunta `Authorization: Bearer <token>` a las peticiones privadas.
- **RF-07:** El frontend bloquea las acciones de edición para usuarios CONSULTA, sin depender de ello como único control de seguridad.
- **RF-08:** Sin sesión, navegar a rutas académicas dirige a `/login` y no revela el contenido protegido.
- **RF-09:** La vista de login no provoca un bucle de redirecciones.
- **RF-10:** Cerrar sesión elimina el estado de sesión de Blazor y redirige a `/login`.
- **RF-11:** Administración de usuarios no expone contraseñas ni hashes en las respuestas.
- **RF-12:** No permitir desactivar ni quitar el último administrador activo conforme a las validaciones de base de datos existentes.

## 5. Requisitos no funcionales
- **RNF-01:** Mantener ASP.NET Core 8, Blazor Server y SQL Server existentes.
- **RNF-02:** No guardar secretos JWT ni contraseñas en Git.
- **RNF-03:** Usar HTTPS en despliegue público.
- **RNF-04:** No modificar arbitrariamente DTO, rutas ni procedimientos de V1/V2.
- **RNF-05:** La versión debe compilar en modo Release antes de publicar.
- **RNF-06:** Manejar claramente respuestas HTTP 401 (no autenticado) y 403 (sin permiso).

## 6. Criterios de aceptación
1. ADMIN accede a administración y a operaciones académicas.
2. GESTOR modifica datos académicos, pero recibe 403 en administración.
3. CONSULTA realiza GET académicos, pero recibe 403 en POST/PUT/PATCH/DELETE restringidos.
4. Una petición privada sin JWT recibe 401.
5. El login funciona sin JWT previo.
6. Logout regresa a login y el frontend deja de mostrar vistas privadas.
7. No hay regresiones detectadas en Docentes Maestro y módulos académicos.
8. El repositorio no incluye credenciales ni archivos de compilación.

## 7. Fuera de alcance
Crear nuevos módulos académicos, rediseñar la arquitectura, migrar la base de datos, implementar renovación automática de tokens o declarar revocación criptográfica del JWT sin pruebas. La publicación de producción es una fase posterior.

## 8. Evidencia y pendientes
Se han compartido/controlado archivos de servicios y controladores; en la conversación se confirmó implementación de varios pasos. **Falta ejecutar** pruebas de integración reales, validar persistencia de sesión en navegación, compilar la versión local final y revisar los cambios no publicados del equipo.
