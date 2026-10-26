USE gestion_local;
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
            THROW 51011, 'El área ya está asociada al estudio.', 1;

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
        THROW;
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
            THROW 51017, 'No existe una asociación activa para actualizar.', 1;

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
            THROW 51011, 'El área de destino ya está asociada al estudio.', 1;

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
        THROW;
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
