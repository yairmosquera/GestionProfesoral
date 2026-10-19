
USE gestion_local;
GO

-- 1. CREAR TABLA SI NO EXISTE

IF OBJECT_ID('dbo.experiecia', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.experiecia
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        nombre_cargo VARCHAR(45) NOT NULL,
        institucion VARCHAR(45) NOT NULL,
        tipo VARCHAR(45) NOT NULL,
        fecha_inicio DATE NOT NULL,
        fecha_fin DATE NULL,
        docente INT NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_experiecia_activo DEFAULT (1),

        CONSTRAINT FK_experiecia_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula)
    );
END;
GO

-- 2. LISTAR EXPERIENCIAS ACTIVAS

CREATE OR ALTER PROCEDURE dbo.sp_listar_experiencia_docente
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        nombre_cargo AS NombreCargo,
        institucion AS Institucion,
        tipo AS Tipo,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        docente AS Docente,
        activo AS Activo
    FROM dbo.experiecia
    WHERE activo = 1
    ORDER BY id DESC;
END;
GO

-- 3. CONSULTAR POR ID

CREATE OR ALTER PROCEDURE dbo.sp_consultar_experiencia_docente
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        nombre_cargo AS NombreCargo,
        institucion AS Institucion,
        tipo AS Tipo,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        docente AS Docente,
        activo AS Activo
    FROM dbo.experiecia
    WHERE id = @Id
      AND activo = 1;
END;
GO

-- 4. CONSULTAR EXPERIENCIAS DE UN DOCENTE

CREATE OR ALTER PROCEDURE dbo.sp_experiencias_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        nombre_cargo AS NombreCargo,
        institucion AS Institucion,
        tipo AS Tipo,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        docente AS Docente,
        activo AS Activo
    FROM dbo.experiecia
    WHERE docente = @Cedula
      AND activo = 1
    ORDER BY fecha_inicio DESC, id DESC;
END;
GO

-- 5. CREAR EXPERIENCIA

CREATE OR ALTER PROCEDURE dbo.sp_crear_experiencia_docente
    @NombreCargo VARCHAR(45),
    @Institucion VARCHAR(45),
    @Tipo VARCHAR(45),
    @FechaInicio DATE,
    @FechaFin DATE = NULL,
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.docente
        WHERE cedula = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51002,
            'El docente no existe o se encuentra inactivo.',
            1;
    END;

    IF @FechaFin IS NOT NULL
       AND @FechaFin < @FechaInicio
    BEGIN
        THROW 51003,
            'La fecha final no puede ser anterior a la inicial.',
            1;
    END;

    INSERT INTO dbo.experiecia
    (
        nombre_cargo,
        institucion,
        tipo,
        fecha_inicio,
        fecha_fin,
        docente,
        activo
    )
    VALUES
    (
        @NombreCargo,
        @Institucion,
        @Tipo,
        @FechaInicio,
        @FechaFin,
        @Docente,
        1
    );

    DECLARE @NuevoId INT =
        CONVERT(INT, SCOPE_IDENTITY());

    EXEC dbo.sp_consultar_experiencia_docente
        @Id = @NuevoId;
END;
GO

-- 6. ACTUALIZAR EXPERIENCIA

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_experiencia_docente
    @Id INT,
    @NombreCargo VARCHAR(45),
    @Institucion VARCHAR(45),
    @Tipo VARCHAR(45),
    @FechaInicio DATE,
    @FechaFin DATE = NULL,
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.docente
        WHERE cedula = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51002,
            'El docente no existe o se encuentra inactivo.',
            1;
    END;

    IF @FechaFin IS NOT NULL
       AND @FechaFin < @FechaInicio
    BEGIN
        THROW 51003,
            'La fecha final no puede ser anterior a la inicial.',
            1;
    END;

    UPDATE dbo.experiecia
    SET
        nombre_cargo = @NombreCargo,
        institucion = @Institucion,
        tipo = @Tipo,
        fecha_inicio = @FechaInicio,
        fecha_fin = @FechaFin,
        docente = @Docente
    WHERE id = @Id
      AND activo = 1;

    EXEC dbo.sp_consultar_experiencia_docente
        @Id = @Id;
END;
GO

-- 7. RETIRAR EXPERIENCIA (BORRADO LÓGICO)

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_experiencia_docente
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.experiecia
    SET activo = 0
    WHERE id = @Id
      AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
