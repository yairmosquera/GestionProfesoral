# Plan de pruebas de seguridad y regresión — V3

## 1. Alcance
Comprobar autenticación, autorización, navegación de Blazor, administración de cuentas, efecto de cambio de roles y funcionamiento de módulos V1/V2 protegidos por V3.

## 2. Preparación
- Ambiente de pruebas separado de producción.
- Usuarios: ADMIN, GESTOR y CONSULTA; guardar credenciales de forma segura.
- Token vigente por usuario y una solicitud sin token.
- Datos de prueba reversibles, sin alterar registros reales.
- API y frontend compilados en la versión local definitiva.

## 3. Matriz mínima
| ID | Escenario | Resultado esperado |
|---|---|---|
| ST-01 | POST `/api/auth/login` válido | 200 y resultado de sesión válido |
| ST-02 | POST login con contraseña incorrecta | 401 |
| ST-03 | GET `/api/auth/me` sin token | 401 |
| ST-04 | GET `/api/auth/me` autenticado | 200, sin hashes |
| ST-05 | GET `/api/administracion-usuarios` con ADMIN | 200 |
| ST-06 | GET administración con GESTOR | 403 |
| ST-07 | GET administración con CONSULTA | 403 |
| ST-08 | POST `/api/administracion-usuarios/registrar` con CONSULTA | 403 |
| ST-09 | POST registro con ADMIN y datos válidos | 201, rol CONSULTA inicial |
| ST-10 | GET académico con CONSULTA | 200 |
| ST-11 | POST/PUT/DELETE académico con CONSULTA | 403 |
| ST-12 | POST/PUT/DELETE académico con GESTOR | No 403 por rol; resultado depende de datos válidos |
| ST-13 | GET académico sin token | 401 |
| ST-14 | Navegar directo a `/docentes-maestro` sin sesión | Redirige a `/login` |
| ST-15 | Login y navegación a Docentes Maestro | Mantiene sesión, sin bucle |
| ST-16 | Botón Cerrar sesión | Oculta contenido y navega a `/login` |
| ST-17 | Intentar retirar último ADMIN activo | Rechazado por reglas de negocio |
| ST-18 | Intentar desactivar último ADMIN activo | Rechazado por reglas de negocio |
| ST-19 | Cambiar rol/desactivar usuario y usar token anterior | Comportamiento consistente con verificación de rol/estado vigente |
| ST-20 | Refrescar/reconectar Blazor con sesión | Comportamiento identificado y documentado; sin exposición privada |

## 4. Regresión V1/V2
Probar como mínimo: Programas, Áreas, Redes, Líneas, Términos Clave, Docentes Maestro, Estudios, Evaluaciones, Experiencias, Reconocimientos, Becas, Apoyos, Redes-Docentes, Docentes-Departamentos e Intereses Futuros.

## 5. Evidencia
Por caso registrar: fecha, versión/commit, entorno, rol, endpoint o pantalla, resultado HTTP o captura, esperado, real y observaciones. No incluir tokens ni contraseñas en evidencias.

## 6. Criterio de salida
Todos los casos críticos ST-01 a ST-16 superados; ST-17 a ST-20 documentados; builds correctos y sin regresiones graves. No aprobar despliegue si hay rutas privadas accesibles sin autorización.
