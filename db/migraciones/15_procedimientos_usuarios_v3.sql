USE gestion_local;
GO

-- V3: procedimientos de usuarios para el backend.
-- No crear usuarios de demostración ni contraseñas predeterminadas.

CREATE OR ALTER PROCEDURE dbo.sp_usuario_consultar_por_nombre
    @NombreUsuario NVARCHAR(80)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        nombre_usuario AS NombreUsuario,
        correo AS Correo,
        nombre_completo AS NombreCompleto,
        password_hash AS PasswordHash,
        activo AS Activo,
        fecha_creacion AS FechaCreacion,
        ultimo_acceso AS UltimoAcceso
    FROM dbo.usuarios
    WHERE nombre_usuario = @NombreUsuario;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_usuario_consultar_por_id
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        nombre_usuario AS NombreUsuario,
        correo AS Correo,
        nombre_completo AS NombreCompleto,
        password_hash AS PasswordHash,
        activo AS Activo,
        fecha_creacion AS FechaCreacion,
        ultimo_acceso AS UltimoAcceso
    FROM dbo.usuarios
    WHERE id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_usuario_crear
    @NombreUsuario NVARCHAR(80),
    @Correo NVARCHAR(150),
    @NombreCompleto NVARCHAR(160),
    @PasswordHash NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@NombreUsuario)), N'') IS NULL
        THROW 51001, N'El nombre de usuario es obligatorio.', 1;

    IF NULLIF(LTRIM(RTRIM(@Correo)), N'') IS NULL
        THROW 51002, N'El correo es obligatorio.', 1;

    IF NULLIF(LTRIM(RTRIM(@NombreCompleto)), N'') IS NULL
        THROW 51003, N'El nombre completo es obligatorio.', 1;

    IF @PasswordHash IS NULL OR LEN(@PasswordHash) < 40
        THROW 51004, N'El hash de contraseña no es válido.', 1;

    INSERT dbo.usuarios (nombre_usuario, correo, nombre_completo, password_hash)
    VALUES (@NombreUsuario, @Correo, @NombreCompleto, @PasswordHash);

    DECLARE @Id INT = CONVERT(INT, SCOPE_IDENTITY());
    EXEC dbo.sp_usuario_consultar_por_id @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_usuario_registrar_acceso
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.usuarios
    SET ultimo_acceso = SYSUTCDATETIME()
    WHERE id = @Id AND activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_usuario_roles_activos
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.codigo
    FROM dbo.usuario_roles ur
    INNER JOIN dbo.roles r ON r.id = ur.rol_id
    INNER JOIN dbo.usuarios u ON u.id = ur.usuario_id
    WHERE ur.usuario_id = @Id AND r.activo = 1 AND u.activo = 1;
END;
GO

-- Verificación: no mostrar hashes de contraseñas.
SELECT p.name FROM sys.procedures p
WHERE p.name IN (
    'sp_usuario_consultar_por_nombre',
    'sp_usuario_consultar_por_id',
    'sp_usuario_crear',
    'sp_usuario_registrar_acceso',
    'sp_usuario_roles_activos'
) ORDER BY p.name;
GO
