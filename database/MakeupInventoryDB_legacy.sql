
CREATE DATABASE MakeupInventoryDB;
GO

USE MakeupInventoryDB;
GO

--Crear la tabla Categorias
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL
);
GO

-- Crear la tabla Usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Rol NVARCHAR(50) NOT NULL DEFAULT 'Vendedor'
);
GO

-- Crear la tabla Productos
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio DECIMAL(18,2) NOT NULL CHECK (Precio > 0),
    Stock INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    CategoriaId INT NOT NULL,
    Proveedor NVARCHAR(200) NULL,
    FechaIngreso DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id) ON DELETE NO ACTION
);
GO

--  Crear la tabla Ventas
CREATE TABLE Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(18,2) NOT NULL CHECK (Total > 0),
    MetodoPago NVARCHAR(50) NOT NULL,
    UsuarioId INT NULL,
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE SET NULL
);
GO

-- Crear la tabla DetalleVentas
CREATE TABLE DetalleVentas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18,2) NOT NULL CHECK (PrecioUnitario > 0),
    Subtotal DECIMAL(18,2) NOT NULL CHECK (Subtotal > 0),
    CONSTRAINT FK_DetalleVentas_Ventas FOREIGN KEY (VentaId) REFERENCES Ventas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_DetalleVentas_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id) ON DELETE NO ACTION
);
GO

--  Crear la tabla MovimientoInventario
CREATE TABLE MovimientoInventario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Motivo NVARCHAR(50) NOT NULL,
    UsuarioId INT NULL,
    CONSTRAINT FK_Movimiento_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Movimiento_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE SET NULL
);
GO

-- Insertar Datos Semilla (Seed Data)
INSERT INTO Categorias (Nombre, Descripcion) VALUES 
('Labiales', 'Labiales de todo tipo'),
('Sombras', 'Paletas y sombras individuales'),
('Bases', 'Bases y correctores'),
('Rubor', 'Rubores e iluminadores'),
('Delineadores', 'Delineadores de ojos y labios');

INSERT INTO Productos (Nombre, Precio, Stock, CategoriaId, Proveedor) VALUES 
('Labial Rojo Mate', 8.50, 25, 1, 'Maybelline'),
('Labial Rosa Clásico', 7.00, 30, 1, 'Maybelline'),
('Paleta de Sombras Nude', 15.99, 15, 2, 'NYX'),
('Base Líquida Tono Medio', 12.75, 20, 3, 'L''Oréal'),
('Rubor Coral', 9.25, 18, 4, 'NYX'),
('Delineador Negro', 5.50, 40, 5, 'Maybelline');

INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol) VALUES 
('Administrador', 'admin@makeup.com', 'TEMP_HASH', 'Admin');
GO