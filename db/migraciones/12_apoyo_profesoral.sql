USE gestion_local;
GO

IF OBJECT_ID('dbo.apoyo_profesoral', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.apoyo_profesoral
    (
        estudios INT NOT NULL,
        con_apoyo TINYINT NOT NULL,
        institucion VARCHAR(45) NOT NULL,
        tipo VARCHAR(45) NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_apoyo_profesoral_activo DEFAULT (1),
        CONSTRAINT PK_apoyo_profesoral PRIMARY KEY (estudios),
        CONSTRAINT FK_apoyo_profesoral_estudios
            FOREIGN KEY (estudios) REFERENCES dbo.estudios_realizados(id),
        CONSTRAINT CK_apoyo_profesoral_con_apoyo CHECK (con_apoyo IN (0, 1))
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_apoyo_profesoral
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudios AS Estudios, con_apoyo AS ConApoyo, institucion AS Institucion,
           tipo AS Tipo, activo AS Activo
    FROM dbo.apoyo_profesoral
    WHERE activo = 1
    ORDER BY estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_apoyo_profesoral
    @Estudios INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudios AS Estudios, con_apoyo AS ConApoyo, institucion AS Institucion,
           tipo AS Tipo, activo AS Activo
    FROM dbo.apoyo_profesoral
    WHERE estudios = @Estudios AND activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_apoyo_profesoral
    @Estudios INT,
    @ConApoyo TINYINT,
    @Institucion VARCHAR(45),
    @Tipo VARCHAR(45)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ConApoyo NOT IN (0, 1)
        THROW 51012, 'El indicador de apoyo debe ser 0 o 1.', 1;

    IF EXISTS (SELECT 1 FROM dbo.apoyo_profesoral WHERE estudios = @Estudios AND activo = 1)
        THROW 51013, 'El estudio ya tiene un apoyo profesoral registrado.', 1;

    IF EXISTS (SELECT 1 FROM dbo.apoyo_profesoral WHERE estudios = @Estudios AND activo = 0)
        UPDATE dbo.apoyo_profesoral
        SET con_apoyo = @ConApoyo, institucion = @Institucion, tipo = @Tipo, activo = 1
        WHERE estudios = @Estudios;
    ELSE
        INSERT INTO dbo.apoyo_profesoral (estudios, con_apoyo, institucion, tipo, activo)
        VALUES (@Estudios, @ConApoyo, @Institucion, @Tipo, 1);

    EXEC dbo.sp_consultar_apoyo_profesoral @Estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_apoyo_profesoral
    @Estudios INT,
    @ConApoyo TINYINT,
    @Institucion VARCHAR(45),
    @Tipo VARCHAR(45)
AS
BEGIN
    SET NOCOUNT ON;
    IF @ConApoyo NOT IN (0, 1)
        THROW 51012, 'El indicador de apoyo debe ser 0 o 1.', 1;

    UPDATE dbo.apoyo_profesoral
    SET con_apoyo = @ConApoyo, institucion = @Institucion, tipo = @Tipo
    WHERE estudios = @Estudios AND activo = 1;

    IF @@ROWCOUNT = 0
        RETURN;

    EXEC dbo.sp_consultar_apoyo_profesoral @Estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_apoyo_profesoral
    @Estudios INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.apoyo_profesoral SET activo = 0
    WHERE estudios = @Estudios AND activo = 1;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
