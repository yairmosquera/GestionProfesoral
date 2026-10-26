USE gestion_local;
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
            THROW 51010, 'El interés futuro ya está registrado.', 1;

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
        THROW;
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
            THROW 51016, 'No existe un interés futuro activo para actualizar.', 1;

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
            THROW 51010, 'El interés futuro de destino ya está registrado.', 1;

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
        THROW;
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
