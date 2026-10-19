
USE gestion_local;
GO

-- ============================================
-- 1. CREAR TABLA SI NO EXISTE
-- ============================================

IF OBJECT_ID('dbo.docente_departamento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.docente_departamento
    (
        docente INT NOT NULL,
        departamento INT NOT NULL,
        dedicacion VARCHAR(15) NOT NULL,
        modalidad VARCHAR(45) NOT NULL,
        fecha_ingreso DATE NOT NULL,
        fecha_salida DATE NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_docente_departamento_activo DEFAULT (1),

        CONSTRAINT PK_docente_departamento
            PRIMARY KEY (docente, departamento),

        CONSTRAINT FK_docente_departamento_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula),

        CONSTRAINT FK_docente_departamento_programa
            FOREIGN KEY (departamento)
            REFERENCES dbo.programa(id)
    );
END;
GO

-- ============================================
-- 2. LISTAR ASOCIACIONES ACTIVAS
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_listar_docente_departamento
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        docente AS Docente,
        departamento AS Departamento,
        dedicacion AS Dedicacion,
        modalidad AS Modalidad,
        fecha_ingreso AS FechaIngreso,
        fecha_salida AS FechaSalida,
        activo AS Activo
    FROM dbo.docente_departamento
    WHERE activo = 1
    ORDER BY docente, departamento;
END;
GO

-- ============================================
-- 3. CONSULTAR POR CLAVE COMPUESTA
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_consultar_docente_departamento
    @Docente INT,
    @Departamento INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        docente AS Docente,
        departamento AS Departamento,
        dedicacion AS Dedicacion,
        modalidad AS Modalidad,
        fecha_ingreso AS FechaIngreso,
        fecha_salida AS FechaSalida,
        activo AS Activo
    FROM dbo.docente_departamento
    WHERE docente = @Docente
      AND departamento = @Departamento
      AND activo = 1;
END;
GO

-- ============================================
-- 4. CONSULTAR DEPARTAMENTOS POR DOCENTE
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_departamentos_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        docente AS Docente,
        departamento AS Departamento,
        dedicacion AS Dedicacion,
        modalidad AS Modalidad,
        fecha_ingreso AS FechaIngreso,
        fecha_salida AS FechaSalida,
        activo AS Activo
    FROM dbo.docente_departamento
    WHERE docente = @Cedula
      AND activo = 1
    ORDER BY fecha_ingreso DESC, departamento;
END;
GO

-- ============================================
-- 5. CREAR ASOCIACIÓN
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_crear_docente_departamento
    @Docente INT,
    @Departamento INT,
    @Dedicacion VARCHAR(15),
    @Modalidad VARCHAR(45),
    @FechaIngreso DATE,
    @FechaSalida DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @FechaSalida IS NOT NULL
       AND @FechaSalida < @FechaIngreso
    BEGIN
        THROW 51004,
            'La fecha de salida no puede ser anterior a la fecha de ingreso.',
            1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

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

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.programa
            WHERE id = @Departamento
              AND activo = 1
        )
        BEGIN
            THROW 51003,
                'El programa no existe o se encuentra inactivo.',
                1;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.docente_departamento WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 1
        )
        BEGIN
            THROW 51005,
                'El docente ya se encuentra asociado a este departamento.',
                1;
        END;

        -- REACTIVAR UNA ASOCIACIÓN RETIRADA

        IF EXISTS
        (
            SELECT 1
            FROM dbo.docente_departamento WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @Docente
              AND departamento = @Departamento
              AND activo = 0
        )
        BEGIN
            UPDATE dbo.docente_departamento
            SET
                dedicacion = @Dedicacion,
                modalidad = @Modalidad,
                fecha_ingreso = @FechaIngreso,
                fecha_salida = @FechaSalida,
                activo = 1
            WHERE docente = @Docente
              AND departamento = @Departamento;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.docente_departamento
            (
                docente,
                departamento,
                dedicacion,
                modalidad,
                fecha_ingreso,
                fecha_salida,
                activo
            )
            VALUES
            (
                @Docente,
                @Departamento,
                @Dedicacion,
                @Modalidad,
                @FechaIngreso,
                @FechaSalida,
                1
            );
        END;

        COMMIT TRANSACTION;

        EXEC dbo.sp_consultar_docente_departamento
            @Docente = @Docente,
            @Departamento = @Departamento;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO

-- ============================================
-- 6. ACTUALIZAR ASOCIACIÓN
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_docente_departamento
    @Docente INT,
    @Departamento INT,
    @Dedicacion VARCHAR(15),
    @Modalidad VARCHAR(45),
    @FechaIngreso DATE,
    @FechaSalida DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FechaSalida IS NOT NULL
       AND @FechaSalida < @FechaIngreso
    BEGIN
        THROW 51004,
            'La fecha de salida no puede ser anterior a la fecha de ingreso.',
            1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.docente_departamento
        WHERE docente = @Docente
          AND departamento = @Departamento
          AND activo = 1
    )
    BEGIN
        THROW 51006,
            'No existe una asociación activa para actualizar.',
            1;
    END;

    UPDATE dbo.docente_departamento
    SET
        dedicacion = @Dedicacion,
        modalidad = @Modalidad,
        fecha_ingreso = @FechaIngreso,
        fecha_salida = @FechaSalida
    WHERE docente = @Docente
      AND departamento = @Departamento
      AND activo = 1;

    EXEC dbo.sp_consultar_docente_departamento
        @Docente = @Docente,
        @Departamento = @Departamento;
END;
GO

-- ============================================
-- 7. RETIRAR ASOCIACIÓN (BORRADO LÓGICO)
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_docente_departamento
    @Docente INT,
    @Departamento INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.docente_departamento
    SET activo = 0
    WHERE docente = @Docente
      AND departamento = @Departamento
      AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
