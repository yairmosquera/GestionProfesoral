
USE gestion_local;
GO

-- =====================================================
-- VALIDACIONES PREVIAS
-- =====================================================

IF OBJECT_ID('dbo.estudios_realizados', 'U') IS NULL
BEGIN
    THROW 51010, 'La tabla estudios_realizados no existe.', 1;
END;
GO

-- Si el ID ya es IDENTITY no se necesita la secuencia.
IF COLUMNPROPERTY(
    OBJECT_ID('dbo.estudios_realizados'),
    'id',
    'IsIdentity'
) = 1
BEGIN
    THROW 51011, 'El ID ya es IDENTITY. Revise la migracion.', 1;
END;
GO

-- =====================================================
-- SECUENCIA PARA EL ID AUTOMÁTICO
-- =====================================================

IF OBJECT_ID('dbo.seq_estudios_realizados', 'SO') IS NULL
BEGIN
    DECLARE @Siguiente BIGINT;

    SELECT @Siguiente = ISNULL(MAX(CONVERT(BIGINT, id)), 0) + 1
    FROM dbo.estudios_realizados;

    DECLARE @Sql NVARCHAR(MAX);

    SET @Sql =
        N'CREATE SEQUENCE dbo.seq_estudios_realizados
          AS INT
          START WITH ' + CAST(@Siguiente AS NVARCHAR(20)) +
        N' INCREMENT BY 1 NO CYCLE;';

    EXEC sys.sp_executesql @Sql;
END;
GO

-- Valor predeterminado de ID.
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c
        ON c.object_id = dc.parent_object_id
        AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id =
        OBJECT_ID('dbo.estudios_realizados')
      AND c.name = 'id'
)
BEGIN
    ALTER TABLE dbo.estudios_realizados
    ADD CONSTRAINT DF_estudios_id
    DEFAULT (
        NEXT VALUE FOR dbo.seq_estudios_realizados
    ) FOR id;
END;
GO

-- =====================================================
-- CREAR ESTUDIO SIN ENVIAR ID
-- =====================================================

CREATE OR ALTER PROCEDURE dbo.sp_crear_estudios_realizados
    @Titulo VARCHAR(45),
    @Universidad VARCHAR(50),
    @Fecha DATE,
    @Tipo VARCHAR(45),
    @Ciudad VARCHAR(45),
    @Docente INT,
    @InsAcreditada TINYINT,
    @Metodologia VARCHAR(45),
    @PerfilEgresado VARCHAR(MAX),
    @Pais VARCHAR(45)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.docente
        WHERE cedula = @Docente
          AND activo = 1
    )
    BEGIN
        THROW 51002,
            'El docente no existe o esta retirado.', 1;
    END;

    DECLARE @Nuevo TABLE (Id INT);

    INSERT INTO dbo.estudios_realizados
    (
        titulo,
        universidad,
        fecha,
        tipo,
        ciudad,
        docente,
        ins_acreditada,
        metodologia,
        perfil_egresado,
        pais,
        activo
    )
    OUTPUT INSERTED.id INTO @Nuevo
    VALUES
    (
        @Titulo,
        @Universidad,
        @Fecha,
        @Tipo,
        @Ciudad,
        @Docente,
        @InsAcreditada,
        @Metodologia,
        @PerfilEgresado,
        @Pais,
        1
    );

    DECLARE @NuevoId INT;

    SELECT @NuevoId = Id FROM @Nuevo;

    EXEC dbo.sp_consultar_estudios_realizados @NuevoId;
END;
GO

-- =====================================================
-- COMPROBACIÓN
-- =====================================================

SELECT
    c.name AS Columna,
    dc.definition AS ValorAutomatico
FROM sys.columns c
LEFT JOIN sys.default_constraints dc
    ON dc.parent_object_id = c.object_id
   AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('dbo.estudios_realizados')
  AND c.name = 'id';

SELECT
    name AS Procedimiento
FROM sys.procedures
WHERE name = 'sp_crear_estudios_realizados';
GO
