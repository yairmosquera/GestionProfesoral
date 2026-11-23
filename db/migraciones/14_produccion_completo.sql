-- Instalacion idempotente para ejecutar despues de seleccionar db81973 en WebMSSQL.
-- No cambia de base de datos ni elimina tablas existentes.
-- GO es un separador de lotes requerido por CREATE OR ALTER PROCEDURE.

IF OBJECT_ID(N'dbo.area_conocimiento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.area_conocimiento
    (
        id VARCHAR(6) NOT NULL CONSTRAINT PK_area_conocimiento PRIMARY KEY,
        gran_area VARCHAR(60) NOT NULL,
        area VARCHAR(60) NOT NULL,
        disciplina VARCHAR(150) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_area_conocimiento_activo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.termino_clave', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.termino_clave
    (
        termino VARCHAR(30) NOT NULL CONSTRAINT PK_termino_clave PRIMARY KEY,
        termino_ingles VARCHAR(30) NULL,
        activo BIT NOT NULL CONSTRAINT DF_termino_clave_activo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.linea_investigacion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.linea_investigacion
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_linea_investigacion PRIMARY KEY,
        nombre VARCHAR(45) NOT NULL,
        descripcion VARCHAR(256) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_linea_investigacion_activo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.programa', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.programa
    (
        id INT NOT NULL CONSTRAINT PK_programa PRIMARY KEY,
        nombre VARCHAR(60) NOT NULL,
        tipo VARCHAR(45) NOT NULL,
        nivel VARCHAR(45) NOT NULL,
        fecha_creacion VARCHAR(45) NOT NULL,
        fecha_cierre VARCHAR(45) NULL,
        numero_cohortes VARCHAR(45) NOT NULL,
        cant_graduados VARCHAR(45) NOT NULL,
        fecha_actualizacion VARCHAR(45) NOT NULL,
        ciudad VARCHAR(45) NOT NULL,
        facultad INT NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_programa_activo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.red', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.red
    (
        idr INT NOT NULL CONSTRAINT PK_red PRIMARY KEY,
        nombre VARCHAR(45) NOT NULL,
        url VARCHAR(45) NOT NULL,
        pais VARCHAR(45) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_red_activo DEFAULT (1)
    );
END;
GO


-- ============================================================
-- Migracion 01_docente.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
GO

-- No elimina ninguna tabla existente.
IF OBJECT_ID('dbo.docente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.docente
    (
        cedula INT NOT NULL PRIMARY KEY,
        nombres VARCHAR(60) NOT NULL,
        apellidos VARCHAR(60) NOT NULL,
        genero VARCHAR(12) NOT NULL,
        cargo VARCHAR(30) NOT NULL,
        fecha_nacimiento DATE NOT NULL,
        correo VARCHAR(70) NOT NULL,
        telefono VARCHAR(20) NOT NULL,
        url_cvlac VARCHAR(128) NOT NULL,
        fecha_actualizacion DATE NOT NULL,
        escalafon VARCHAR(45) NOT NULL,
        perfil VARCHAR(MAX) NOT NULL,
        cat_minciencia VARCHAR(45) NULL,
        conv_minciencia VARCHAR(45) NOT NULL,
        nacionalidaad VARCHAR(45) NOT NULL,
        linea_investigacion_principal INT NULL,
        activo BIT NOT NULL DEFAULT 1,

        CONSTRAINT FK_docente_linea
        FOREIGN KEY (linea_investigacion_principal)
        REFERENCES dbo.linea_investigacion(id)
    );
END;
GO

-- LISTAR
CREATE OR ALTER PROCEDURE dbo.sp_listar_docente
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.docente
    WHERE activo = 1
    ORDER BY apellidos, nombres;
END;
GO

-- CONSULTAR
CREATE OR ALTER PROCEDURE dbo.sp_consultar_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.docente
    WHERE cedula = @Cedula AND activo = 1;
END;
GO

-- CREAR
CREATE OR ALTER PROCEDURE dbo.sp_crear_docente
    @Cedula INT,
    @Nombres VARCHAR(60),
    @Apellidos VARCHAR(60),
    @Genero VARCHAR(12),
    @Cargo VARCHAR(30),
    @FechaNacimiento DATE,
    @Correo VARCHAR(70),
    @Telefono VARCHAR(20),
    @UrlCvlac VARCHAR(128),
    @Escalafon VARCHAR(45),
    @Perfil VARCHAR(MAX),
    @CatMinciencia VARCHAR(45) = NULL,
    @ConvMinciencia VARCHAR(45),
    @Nacionalidaad VARCHAR(45),
    @LineaInvestigacionPrincipal INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    INSERT INTO dbo.docente
    (
        cedula, nombres, apellidos, genero, cargo,
        fecha_nacimiento, correo, telefono, url_cvlac,
        fecha_actualizacion, escalafon, perfil,
        cat_minciencia, conv_minciencia, nacionalidaad,
        linea_investigacion_principal, activo
    )
    VALUES
    (
        @Cedula, @Nombres, @Apellidos, @Genero, @Cargo,
        @FechaNacimiento, @Correo, @Telefono, @UrlCvlac,
        CAST(GETDATE() AS DATE), @Escalafon, @Perfil,
        @CatMinciencia, @ConvMinciencia, @Nacionalidaad,
        @LineaInvestigacionPrincipal, 1
    );

    EXEC dbo.sp_consultar_docente @Cedula;
END;
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE dbo.sp_actualizar_docente
    @Cedula INT,
    @Nombres VARCHAR(60),
    @Apellidos VARCHAR(60),
    @Genero VARCHAR(12),
    @Cargo VARCHAR(30),
    @FechaNacimiento DATE,
    @Correo VARCHAR(70),
    @Telefono VARCHAR(20),
    @UrlCvlac VARCHAR(128),
    @Escalafon VARCHAR(45),
    @Perfil VARCHAR(MAX),
    @CatMinciencia VARCHAR(45) = NULL,
    @ConvMinciencia VARCHAR(45),
    @Nacionalidaad VARCHAR(45),
    @LineaInvestigacionPrincipal INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE dbo.docente
    SET nombres = @Nombres,
        apellidos = @Apellidos,
        genero = @Genero,
        cargo = @Cargo,
        fecha_nacimiento = @FechaNacimiento,
        correo = @Correo,
        telefono = @Telefono,
        url_cvlac = @UrlCvlac,
        fecha_actualizacion = CAST(GETDATE() AS DATE),
        escalafon = @Escalafon,
        perfil = @Perfil,
        cat_minciencia = @CatMinciencia,
        conv_minciencia = @ConvMinciencia,
        nacionalidaad = @Nacionalidaad,
        linea_investigacion_principal =
            @LineaInvestigacionPrincipal
    WHERE cedula = @Cedula AND activo = 1;

    EXEC dbo.sp_consultar_docente @Cedula;
END;
GO

-- RETIRAR LÓGICAMENTE
CREATE OR ALTER PROCEDURE dbo.sp_eliminar_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    -- Impide retirar un docente con relaciones registradas.
    -- Las comprobaciones se ampliarán cuando instalemos los detalles.
    IF OBJECT_ID('dbo.estudios_realizados', 'U') IS NOT NULL
    BEGIN
        -- La validación de detalles se implementará al
        -- incorporar sus tablas en la siguiente etapa.
        -- Por ahora no se debe retirar un docente
        -- con registros relacionados.
        IF EXISTS (
            SELECT 1
            FROM sys.foreign_keys
            WHERE referenced_object_id = OBJECT_ID('dbo.docente')
              AND parent_object_id <> OBJECT_ID('dbo.docente')
        )
        BEGIN
            ROLLBACK TRANSACTION;
            ;THROW 51001,
                'Revise las relaciones del docente antes de retirarlo.',
                1;
        END
    END

    UPDATE dbo.docente
    SET activo = 0
    WHERE cedula = @Cedula AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;

    COMMIT TRANSACTION;
END;
GO


-- ============================================================
-- Migracion 02_estudios_realizados.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
GO

-- Tabla de estudios realizados
IF OBJECT_ID('dbo.estudios_realizados', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.estudios_realizados
    (
        id INT NOT NULL PRIMARY KEY,
        titulo VARCHAR(45) NOT NULL,
        universidad VARCHAR(50) NOT NULL,
        fecha DATE NOT NULL,
        tipo VARCHAR(45) NOT NULL,
        ciudad VARCHAR(45) NOT NULL,
        docente INT NOT NULL,
        ins_acreditada TINYINT NOT NULL,
        metodologia VARCHAR(45) NOT NULL,
        perfil_egresado VARCHAR(MAX) NOT NULL,
        pais VARCHAR(45) NOT NULL,
        activo BIT NOT NULL DEFAULT 1,

        CONSTRAINT FK_estudios_docente
            FOREIGN KEY (docente)
            REFERENCES dbo.docente(cedula),

        CONSTRAINT CK_estudios_acreditada
            CHECK (ins_acreditada IN (0, 1))
    );
END;
GO

-- LISTAR TODOS LOS ESTUDIOS ACTIVOS
CREATE OR ALTER PROCEDURE dbo.sp_listar_estudios_realizados
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.id AS Id,
        e.titulo AS Titulo,
        e.universidad AS Universidad,
        e.fecha AS Fecha,
        e.tipo AS Tipo,
        e.ciudad AS Ciudad,
        e.docente AS Docente,
        e.ins_acreditada AS InsAcreditada,
        e.metodologia AS Metodologia,
        e.perfil_egresado AS PerfilEgresado,
        e.pais AS Pais,
        e.activo AS Activo
    FROM dbo.estudios_realizados e
    WHERE e.activo = 1
    ORDER BY e.id;
END;
GO

-- CONSULTAR POR ID
CREATE OR ALTER PROCEDURE dbo.sp_consultar_estudios_realizados
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        titulo AS Titulo,
        universidad AS Universidad,
        fecha AS Fecha,
        tipo AS Tipo,
        ciudad AS Ciudad,
        docente AS Docente,
        ins_acreditada AS InsAcreditada,
        metodologia AS Metodologia,
        perfil_egresado AS PerfilEgresado,
        pais AS Pais,
        activo AS Activo
    FROM dbo.estudios_realizados
    WHERE id = @Id AND activo = 1;
END;
GO

-- LISTAR ESTUDIOS DE UN DOCENTE
CREATE OR ALTER PROCEDURE dbo.sp_estudios_por_docente
    @Cedula INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id AS Id,
        titulo AS Titulo,
        universidad AS Universidad,
        fecha AS Fecha,
        tipo AS Tipo,
        ciudad AS Ciudad,
        docente AS Docente,
        ins_acreditada AS InsAcreditada,
        metodologia AS Metodologia,
        perfil_egresado AS PerfilEgresado,
        pais AS Pais,
        activo AS Activo
    FROM dbo.estudios_realizados
    WHERE docente = @Cedula AND activo = 1
    ORDER BY fecha DESC;
END;
GO

-- CREAR ESTUDIO
CREATE OR ALTER PROCEDURE dbo.sp_crear_estudios_realizados
    @Id INT,
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

    -- Verificar que el docente esté activo
    IF NOT EXISTS (
        SELECT 1 FROM dbo.docente
        WHERE cedula = @Docente AND activo = 1
    )
    BEGIN
        ;THROW 51002, 'El docente seleccionado no existe o está retirado.', 1;
    END;

    INSERT INTO dbo.estudios_realizados
    (
        id, titulo, universidad, fecha, tipo,
        ciudad, docente, ins_acreditada,
        metodologia, perfil_egresado, pais, activo
    )
    VALUES
    (
        @Id, @Titulo, @Universidad, @Fecha, @Tipo,
        @Ciudad, @Docente, @InsAcreditada,
        @Metodologia, @PerfilEgresado, @Pais, 1
    );

    EXEC dbo.sp_consultar_estudios_realizados @Id;
END;
GO

-- ACTUALIZAR ESTUDIO
CREATE OR ALTER PROCEDURE dbo.sp_actualizar_estudios_realizados
    @Id INT,
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
        SELECT 1 FROM dbo.docente
        WHERE cedula = @Docente AND activo = 1
    )
    BEGIN
        ;THROW 51002, 'El docente seleccionado no existe o está retirado.', 1;
    END;

    UPDATE dbo.estudios_realizados
    SET
        titulo = @Titulo,
        universidad = @Universidad,
        fecha = @Fecha,
        tipo = @Tipo,
        ciudad = @Ciudad,
        docente = @Docente,
        ins_acreditada = @InsAcreditada,
        metodologia = @Metodologia,
        perfil_egresado = @PerfilEgresado,
        pais = @Pais
    WHERE id = @Id AND activo = 1;

    EXEC dbo.sp_consultar_estudios_realizados @Id;
END;
GO

-- RETIRAR ESTUDIO
CREATE OR ALTER PROCEDURE dbo.sp_eliminar_estudios_realizados
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE dbo.estudios_realizados
    SET activo = 0
    WHERE id = @Id AND activo = 1;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO


-- ============================================================
-- Migracion 03_id_automatico_estudios.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
GO

-- =====================================================
-- VALIDACIONES PREVIAS
-- =====================================================

IF OBJECT_ID('dbo.estudios_realizados', 'U') IS NULL
BEGIN
    ;THROW 51010, 'La tabla estudios_realizados no existe.', 1;
END;
GO

-- Si el ID ya es IDENTITY, la secuencia y el DEFAULT no son necesarios.
GO

-- =====================================================
-- SECUENCIA PARA EL ID AUTOMÁTICO
-- =====================================================

IF COLUMNPROPERTY(OBJECT_ID('dbo.estudios_realizados'), 'id', 'IsIdentity') = 0
   AND OBJECT_ID('dbo.seq_estudios_realizados', 'SO') IS NULL
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
IF COLUMNPROPERTY(OBJECT_ID('dbo.estudios_realizados'), 'id', 'IsIdentity') = 0
AND NOT EXISTS (
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
        ;THROW 51002,
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


-- ============================================================
-- Migracion 04_trigger_estudios_docente.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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


-- ============================================================
-- Migracion 05_evaluacion_docente.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
        ;THROW 51002,
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
        ;THROW 51002,
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


-- ============================================================
-- Migracion 06_experiencia_docente.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
        ;THROW 51002,
            'El docente no existe o se encuentra inactivo.',
            1;
    END;

    IF @FechaFin IS NOT NULL
       AND @FechaFin < @FechaInicio
    BEGIN
        ;THROW 51003,
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
        ;THROW 51002,
            'El docente no existe o se encuentra inactivo.',
            1;
    END;

    IF @FechaFin IS NOT NULL
       AND @FechaFin < @FechaInicio
    BEGIN
        ;THROW 51003,
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


-- ============================================================
-- Migracion 07_reconocimientos_docentes.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
        ;THROW 51002,
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
        ;THROW 51002,
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


-- ============================================================
-- Migracion 08_red_docente.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
            ;THROW 51002,
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
            ;THROW 51003,
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
            ;THROW 51004,
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

        ;THROW;
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
        ;THROW 51005,
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


-- ============================================================
-- Migracion 09_docente_departamento.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
        ;THROW 51004,
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
            ;THROW 51002,
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
            ;THROW 51003,
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
            ;THROW 51005,
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

        ;THROW;
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
        ;THROW 51004,
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
        ;THROW 51006,
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


-- ============================================================
-- Migracion 10_intereses_futuros.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
GO

IF OBJECT_ID('dbo.intereses_futuros', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.intereses_futuros
    (
        docente INT NOT NULL,
        termino_clave VARCHAR(30) NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_intereses_futuros_activo DEFAULT (1),
        CONSTRAINT PK_intereses_futuros
            PRIMARY KEY (docente, termino_clave),
        CONSTRAINT FK_intereses_futuros_docente
            FOREIGN KEY (docente) REFERENCES dbo.docente(cedula),
        CONSTRAINT FK_intereses_futuros_termino
            FOREIGN KEY (termino_clave) REFERENCES dbo.termino_clave(termino)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_intereses_futuros
AS
BEGIN
    SET NOCOUNT ON;
    SELECT docente AS Docente, termino_clave AS TerminoClave, activo AS Activo
    FROM dbo.intereses_futuros
    WHERE activo = 1
    ORDER BY docente, termino_clave;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_intereses_futuros
    @Docente INT,
    @TerminoClave VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT docente AS Docente, termino_clave AS TerminoClave, activo AS Activo
    FROM dbo.intereses_futuros
    WHERE docente = @Docente AND termino_clave = @TerminoClave AND activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_intereses_por_docente
    @Docente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT docente AS Docente, termino_clave AS TerminoClave, activo AS Activo
    FROM dbo.intereses_futuros
    WHERE docente = @Docente AND activo = 1
    ORDER BY termino_clave;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_intereses_futuros
    @Docente INT,
    @TerminoClave VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS
        (
            SELECT 1 FROM dbo.intereses_futuros WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @Docente AND termino_clave = @TerminoClave
              AND activo = 1
        )
        BEGIN
            ;THROW 51010, 'El interés futuro ya está registrado.', 1;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.intereses_futuros WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @Docente AND termino_clave = @TerminoClave
              AND activo = 0
        )
            UPDATE dbo.intereses_futuros SET activo = 1
            WHERE docente = @Docente AND termino_clave = @TerminoClave;
        ELSE
            INSERT INTO dbo.intereses_futuros (docente, termino_clave, activo)
            VALUES (@Docente, @TerminoClave, 1);

        COMMIT TRANSACTION;
        EXEC dbo.sp_consultar_intereses_futuros @Docente, @TerminoClave;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_intereses_futuros
    @Docente INT,
    @TerminoClave VARCHAR(30),
    @NuevoDocente INT,
    @NuevoTerminoClave VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.intereses_futuros WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @Docente AND termino_clave = @TerminoClave
              AND activo = 1
        )
        BEGIN
            ;THROW 51016, 'No existe un interés futuro activo para actualizar.', 1;
        END;

        IF @Docente = @NuevoDocente AND @TerminoClave = @NuevoTerminoClave
        BEGIN
            COMMIT TRANSACTION;
            EXEC dbo.sp_consultar_intereses_futuros @NuevoDocente, @NuevoTerminoClave;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.intereses_futuros WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @NuevoDocente
              AND termino_clave = @NuevoTerminoClave
              AND activo = 1
        )
        BEGIN
            ;THROW 51010, 'El interés futuro de destino ya está registrado.', 1;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.intereses_futuros WITH (UPDLOCK, HOLDLOCK)
            WHERE docente = @NuevoDocente
              AND termino_clave = @NuevoTerminoClave
              AND activo = 0
        )
        BEGIN
            UPDATE dbo.intereses_futuros
            SET activo = 0
            WHERE docente = @Docente AND termino_clave = @TerminoClave;

            UPDATE dbo.intereses_futuros
            SET activo = 1
            WHERE docente = @NuevoDocente
              AND termino_clave = @NuevoTerminoClave;
        END
        ELSE
        BEGIN
            UPDATE dbo.intereses_futuros
            SET docente = @NuevoDocente,
                termino_clave = @NuevoTerminoClave
            WHERE docente = @Docente AND termino_clave = @TerminoClave;
        END;

        COMMIT TRANSACTION;
        EXEC dbo.sp_consultar_intereses_futuros @NuevoDocente, @NuevoTerminoClave;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_intereses_futuros
    @Docente INT,
    @TerminoClave VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.intereses_futuros SET activo = 0
    WHERE docente = @Docente AND termino_clave = @TerminoClave AND activo = 1;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO


-- ============================================================
-- Migracion 11_estudio_ac.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
GO

IF OBJECT_ID('dbo.estudio_ac', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.estudio_ac
    (
        estudio INT NOT NULL,
        area_conocimiento VARCHAR(6) NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_estudio_ac_activo DEFAULT (1),
        CONSTRAINT PK_estudio_ac
            PRIMARY KEY (estudio, area_conocimiento),
        CONSTRAINT FK_estudio_ac_estudio
            FOREIGN KEY (estudio) REFERENCES dbo.estudios_realizados(id),
        CONSTRAINT FK_estudio_ac_area
            FOREIGN KEY (area_conocimiento) REFERENCES dbo.area_conocimiento(id)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_listar_estudio_ac
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudio AS Estudio, area_conocimiento AS AreaConocimiento, activo AS Activo
    FROM dbo.estudio_ac
    WHERE activo = 1
    ORDER BY estudio, area_conocimiento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_consultar_estudio_ac
    @Estudio INT,
    @AreaConocimiento VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudio AS Estudio, area_conocimiento AS AreaConocimiento, activo AS Activo
    FROM dbo.estudio_ac
    WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento AND activo = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_areas_por_estudio
    @Estudio INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT estudio AS Estudio, area_conocimiento AS AreaConocimiento, activo AS Activo
    FROM dbo.estudio_ac
    WHERE estudio = @Estudio AND activo = 1
    ORDER BY area_conocimiento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_crear_estudio_ac
    @Estudio INT,
    @AreaConocimiento VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS
        (
            SELECT 1 FROM dbo.estudio_ac WITH (UPDLOCK, HOLDLOCK)
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento
              AND activo = 1
        )
        BEGIN
            ;THROW 51011, 'El área ya está asociada al estudio.', 1;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.estudio_ac WITH (UPDLOCK, HOLDLOCK)
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento
              AND activo = 0
        )
            UPDATE dbo.estudio_ac SET activo = 1
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento;
        ELSE
            INSERT INTO dbo.estudio_ac (estudio, area_conocimiento, activo)
            VALUES (@Estudio, @AreaConocimiento, 1);

        COMMIT TRANSACTION;
        EXEC dbo.sp_consultar_estudio_ac @Estudio, @AreaConocimiento;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_actualizar_estudio_ac
    @Estudio INT,
    @AreaConocimiento VARCHAR(6),
    @NuevoEstudio INT,
    @NuevaAreaConocimiento VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.estudio_ac WITH (UPDLOCK, HOLDLOCK)
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento
              AND activo = 1
        )
        BEGIN
            ;THROW 51017, 'No existe una asociación activa para actualizar.', 1;
        END;

        IF @Estudio = @NuevoEstudio AND @AreaConocimiento = @NuevaAreaConocimiento
        BEGIN
            COMMIT TRANSACTION;
            EXEC dbo.sp_consultar_estudio_ac @NuevoEstudio, @NuevaAreaConocimiento;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.estudio_ac WITH (UPDLOCK, HOLDLOCK)
            WHERE estudio = @NuevoEstudio
              AND area_conocimiento = @NuevaAreaConocimiento
              AND activo = 1
        )
        BEGIN
            ;THROW 51011, 'El área de destino ya está asociada al estudio.', 1;
        END;

        IF EXISTS
        (
            SELECT 1 FROM dbo.estudio_ac WITH (UPDLOCK, HOLDLOCK)
            WHERE estudio = @NuevoEstudio
              AND area_conocimiento = @NuevaAreaConocimiento
              AND activo = 0
        )
        BEGIN
            UPDATE dbo.estudio_ac
            SET activo = 0
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento;

            UPDATE dbo.estudio_ac
            SET activo = 1
            WHERE estudio = @NuevoEstudio
              AND area_conocimiento = @NuevaAreaConocimiento;
        END
        ELSE
        BEGIN
            UPDATE dbo.estudio_ac
            SET estudio = @NuevoEstudio,
                area_conocimiento = @NuevaAreaConocimiento
            WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento;
        END;

        COMMIT TRANSACTION;
        EXEC dbo.sp_consultar_estudio_ac @NuevoEstudio, @NuevaAreaConocimiento;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        ;THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eliminar_estudio_ac
    @Estudio INT,
    @AreaConocimiento VARCHAR(6)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.estudio_ac SET activo = 0
    WHERE estudio = @Estudio AND area_conocimiento = @AreaConocimiento AND activo = 1;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO


-- ============================================================
-- Migracion 12_apoyo_profesoral.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
    BEGIN
        ;THROW 51012, 'El indicador de apoyo debe ser 0 o 1.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.apoyo_profesoral WHERE estudios = @Estudios AND activo = 1)
    BEGIN
        ;THROW 51013, 'El estudio ya tiene un apoyo profesoral registrado.', 1;
    END;

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
    BEGIN
        ;THROW 51012, 'El indicador de apoyo debe ser 0 o 1.', 1;
    END;

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


-- ============================================================
-- Migracion 13_beca.sql
-- ============================================================
-- Ejecutar en la base seleccionada en WebMSSQL.
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
    BEGIN
        ;THROW 51014, 'La fecha de fin no puede ser anterior a la fecha de inicio.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.beca WHERE estudios = @Estudios AND activo = 1)
    BEGIN
        ;THROW 51015, 'El estudio ya tiene una beca registrada.', 1;
    END;

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
    BEGIN
        ;THROW 51014, 'La fecha de fin no puede ser anterior a la fecha de inicio.', 1;
    END;

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


-- Verificacion: cada fila debe mostrar OK.
SELECT esperado.Nombre AS Procedimiento,
       CASE WHEN p.object_id IS NULL THEN N'FALTA' ELSE N'OK' END AS Estado
FROM (VALUES
        (N'sp_listar_docente'),
        (N'sp_consultar_docente'),
        (N'sp_crear_docente'),
        (N'sp_actualizar_docente'),
        (N'sp_eliminar_docente'),
        (N'sp_listar_estudios_realizados'),
        (N'sp_consultar_estudios_realizados'),
        (N'sp_estudios_por_docente'),
        (N'sp_crear_estudios_realizados'),
        (N'sp_actualizar_estudios_realizados'),
        (N'sp_eliminar_estudios_realizados'),
        (N'sp_listar_evaluacion_docente'),
        (N'sp_consultar_evaluacion_docente'),
        (N'sp_evaluaciones_por_docente'),
        (N'sp_crear_evaluacion_docente'),
        (N'sp_actualizar_evaluacion_docente'),
        (N'sp_eliminar_evaluacion_docente'),
        (N'sp_listar_experiencia_docente'),
        (N'sp_consultar_experiencia_docente'),
        (N'sp_experiencias_por_docente'),
        (N'sp_crear_experiencia_docente'),
        (N'sp_actualizar_experiencia_docente'),
        (N'sp_eliminar_experiencia_docente'),
        (N'sp_listar_reconocimiento'),
        (N'sp_consultar_reconocimiento'),
        (N'sp_reconocimientos_por_docente'),
        (N'sp_crear_reconocimiento'),
        (N'sp_actualizar_reconocimiento'),
        (N'sp_eliminar_reconocimiento'),
        (N'sp_listar_red_docente'),
        (N'sp_consultar_red_docente'),
        (N'sp_redes_por_docente'),
        (N'sp_crear_red_docente'),
        (N'sp_actualizar_red_docente'),
        (N'sp_eliminar_red_docente'),
        (N'sp_listar_docente_departamento'),
        (N'sp_consultar_docente_departamento'),
        (N'sp_departamentos_por_docente'),
        (N'sp_crear_docente_departamento'),
        (N'sp_actualizar_docente_departamento'),
        (N'sp_eliminar_docente_departamento'),
        (N'sp_listar_intereses_futuros'),
        (N'sp_consultar_intereses_futuros'),
        (N'sp_intereses_por_docente'),
        (N'sp_crear_intereses_futuros'),
        (N'sp_actualizar_intereses_futuros'),
        (N'sp_eliminar_intereses_futuros'),
        (N'sp_listar_estudio_ac'),
        (N'sp_consultar_estudio_ac'),
        (N'sp_areas_por_estudio'),
        (N'sp_crear_estudio_ac'),
        (N'sp_actualizar_estudio_ac'),
        (N'sp_eliminar_estudio_ac'),
        (N'sp_listar_apoyo_profesoral'),
        (N'sp_consultar_apoyo_profesoral'),
        (N'sp_crear_apoyo_profesoral'),
        (N'sp_actualizar_apoyo_profesoral'),
        (N'sp_eliminar_apoyo_profesoral'),
        (N'sp_listar_beca'),
        (N'sp_consultar_beca'),
        (N'sp_crear_beca'),
        (N'sp_actualizar_beca'),
        (N'sp_eliminar_beca')) AS esperado(Nombre)
LEFT JOIN sys.procedures AS p
    ON p.schema_id = SCHEMA_ID(N'dbo')
   AND p.name = esperado.Nombre
ORDER BY esperado.Nombre;
GO
