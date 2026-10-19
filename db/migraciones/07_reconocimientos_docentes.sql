
USE gestion_local;
GO

-- 1. CREAR TABLA SI NO EXISTE

IF OBJECT_ID('dbo.reconocimiento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.reconocimiento
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        tipo VARCHAR(45) NOT NULL,
        fecha DATE NOT NULL,
        institucion VARCHAR(45) NOT NULL,
        nombre VARCHAR(45) NOT NULL,
        ambito VARCHAR(45) NOT NULL,
        docente INT NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_reconocimiento_activo DEFAULT (1),

        CONSTRAINT FK_reconocimiento_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula)
    );
END;
GO

-- 2. LISTAR RECONOCIMIENTOS ACTIVOS

CREATE OR ALTER PROCEDURE dbo.sp_listar_reconocimiento
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        tipo AS Tipo,
        fecha AS Fecha,
        institucion AS Institucion,
        nombre AS Nombre,
        ambito AS Ambito,
        docente AS Docente,
        activo AS Activo
    FROM dbo.reconocimiento
    WHERE activo = 1
    ORDER BY id DESC;
END;
GO

-- 3. CONSULTAR RECONOCIMIENTO POR ID

CREATE OR ALTER PROCEDURE dbo.sp_consultar_reconocimiento
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        tipo AS Tipo,
        fecha AS Fecha,
        institucion AS Institucion,
        nombre AS Nombre,
        ambito AS Ambito,
        docente AS Docente,
        activo AS Activo
    FROM dbo.reconocimiento
    WHERE id = @Id
      AND activo = 1;
END;
GO

-- 4. CONSULTAR RECONOCIMIENTOS POR DOCENTE

CREATE OR ALTER PROCEDURE dbo.sp_reconocimientos_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        tipo AS Tipo,
        fecha AS Fecha,
        institucion AS Institucion,
        nombre AS Nombre,
        ambito AS Ambito,
        docente AS Docente,
        activo AS Activo
    FROM dbo.reconocimiento
    WHERE docente = @Cedula
      AND activo = 1
    ORDER BY fecha DESC, id DESC;
END;
GO

-- 5. CREAR RECONOCIMIENTO

CREATE OR ALTER PROCEDURE dbo.sp_crear_reconocimiento
    @Tipo VARCHAR(45),
    @Fecha DATE,
    @Institucion VARCHAR(45),
    @Nombre VARCHAR(45),
    @Ambito VARCHAR(45),
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

    INSERT INTO dbo.reconocimiento
    (
        tipo,
        fecha,
        institucion,
        nombre,
        ambito,
        docente,
        activo
    )
    VALUES
    (
        @Tipo,
        @Fecha,
        @Institucion,
        @Nombre,
        @Ambito,
        @Docente,
        1
    );

    DECLARE @NuevoId INT =
        CONVERT(INT, SCOPE_IDENTITY());

    EXEC dbo.sp_consultar_reconocimiento
        @Id = @NuevoId;
END;
GO

-- 6. ACTUALIZAR RECONOCIMIENTO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_reconocimiento
    @Id INT,
    @Tipo VARCHAR(45),
    @Fecha DATE,
    @Institucion VARCHAR(45),
    @Nombre VARCHAR(45),
    @Ambito VARCHAR(45),
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

    UPDATE dbo.reconocimiento
    SET
        tipo = @Tipo,
        fecha = @Fecha,
        institucion = @Institucion,
        nombre = @Nombre,
        ambito = @Ambito,
        docente = @Docente
    WHERE id = @Id
      AND activo = 1;

    EXEC dbo.sp_consultar_reconocimiento
        @Id = @Id;
END;
GO

-- 7. RETIRAR RECONOCIMIENTO (BORRADO LÓGICO)

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_reconocimiento
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.reconocimiento
    SET activo = 0
    WHERE id = @Id
      AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
