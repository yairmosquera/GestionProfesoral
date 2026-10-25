USE gestion_local;
GO

IF OBJECT_ID('dbo.beca', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.beca
    (
        estudios INT NOT NULL,
        tipo VARCHAR(45) NOT NULL,
        institucion VARCHAR(80) NOT NULL,
        fecha_inicio DATE NOT NULL,
        fecha_fin DATE NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_beca_activo DEFAULT (1),
        CONSTRAINT PK_beca PRIMARY KEY (estudios),
        CONSTRAINT FK_beca_estudios
            FOREIGN KEY (estudios) REFERENCES dbo.estudios_realizados(id),
        CONSTRAINT CK_beca_fechas
            CHECK (fecha_fin IS NULL OR fecha_fin >= fecha_inicio)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_beca
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudios AS Estudios, tipo AS Tipo, institucion AS Institucion,
           fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, activo AS Activo
    FROM dbo.beca
    WHERE activo = 1
    ORDER BY estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_beca
    @Estudios INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudios AS Estudios, tipo AS Tipo, institucion AS Institucion,
           fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, activo AS Activo
    FROM dbo.beca
    WHERE estudios = @Estudios AND activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_beca
    @Estudios INT,
    @Tipo VARCHAR(45),
    @Institucion VARCHAR(80),
    @FechaInicio DATE,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @FechaFin IS NOT NULL AND @FechaFin < @FechaInicio
        THROW 51014, 'La fecha de fin no puede ser anterior a la fecha de inicio.', 1;

    IF EXISTS (SELECT 1 FROM dbo.beca WHERE estudios = @Estudios AND activo = 1)
        THROW 51015, 'El estudio ya tiene una beca registrada.', 1;

    IF EXISTS (SELECT 1 FROM dbo.beca WHERE estudios = @Estudios AND activo = 0)
        UPDATE dbo.beca
        SET tipo = @Tipo, institucion = @Institucion, fecha_inicio = @FechaInicio,
            fecha_fin = @FechaFin, activo = 1
        WHERE estudios = @Estudios;
    ELSE
        INSERT INTO dbo.beca (estudios, tipo, institucion, fecha_inicio, fecha_fin, activo)
        VALUES (@Estudios, @Tipo, @Institucion, @FechaInicio, @FechaFin, 1);

    EXEC dbo.sp_consultar_beca @Estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_beca
    @Estudios INT,
    @Tipo VARCHAR(45),
    @Institucion VARCHAR(80),
    @FechaInicio DATE,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaFin IS NOT NULL AND @FechaFin < @FechaInicio
        THROW 51014, 'La fecha de fin no puede ser anterior a la fecha de inicio.', 1;

    UPDATE dbo.beca
    SET tipo = @Tipo, institucion = @Institucion, fecha_inicio = @FechaInicio,
        fecha_fin = @FechaFin
    WHERE estudios = @Estudios AND activo = 1;

    IF @@ROWCOUNT = 0
        RETURN;

    EXEC dbo.sp_consultar_beca @Estudios;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_beca
    @Estudios INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.beca SET activo = 0
    WHERE estudios = @Estudios AND activo = 1;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
