USE gestion_local;
GO

/*
  Gestión Profesoral - Versión 3
  Crear usuario nuevo con rol CONSULTA inicial.
  Ejecutar una sola vez o repetir sin pérdida de datos.
  Reemplaza únicamente dbo.sp_usuario_crear.

  El hash lo genera ServicioPassword en ASP.NET Core.
  Este procedimiento NO verifica JWT: llamarlo solo desde
  un controlador [Authorize(Roles = "ADMIN")].
*/
CREATE OR ALTER PROCEDURE dbo.sp_usuario_crear
    @NombreUsuario NVARCHAR(80),
    @Correo NVARCHAR(150),
    @NombreCompleto NVARCHAR(160),
    @PasswordHash NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @NombreUsuario = NULLIF(LTRIM(RTRIM(@NombreUsuario)), N'');
    SET @Correo = NULLIF(LTRIM(RTRIM(@Correo)), N'');
    SET @NombreCompleto = NULLIF(LTRIM(RTRIM(@NombreCompleto)), N'');

    IF @NombreUsuario IS NULL OR @Correo IS NULL OR @NombreCompleto IS NULL
        THROW 51031, N'Nombre de usuario, correo y nombre completo son obligatorios.', 1;

    IF @PasswordHash IS NULL OR LEN(@PasswordHash) < 40
        THROW 51032, N'El hash de contraseña no es válido.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.usuarios WITH (UPDLOCK, HOLDLOCK)
                   WHERE nombre_usuario = @NombreUsuario)
            THROW 51033, N'El nombre de usuario ya está registrado.', 1;

        IF EXISTS (SELECT 1 FROM dbo.usuarios WITH (UPDLOCK, HOLDLOCK)
                   WHERE correo = @Correo)
            THROW 51034, N'El correo ya está registrado.', 1;

        DECLARE @RolConsultaId INT;
        SELECT @RolConsultaId = id
        FROM dbo.roles
        WHERE codigo = 'CONSULTA' AND activo = 1;

        IF @RolConsultaId IS NULL
            THROW 51035, N'No existe un rol CONSULTA activo.', 1;

        INSERT INTO dbo.usuarios
            (nombre_usuario, correo, nombre_completo, password_hash)
        VALUES
            (@NombreUsuario, @Correo, @NombreCompleto, @PasswordHash);

        DECLARE @UsuarioId INT = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.usuario_roles (usuario_id, rol_id)
        VALUES (@UsuarioId, @RolConsultaId);

        COMMIT TRANSACTION;

        -- El objeto que espera RepositorioUsuario.CrearAsync.
        -- Nunca se devuelve password_hash.
        SELECT id AS Id,
               nombre_usuario AS NombreUsuario,
               correo AS Correo,
               nombre_completo AS NombreCompleto,
               activo AS Activo,
               fecha_creacion AS FechaCreacion,
               ultimo_acceso AS UltimoAcceso
        FROM dbo.usuarios WHERE id = @UsuarioId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

SELECT SCHEMA_NAME(schema_id) AS Esquema, name AS Procedimiento
FROM sys.procedures
WHERE name = N'sp_usuario_crear';
GO