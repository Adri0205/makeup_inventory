-- ==============================================================================
-- UNIVERSIDAD DON BOSCO
-- PROYECTO CÁTEDRA FASE II: Sistema de Inventario y Tienda de Cosméticos "Glow & Glamour"
-- SCRIPT COMPLETO DE BASE DE DATOS: MakeupInventoryDB.sql
-- ==============================================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MakeupInventoryDB')
BEGIN
    CREATE DATABASE MakeupInventoryDB;
END
GO

USE MakeupInventoryDB;
GO

-- 1. Tabla: Categorías de Cosméticos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias')
BEGIN
    CREATE TABLE Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(255) NULL
    );
END
GO

-- 2. Tabla: Usuarios del Sistema (Administrador, Vendedora, Cliente)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Rol NVARCHAR(50) NOT NULL DEFAULT 'Vendedor'
    );
END
GO

-- 3. Tabla: Productos (Cosméticos, Stock y Enlaces a Imágenes)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Productos')
BEGIN
    CREATE TABLE Productos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(200) NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        Precio DECIMAL(18,2) NOT NULL CHECK (Precio > 0),
        Stock INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
        CategoriaId INT NOT NULL,
        Proveedor NVARCHAR(200) NULL,
        ImagenUrl NVARCHAR(500) NULL,
        FechaIngreso DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id) ON DELETE NO ACTION
    );
END
GO

-- 4. Tabla: Ventas (Transacciones Comerciales de Tienda Online y Mostrador)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ventas')
BEGIN
    CREATE TABLE Ventas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Total DECIMAL(18,2) NOT NULL CHECK (Total > 0),
        MetodoPago NVARCHAR(50) NOT NULL,
        UsuarioId INT NULL,
        ClienteNombre NVARCHAR(150) NULL,
        ClienteEmail NVARCHAR(150) NULL,
        ClienteTelefono NVARCHAR(50) NULL,
        DireccionEntrega NVARCHAR(250) NULL,
        Estado NVARCHAR(50) NOT NULL DEFAULT 'Completada',
        CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE SET NULL
    );
END
GO

-- 5. Tabla: DetalleVentas (Líneas de Facturación)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetalleVentas')
BEGIN
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
END
GO

-- 6. Tabla: MovimientoInventario (Kárdex y Auditoría de Stock)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MovimientoInventario')
BEGIN
    CREATE TABLE MovimientoInventario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductoId INT NOT NULL,
        Cantidad INT NOT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Motivo NVARCHAR(150) NOT NULL,
        UsuarioId INT NULL,
        CONSTRAINT FK_Movimiento_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Movimiento_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE SET NULL
    );
END
GO

-- ==============================================================================
-- INSERCIÓN DE DATOS SEMILLA (SEED DATA)
-- ==============================================================================

-- Categorías
IF NOT EXISTS (SELECT 1 FROM Categorias)
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion) VALUES 
    ('Labiales', 'Labiales mate, cremosos, tintas y brillos hidratantes'),
    ('Sombras', 'Paletas de sombras nude, coloridas y glitter de alta pigmentación'),
    ('Bases y Correctores', 'Bases líquidas, correctores y polvos matificantes'),
    ('Rubor e Iluminador', 'Rubores en crema, polvo compacto e iluminadores satinados'),
    ('Delineadores y Máscaras', 'Delineadores a prueba de agua y máscaras para pestañas'),
    ('Cuidado Facial', 'Fijadores de maquillaje, sueros hidratantes y primers');
END
GO

-- Usuarios Iniciales
IF NOT EXISTS (SELECT 1 FROM Usuarios)
BEGIN
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol) VALUES 
    ('Administradora General', 'admin@makeup.com', 'AQAAAAIAAYagAAAAEAdminHash123==', 'Admin'),
    ('Vendedora Principal', 'vendedora@makeup.com', 'AQAAAAIAAYagAAAAEVendedoraHash123==', 'Vendedor'),
    ('Cliente Frecuente', 'cliente@makeup.com', 'AQAAAAIAAYagAAAAEClienteHash123==', 'Cliente');
END
GO

-- Productos Iniciales con Fotografía
IF NOT EXISTS (SELECT 1 FROM Productos)
BEGIN
    DECLARE @catLab INT = (SELECT TOP 1 Id FROM Categorias WHERE Nombre = 'Labiales');
    DECLARE @catSom INT = (SELECT TOP 1 Id FROM Categorias WHERE Nombre = 'Sombras');
    DECLARE @catBas INT = (SELECT TOP 1 Id FROM Categorias WHERE Nombre LIKE 'Bases%');
    DECLARE @catRub INT = (SELECT TOP 1 Id FROM Categorias WHERE Nombre LIKE 'Rubor%');
    DECLARE @catDel INT = (SELECT TOP 1 Id FROM Categorias WHERE Nombre LIKE 'Delineador%');

    INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, CategoriaId, Proveedor, ImagenUrl, FechaIngreso) VALUES 
    ('Labial Rojo Pasión Mate', 'Labial líquido de larga duración con acabado terciopelo mate indeleble. Ideal para piel mixta o grasa en eventos nocturnos.', 8.50, 25, @catLab, 'Maybelline New York', 'https://images.unsplash.com/photo-1586495777744-4413f21062fa?auto=format&fit=crop&w=600&q=80', GETDATE()),
    ('Labial Nude Hidratante Rosa', 'Fórmula enriquecida con manteca de karité y vitamina E. Tono rosa suave ideal para maquillaje diario y piel seca.', 7.00, 30, @catLab, 'L''Oréal Paris', 'https://images.unsplash.com/photo-1625093742435-6fa192b6fb10?auto=format&fit=crop&w=600&q=80', GETDATE()),
    ('Paleta de Sombras Nude Glam', 'Paleta con 16 tonos neutros, cálidos y satinados de ultra alta pigmentación para looks tanto de oficina como de noche.', 15.99, 15, @catSom, 'NYX Professional', 'https://images.unsplash.com/photo-1512496015851-a90fb38ba796?auto=format&fit=crop&w=600&q=80', GETDATE()),
    ('Base Líquida Control Grasa Tono Medio', 'Base unificadora con niacinamida y acabado semimate. Controla el sebo y minimiza poros hasta por 16 horas.', 12.75, 20, @catBas, 'L''Oréal Paris', 'https://images.unsplash.com/photo-1631729371254-42c2892f0e6e?auto=format&fit=crop&w=600&q=80', GETDATE()),
    ('Rubor Compacto Tono Coral Glow', 'Rubor sedoso micropulverizado que brinda un resplandor saludable y natural en pómulos con sutil reflejo dorado.', 9.25, 4, @catRub, 'NYX Professional', 'https://images.unsplash.com/photo-1522337360788-8b13dee7a37e?auto=format&fit=crop&w=600&q=80', GETDATE()),
    ('Delineador Plumón Waterproof Negro Intenso', 'Punta fina de precisión milimétrica resistente al agua, sudor y transferencias durante todo el día.', 5.50, 40, @catDel, 'Maybelline New York', 'https://images.unsplash.com/photo-1631214524020-7e18db9a8f92?auto=format&fit=crop&w=600&q=80', GETDATE());
END
GO

PRINT 'Base de datos MakeupInventoryDB configurada y sembrada exitosamente.';
GO
