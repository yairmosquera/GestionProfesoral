USE gestion_local;
GO

IF OBJECT_ID('red', 'U') IS NOT NULL DROP TABLE red;
IF OBJECT_ID('programa', 'U') IS NOT NULL DROP TABLE programa;
IF OBJECT_ID('linea_investigacion', 'U') IS NOT NULL DROP TABLE linea_investigacion;
IF OBJECT_ID('termino_clave', 'U') IS NOT NULL DROP TABLE termino_clave;
IF OBJECT_ID('area_conocimiento', 'U') IS NOT NULL DROP TABLE area_conocimiento;
GO

CREATE TABLE area_conocimiento (
    id VARCHAR(6) NOT NULL,
    gran_area VARCHAR(60) NOT NULL,
    area VARCHAR(60) NOT NULL,
    disciplina VARCHAR(150) NOT NULL,
    activo BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);

CREATE TABLE termino_clave (
    termino VARCHAR(30) NOT NULL,
    termino_ingles VARCHAR(30),
    activo BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (termino)
);

CREATE TABLE linea_investigacion (
    id INT IDENTITY(1,1) NOT NULL,
    nombre VARCHAR(45) NOT NULL,
    descripcion VARCHAR(256) NOT NULL,
    activo BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);

CREATE TABLE programa (
    id INT NOT NULL,
    nombre VARCHAR(60) NOT NULL,
    tipo VARCHAR(45) NOT NULL,
    nivel VARCHAR(45) NOT NULL,
    fecha_creacion VARCHAR(45) NOT NULL,
    fecha_cierre VARCHAR(45),
    numero_cohortes VARCHAR(45) NOT NULL,
    cant_graduados VARCHAR(45) NOT NULL,
    fecha_actualizacion VARCHAR(45) NOT NULL,
    ciudad VARCHAR(45) NOT NULL,
    facultad INT NOT NULL,
    activo BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (id)
);

CREATE TABLE red (
    idr INT NOT NULL,
    nombre VARCHAR(45) NOT NULL,
    url VARCHAR(45) NOT NULL,
    pais VARCHAR(45) NOT NULL,
    activo BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (idr)
);
GO