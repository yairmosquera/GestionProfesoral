# Investigación y decisiones técnicas — V3

## DEC-01 — JWT para la API
**Decisión:** mantener autenticación JWT Bearer y autorización por roles en controladores ASP.NET Core.  
**Motivo:** seguridad validada en servidor y compatibilidad con servicios HttpClient existentes.  
**Alternativa descartada:** confiar únicamente en botones ocultos del frontend.

## DEC-02 — Roles
**Decisión:** ADMIN y GESTOR pueden modificar módulos académicos; CONSULTA únicamente GET; administración de cuentas solo ADMIN.  
**Evidencia:** atributos `[Authorize(Roles = "ADMIN")]` en controladores de administración/registro proporcionados por el proyecto y patrón de autorización usado en los módulos académicos.

## DEC-03 — Registro de usuarios
**Decisión:** el endpoint de registro requiere ADMIN; no hay registro público; nueva cuenta inicia en CONSULTA.  
**Detalle:** la creación y los roles iniciales dependen de repositorios/procedimientos de SQL existentes.

## DEC-04 — Estado de sesión en Blazor Server
**Decisión:** conservar `ServicioSesionJwt` en alcance scoped; navegación y layout deben operar dentro de un mismo circuito interactivo.  
**Por comprobar:** comportamiento tras refresh, apertura de pestañas y reconexión; no prometer persistencia del JWT que no haya sido implementada.

## DEC-05 — Cierre de sesión
**Decisión:** borrar el estado JWT del frontend y redirigir a `/login`.  
**Límite:** esto no demuestra revocación del JWT emitido en el servidor. Requiere expiración corta y, si se necesita, mecanismo explícito de revocación.

## DEC-06 — Gestión de errores
**Decisión:** 401 para ausencia/invalidez de autenticación; 403 para usuario autenticado sin rol autorizado. Validaciones de negocio mantienen mensajes y códigos propios.

## DEC-07 — Despliegue
**Decisión:** preparar API y Blazor como publicaciones distintas, alojadas sobre HTTPS y SQL Server.  
**Pendiente:** confirmar configuración concreta, soporte de hosting y variables disponibles en MonsterASP.NET cuando llegue la fase de despliegue.

## Riesgos de seguridad por verificar
- Política de contraseñas y límites de intentos.
- Tratamiento de refresh/reconexión en Blazor.
- Almacenamiento real del token en memoria y potencial exposición.
- Validez de tokens al cambiar roles o desactivar usuario.
- Manejo de errores sin filtrar datos sensibles.

## Fuentes de verdad
Código local actual, configuraciones reales de API y Blazor, resultados de compilación y pruebas. Una revisión estática no prueba por sí sola la seguridad de runtime.
