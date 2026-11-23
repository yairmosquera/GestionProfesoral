USE gestion_local;
GO

/*
  GESTIÓN PROFESORAL - VERSIÓN 3
  Administración de usuarios y roles.

  Requisitos previos:
    dbo.usuarios, dbo.roles y dbo.usuario_roles (migración 14).
    El administrador inicial ya se creó.

  Seguridad:
    Estos procedimientos deben invocarse exclusivamente desde endpoints
    de la API protegidos con [Authorize(Roles = "ADMIN")].
    SQL no valida el JWT: la autorización ocurre en la API.
    No se exponen hashes de contraseñas.
*/

-- 1. Listar usuarios SIN mostrar password_hash
CREATE OR ALTER PROCEDURE dbo.sp_usuarios_listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.id AS Id,
        u.nombre_usuario AS NombreUsuario,
        u.correo AS Correo,
        u.nombre_completo AS NombreCompleto,
        u.activo AS Activo,
        u.fecha_creacion AS FechaCreacion,
        u.ultimo_acceso AS UltimoAcceso
    FROM dbo.usuarios AS u
    ORDER BY u.nombre_usuario;
END;
GO

-- 2. Listar los roles disponibles
CREATE OR ALTER PROCEDURE dbo.sp_roles_listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        codigo AS Codigo,
        nombre AS Nombre,
        descripcion AS Descripcion,
        activo AS Activo
    FROM dbo.roles
    ORDER BY nombre;
END;
GO

-- 3. Asignar rol existente a un usuario activo
CREATE OR ALTER PROCEDURE dbo.sp_usuario_asignar_rol
    @UsuarioId INT,
    @RolCodigo VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @RolId INT;

        IF NOT EXISTS (
            SELECT 1 FROM dbo.usuarios WITH (UPDLOCK, HOLDLOCK)
            WHERE id = @UsuarioId AND activo = 1
        )
            THROW 51020, N'El usuario no existe o está inactivo.', 1;

        SELECT @RolId = id
        FROM dbo.roles WITH (UPDLOCK, HOLDLOCK)
        WHERE codigo = @RolCodigo AND activo = 1;

        IF @RolId IS NULL
            THROW 51021, N'El rol no existe o está inactivo.', 1;

        IF EXISTS (
            SELECT 1 FROM dbo.usuario_roles
            WHERE usuario_id = @UsuarioId AND rol_id = @RolId
        )
            THROW 51022, N'El usuario ya tiene asignado este rol.', 1;

        INSERT INTO dbo.usuario_roles (usuario_id, rol_id)
        VALUES (@UsuarioId, @RolId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 4. Retirar rol; impedir retirar el último ADMIN activo
CREATE OR ALTER PROCEDURE dbo.sp_usuario_retirar_rol
    @UsuarioId INT,
    @RolCodigo VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @RolId INT;
        SELECT @RolId = id
        FROM dbo.roles
        WHERE codigo = @RolCodigo;

        IF @RolId IS NULL OR NOT EXISTS (
            SELECT 1 FROM dbo.usuario_roles
            WHERE usuario_id = @UsuarioId AND rol_id = @RolId
        )
            THROW 51023, N'El usuario no tiene asignado ese rol.', 1;

        -- Serializa cambios administrativos para evitar quitar simultáneamente
        -- el último administrador.
        DECLARE @LockResult INT;
        EXEC @LockResult = sys.sp_getapplock
            @Resource = 'gestion_profesoral_admin_roles',
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = 10000;
        IF @LockResult < 0
            THROW 51024, N'No se pudo completar el cambio de rol.', 1;

        IF @RolCodigo = 'ADMIN' AND NOT EXISTS (
            SELECT 1
            FROM dbo.usuario_roles AS ur
            JOIN dbo.usuarios AS u ON u.id = ur.usuario_id
            JOIN dbo.roles AS r ON r.id = ur.rol_id
            WHERE r.codigo = 'ADMIN'
              AND r.activo = 1 AND u.activo = 1
              AND u.id <> @UsuarioId
        )
            THROW 51025, N'No se puede retirar el último administrador activo.', 1;

        DELETE FROM dbo.usuario_roles
        WHERE usuario_id = @UsuarioId AND rol_id = @RolId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- 5. Activar/desactivar usuario; proteger al último ADMIN activo
CREATE OR ALTER PROCEDURE dbo.sp_usuario_cambiar_estado
    @UsuarioId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @LockResult INT;
        EXEC @LockResult = sys.sp_getapplock
            @Resource = 'gestion_profesoral_admin_roles',
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = 10000;
        IF @LockResult < 0
            THROW 51024, N'No se pudo completar el cambio de estado.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.usuarios WHERE id = @UsuarioId)
            THROW 51026, N'El usuario no existe.', 1;

        IF @Activo = 0 AND EXISTS (
            SELECT 1
            FROM dbo.usuario_roles AS ur
            JOIN dbo.roles AS r ON r.id = ur.rol_id
            JOIN dbo.usuarios AS u ON u.id = ur.usuario_id
            WHERE ur.usuario_id = @UsuarioId
              AND u.activo = 1 AND r.codigo = 'ADMIN' AND r.activo = 1
        ) AND NOT EXISTS (
            SELECT 1
            FROM dbo.usuario_roles AS ur
            JOIN dbo.roles AS r ON r.id = ur.rol_id
            JOIN dbo.usuarios AS u ON u.id = ur.usuario_id
            WHERE ur.usuario_id <> @UsuarioId
              AND u.activo = 1 AND r.codigo = 'ADMIN' AND r.activo = 1
        )
            THROW 51027, N'No se puede desactivar el último administrador activo.', 1;

        UPDATE dbo.usuarios SET activo = @Activo
        WHERE id = @UsuarioId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- Verificación: esta consulta no cambia cuentas ni roles.
SELECT SCHEMA_NAME(schema_id) AS Esquema, name AS Procedimiento
FROM sys.procedures
WHERE name IN (
    'sp_usuarios_listar',
    'sp_roles_listar',
    'sp_usuario_asignar_rol',
    'sp_usuario_retirar_rol',
    'sp_usuario_cambiar_estado'
)
ORDER BY name;
GO