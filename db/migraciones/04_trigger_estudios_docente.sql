
USE gestion_local;
GO

-- =====================================================
-- GESTIÓN PROFESORAL - VERSIÓN 2
-- TRIGGER: CANTIDAD DE ESTUDIOS POR DOCENTE
-- =====================================================

-- PASO 1: Agregar una columna para almacenar
-- la cantidad de estudios activos del docente.

IF COL_LENGTH('dbo.docente', 'cantidad_estudios') IS NULL
BEGIN
    ALTER TABLE dbo.docente
    ADD cantidad_estudios INT NOT NULL
        CONSTRAINT DF_docente_cantidad_estudios
        DEFAULT (0);
END;
GO

-- PASO 2: Crear o actualizar el trigger.
-- Se ejecuta cuando un estudio se inserta,
-- modifica o elimina físicamente.
-- También detecta cambios de activo y docente.

CREATE OR ALTER TRIGGER dbo.trg_actualizar_cantidad_estudios
ON dbo.estudios_realizados
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Recalcular los estudios activos únicamente
    -- para los docentes afectados por el cambio.

    UPDATE d
    SET cantidad_estudios = (
        SELECT COUNT(*)
        FROM dbo.estudios_realizados e
        WHERE e.docente = d.cedula
          AND e.activo = 1
    )
    FROM dbo.docente d
    WHERE d.cedula IN
    (
        SELECT docente FROM inserted
        UNION
        SELECT docente FROM deleted
    );
END;
GO

-- PASO 3: Sincronizar los docentes existentes.
-- Esto cubre los estudios registrados antes
-- de crear el trigger.

UPDATE d
SET cantidad_estudios = (
    SELECT COUNT(*)
    FROM dbo.estudios_realizados e
    WHERE e.docente = d.cedula
      AND e.activo = 1
)
FROM dbo.docente d;
GO

-- PASO 4: Consultar los resultados.

SELECT
    d.cedula,
    d.nombres,
    d.apellidos,
    d.cantidad_estudios
FROM dbo.docente d
ORDER BY d.cedula;
GO

-- PASO 5: Verificar que el trigger exista.

SELECT
    t.name AS NombreTrigger,
    t.is_disabled AS EstaDeshabilitado
FROM sys.triggers t
WHERE t.parent_id =
    OBJECT_ID('dbo.estudios_realizados');
GO
