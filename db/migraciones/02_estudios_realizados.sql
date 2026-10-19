
USE gestion_local;
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
        THROW 51002, 'El docente seleccionado no existe o está retirado.', 1;
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
        THROW 51002, 'El docente seleccionado no existe o está retirado.', 1;
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
