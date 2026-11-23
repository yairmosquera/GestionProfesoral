USE gestion_local;
GO

/*
  GESTIÓN PROFESORAL - V3
  Migración 14: Usuarios, roles y asignación de roles
  - No modifica tablas de v1 ni v2.
  - No crea usuarios ni contraseñas predeterminadas.
  - La contraseña se guardará como hash seguro generado por .NET.
  - Reejecutable: verifica la existencia de cada objeto.
*/

IF OBJECT_ID(N'dbo.roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.roles
    (
        id INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_roles PRIMARY KEY,
        codigo VARCHAR(30) NOT NULL,
        nombre NVARCHAR(80) NOT NULL,
        descripcion NVARCHAR(250) NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_roles_activo DEFAULT (1),
        CONSTRAINT UQ_roles_codigo UNIQUE (codigo)
    );
END;
GO

IF OBJECT_ID(N'dbo.usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.usuarios
    (
        id INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_usuarios PRIMARY KEY,
        nombre_usuario NVARCHAR(80) NOT NULL,
        correo NVARCHAR(150) NOT NULL,
        nombre_completo NVARCHAR(160) NOT NULL,
        password_hash NVARCHAR(512) NOT NULL,
        activo BIT NOT NULL
            CONSTRAINT DF_usuarios_activo DEFAULT (1),
        fecha_creacion DATETIME2(0) NOT NULL
            CONSTRAINT DF_usuarios_fecha_creacion
            DEFAULT (SYSUTCDATETIME()),
        ultimo_acceso DATETIME2(0) NULL,
        CONSTRAINT UQ_usuarios_nombre_usuario
            UNIQUE (nombre_usuario),
        CONSTRAINT UQ_usuarios_correo
            UNIQUE (correo),
        CONSTRAINT CK_usuarios_nombre_usuario
            CHECK (LEN(LTRIM(RTRIM(nombre_usuario))) > 0),
        CONSTRAINT CK_usuarios_password_hash
            CHECK (LEN(password_hash) >= 40)
    );
END;
GO

IF OBJECT_ID(N'dbo.usuario_roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.usuario_roles
    (
        usuario_id INT NOT NULL,
        rol_id INT NOT NULL,
        fecha_asignacion DATETIME2(0) NOT NULL
            CONSTRAINT DF_usuario_roles_fecha_asignacion
            DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_usuario_roles
            PRIMARY KEY (usuario_id, rol_id),
        CONSTRAINT FK_usuario_roles_usuarios
            FOREIGN KEY (usuario_id) REFERENCES dbo.usuarios(id),
        CONSTRAINT FK_usuario_roles_roles
            FOREIGN KEY (rol_id) REFERENCES dbo.roles(id)
    );
END;
GO

-- Los códigos son estables y sirven para autorización.
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE codigo = 'ADMIN')
BEGIN
    INSERT INTO dbo.roles (codigo, nombre, descripcion)
    VALUES ('ADMIN', N'Administrador',
            N'Administra usuarios, roles y recursos del sistema.');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE codigo = 'GESTOR')
BEGIN
    INSERT INTO dbo.roles (codigo, nombre, descripcion)
    VALUES ('GESTOR', N'Gestor académico',
            N'Gestiona la información profesoral.');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE codigo = 'CONSULTA')
BEGIN
    INSERT INTO dbo.roles (codigo, nombre, descripcion)
    VALUES ('CONSULTA', N'Consulta',
            N'Visualiza información autorizada sin editar.');
END;
GO

-- Comprobación: no se imprimen hashes ni datos sensibles.
SELECT id, codigo, nombre, activo
FROM dbo.roles
ORDER BY id;

SELECT
    OBJECT_ID(N'dbo.usuarios', N'U') AS tabla_usuarios,
    OBJECT_ID(N'dbo.roles', N'U') AS tabla_roles,
    OBJECT_ID(N'dbo.usuario_roles', N'U') AS tabla_usuario_roles;
GO
