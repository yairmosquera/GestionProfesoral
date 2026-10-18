
USE gestion_local;
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
            THROW 51001,
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
