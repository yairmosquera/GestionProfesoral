
USE gestion_local;
GO

-- V3: creación única del primer administrador.
-- No crea cuentas automáticamente ni contraseñas predeterminadas.

CREATE OR ALTER PROCEDURE dbo.sp_crear_administrador_inicial
    @NombreUsuario NVARCHAR(80),
    @Correo NVARCHAR(150),
    @NombreCompleto NVARCHAR(160),
    @PasswordHash NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@NombreUsuario)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@Correo)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@NombreCompleto)), N'') IS NULL
       OR @PasswordHash IS NULL OR LEN(@PasswordHash) < 40
        THROW 51010,
              N'Datos del administrador inicial inválidos.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Evitar administradores duplicados
        DECLARE @LockResult INT;

        EXEC @LockResult = sys.sp_getapplock
            @Resource = 'gestion_profesoral_bootstrap_admin_v3',
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = 10000;

        IF @LockResult < 0
            THROW 51011,
                  N'No fue posible obtener el bloqueo de inicialización.', 1;

        -- Solo permitir la creación cuando no existan usuarios
        IF EXISTS (
            SELECT 1
            FROM dbo.usuarios WITH (UPDLOCK, HOLDLOCK)
        )
            THROW 51012,
                  N'Ya existen usuarios. No se permite repetir la inicialización.', 1;

        -- Obtener el rol de administrador
        DECLARE @RolAdminId INT;

        SELECT @RolAdminId = id
        FROM dbo.roles WITH (UPDLOCK, HOLDLOCK)
        WHERE codigo = 'ADMIN'
          AND activo = 1;

        IF @RolAdminId IS NULL
            THROW 51013,
                  N'No existe el rol ADMIN activo.', 1;

        -- Registrar usuario
        INSERT INTO dbo.usuarios
        (
            nombre_usuario,
            correo,
            nombre_completo,
            password_hash
        )
        VALUES
        (
            @NombreUsuario,
            @Correo,
            @NombreCompleto,
            @PasswordHash
        );

        DECLARE @UsuarioId INT =
            CONVERT(INT, SCOPE_IDENTITY());

        -- Asignar rol ADMIN
        INSERT INTO dbo.usuario_roles
        (
            usuario_id,
            rol_id
        )
        VALUES
        (
            @UsuarioId,
            @RolAdminId
        );

        COMMIT TRANSACTION;

        SELECT
            @UsuarioId AS Id,
            @NombreUsuario AS NombreUsuario;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

-- Verificar instalación del procedimiento
SELECT name
FROM sys.procedures
WHERE name = 'sp_crear_administrador_inicial';
GO
