
USE gestion_local;
GO

-- ============================================
-- 1. CREAR TABLA SI NO EXISTE
-- ============================================

IF OBJECT_ID('dbo.red_docente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.red_docente
    (
        red INT NOT NULL,
        docente INT NOT NULL,
        fecha_inicio DATE NOT NULL,
        fecha_fin VARCHAR(45) NULL,
        act_destacadas VARCHAR(MAX) NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_red_docente_activo DEFAULT (1),

        CONSTRAINT PK_red_docente
            PRIMARY KEY (red, docente),

        CONSTRAINT FK_red_docente_red
            FOREIGN KEY (red)
            REFERENCES dbo.red(idr),

        CONSTRAINT FK_red_docente_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula)
    );
END;
GO

-- ============================================
-- 2. LISTAR ASOCIACIONES ACTIVAS
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_listar_red_docente
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        red AS Red,
        docente AS Docente,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        act_destacadas AS ActDestacadas,
        activo AS Activo
    FROM dbo.red_docente
    WHERE activo = 1
    ORDER BY docente, red;
END;
GO

-- ============================================
-- 3. CONSULTAR UNA ASOCIACIÓN
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_consultar_red_docente
    @Red INT,
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        red AS Red,
        docente AS Docente,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        act_destacadas AS ActDestacadas,
        activo AS Activo
    FROM dbo.red_docente
    WHERE red = @Red
      AND docente = @Docente
      AND activo = 1;
END;
GO

-- ============================================
-- 4. CONSULTAR REDES POR DOCENTE
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_redes_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        red AS Red,
        docente AS Docente,
        fecha_inicio AS FechaInicio,
        fecha_fin AS FechaFin,
        act_destacadas AS ActDestacadas,
        activo AS Activo
    FROM dbo.red_docente
    WHERE docente = @Cedula
      AND activo = 1
    ORDER BY fecha_inicio DESC, red;
END;
GO

-- ============================================
-- 5. CREAR ASOCIACIÓN
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_crear_red_docente
    @Red INT,
    @Docente INT,
    @FechaInicio DATE,
    @FechaFin VARCHAR(45) = NULL,
    @ActDestacadas VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.red
            WHERE idr = @Red
              AND activo = 1
        )
        BEGIN
            THROW 51002,
                'La red no existe o se encuentra inactiva.',
                1;
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.docente
            WHERE cedula = @Docente
              AND activo = 1
        )
        BEGIN
            THROW 51003,
                'El docente no existe o se encuentra inactivo.',
                1;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.red_docente WITH (UPDLOCK, HOLDLOCK)
            WHERE red = @Red
              AND docente = @Docente
              AND activo = 1
        )
        BEGIN
            THROW 51004,
                'El docente ya está asociado a esta red.',
                1;
        END;

        -- Si existe una relación retirada,
        -- se reactiva en lugar de insertar otra
        -- con la misma clave primaria.

        IF EXISTS
        (
            SELECT 1
            FROM dbo.red_docente WITH (UPDLOCK, HOLDLOCK)
            WHERE red = @Red
              AND docente = @Docente
              AND activo = 0
        )
        BEGIN
            UPDATE dbo.red_docente
            SET
                fecha_inicio = @FechaInicio,
                fecha_fin = @FechaFin,
                act_destacadas = @ActDestacadas,
                activo = 1
            WHERE red = @Red
              AND docente = @Docente;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.red_docente
            (
                red,
                docente,
                fecha_inicio,
                fecha_fin,
                act_destacadas,
                activo
            )
            VALUES
            (
                @Red,
                @Docente,
                @FechaInicio,
                @FechaFin,
                @ActDestacadas,
                1
            );
        END;

        COMMIT TRANSACTION;

        EXEC dbo.sp_consultar_red_docente
            @Red = @Red,
            @Docente = @Docente;
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

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_red_docente
    @Red INT,
    @Docente INT,
    @FechaInicio DATE,
    @FechaFin VARCHAR(45) = NULL,
    @ActDestacadas VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.red_docente
        WHERE red = @Red
          AND docente = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51005,
            'No existe una asociación activa para actualizar.',
            1;
    END;

    UPDATE dbo.red_docente
    SET
        fecha_inicio = @FechaInicio,
        fecha_fin = @FechaFin,
        act_destacadas = @ActDestacadas
    WHERE red = @Red
      AND docente = @Docente
      AND activo = 1;

    EXEC dbo.sp_consultar_red_docente
        @Red = @Red,
        @Docente = @Docente;
END;
GO

-- ============================================
-- 7. RETIRAR ASOCIACIÓN
-- ============================================

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_red_docente
    @Red INT,
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.red_docente
    SET activo = 0
    WHERE red = @Red
      AND docente = @Docente
      AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
