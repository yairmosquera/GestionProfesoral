
USE gestion_local;
GO

-- =====================================================
-- GESTIÓN PROFESORAL V2
-- MÓDULO: EVALUACIÓN DOCENTE
-- =====================================================

-- 1. CREAR TABLA SI NO EXISTE

IF OBJECT_ID('dbo.evaluacion_docente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.evaluacion_docente
    (
        id INT IDENTITY(1,1) NOT NULL,
        calificacion REAL NOT NULL,
        semestre VARCHAR(45) NOT NULL,
        docente INT NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_evaluacion_docente_activo
            DEFAULT (1),

        CONSTRAINT PK_evaluacion_docente
            PRIMARY KEY (id),

        CONSTRAINT FK_evaluacion_docente_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula)
    );
END;
GO

-- 2. LISTAR EVALUACIONES ACTIVAS

CREATE OR ALTER PROCEDURE dbo.sp_listar_evaluacion_docente
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        calificacion AS Calificacion,
        semestre AS Semestre,
        docente AS Docente,
        activo AS Activo
    FROM dbo.evaluacion_docente
    WHERE activo = 1
    ORDER BY id DESC;
END;
GO

-- 3. CONSULTAR EVALUACIÓN POR ID

CREATE OR ALTER PROCEDURE dbo.sp_consultar_evaluacion_docente
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        calificacion AS Calificacion,
        semestre AS Semestre,
        docente AS Docente,
        activo AS Activo
    FROM dbo.evaluacion_docente
    WHERE id = @Id
      AND activo = 1;
END;
GO

-- 4. CONSULTAR EVALUACIONES DE UN DOCENTE

CREATE OR ALTER PROCEDURE dbo.sp_evaluaciones_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        calificacion AS Calificacion,
        semestre AS Semestre,
        docente AS Docente,
        activo AS Activo
    FROM dbo.evaluacion_docente
    WHERE docente = @Cedula
      AND activo = 1
    ORDER BY id DESC;
END;
GO

-- 5. CREAR EVALUACIÓN

CREATE OR ALTER PROCEDURE dbo.sp_crear_evaluacion_docente
    @Calificacion REAL,
    @Semestre VARCHAR(45),
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.docente
        WHERE cedula = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51002,
            'El docente no existe o esta retirado.',
            1;
    END;

    INSERT INTO dbo.evaluacion_docente
    (
        calificacion,
        semestre,
        docente,
        activo
    )
    VALUES
    (
        @Calificacion,
        @Semestre,
        @Docente,
        1
    );

    DECLARE @NuevoId INT = CONVERT(
        INT, SCOPE_IDENTITY());

    EXEC dbo.sp_consultar_evaluacion_docente
        @Id = @NuevoId;
END;
GO

-- 6. ACTUALIZAR EVALUACIÓN

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_evaluacion_docente
    @Id INT,
    @Calificacion REAL,
    @Semestre VARCHAR(45),
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.docente
        WHERE cedula = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51002,
            'El docente no existe o esta retirado.',
            1;
    END;

    UPDATE dbo.evaluacion_docente
    SET
        calificacion = @Calificacion,
        semestre = @Semestre,
        docente = @Docente
    WHERE id = @Id
      AND activo = 1;

    IF @@ROWCOUNT = 0
        RETURN;

    EXEC dbo.sp_consultar_evaluacion_docente
        @Id = @Id;
END;
GO

-- 7. RETIRO LÓGICO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_evaluacion_docente
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.evaluacion_docente
    SET activo = 0
    WHERE id = @Id
      AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

-- 8. VERIFICACIÓN

SELECT
    name AS Procedimiento
FROM sys.procedures
WHERE name IN
(
    'sp_listar_evaluacion_docente',
    'sp_consultar_evaluacion_docente',
    'sp_evaluaciones_por_docente',
    'sp_crear_evaluacion_docente',
    'sp_actualizar_evaluacion_docente',
    'sp_eliminar_evaluacion_docente'
)
ORDER BY name;
GO
