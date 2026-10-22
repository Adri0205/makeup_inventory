# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# MANUAL TÉCNICO Y ARQUITECTURA DEL SISTEMA
### PROYECTO CÁTEDRA FASE II: SISTEMA WEB DE INVENTARIO Y TIENDA "GLOW & GLAMOUR"

---

## 1. DESCRIPCIÓN ARQUITECTÓNICA DEL SISTEMA

La aplicación ha sido desarrollada siguiendo una **Arquitectura en Capas (N-Tier Architecture)** basada en el patrón de diseño **MVC (Model-View-Controller)** y complementada con los patrones **Repository** y **Service Layer**, garantizando alta cohesión, bajo acoplamiento, mantenibilidad y escalabilidad empresarial.

```
+-----------------------------------------------------------------------+
|                    CAPA DE PRESENTACIÓN (FRONTEND)                    |
|   Vistas Razor (.cshtml) + Bootstrap 5 + Bootstrap Icons + JS Vanilla |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                   CAPA DE CONTROLADORES (CONTROLLERS)                 |
|   TiendaController | AsesorIAController | VentasController | Etc.    |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                  CAPA DE LÓGICA DE NEGOCIO (SERVICES)                 |
|   IAService | VentaService | ProductoService | CarritoService         |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                 CAPA DE ACCESO A DATOS (REPOSITORIES & ORM)           |
|   IRepository<T> | ProductoRepository | VentaRepository | EF Core     |
+-----------------------------------------------------------------------+
                                   |
                                   v
+-----------------------------------------------------------------------+
|                     CAPA DE PERSISTENCIA (DATABASE)                   |
|                 Microsoft SQL Server 2022 / LocalDB                   |
+-----------------------------------------------------------------------+
```

---

## 2. STACK TECNOLÓGICO Y VERSIONES UTILIZADAS

* **Framework Principal:** .NET 10.0 (ASP.NET Core Web App MVC).
* **Lenguaje de Programación:** C# versión 13 / 14 con características modernas (Pattern Matching, Records, Top-Level Statements, Nullable Reference Types).
* **Mapeo Objeto-Relacional (ORM):** Microsoft Entity Framework Core 10.0.13 (`Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`).
* **Base de Datos:** Microsoft SQL Server 2022 / SQL Server LocalDB.
* **Serialización y Sesiones:** `Microsoft.AspNetCore.Mvc.NewtonsoftJson`, `System.Text.Json`, `Microsoft.AspNetCore.Session` con memoria distribuida en caché (`AddDistributedMemoryCache`).
* **Contenerización:** Docker con Dockerfile multi-etapa y Docker Compose v3.8.
* **Frontend:** Razor Views, HTML5 semántico, CSS3, Bootstrap 5.3, Bootstrap Icons 1.11, Google Fonts (*Playfair Display* & *Plus Jakarta Sans*).

---

## 3. ESTRUCTURA DEL PROYECTO Y ORGANIZACIÓN DEL CÓDIGO

```
PROYECTO CATEDRA FASE II/
├── .env.example                     # Plantilla de variables de entorno seguras
├── .gitignore                       # Filtro de artefactos temporales y binarios
├── docker-compose.yml               # Orquestación de contenedores en la raíz
├── README.md                        # Documentación ejecutiva de presentación UDB
├── database/
│   └── MakeupInventoryDB.sql       # Script DDL/DML completo de base de datos
├── docker/
│   ├── .dockerignore
│   ├── .env.example
│   ├── Dockerfile                   # Construcción multi-stage de la imagen .NET
│   └── docker-compose.yml           # Definición de servicios (app + mssql)
├── docs/
│   ├── Evidencias_Pruebas.md        # Matriz y evidencia de casos de prueba
│   ├── Manual_Instalacion.md        # Guía de despliegue y puesta en marcha
│   ├── Manual_Tecnico.md            # Especificación de ingeniería y arquitectura
│   ├── Manual_Usuario.md            # Manual de operación para cliente y vendedora
│   └── Propuesta_Despliegue_Azure.md# Estrategia de despliegue en Microsoft Azure
└── src/
    └── MaquillajeInventory/
        ├── Controllers/             # Controladores MVC y Endpoints API REST
        │   ├── AdminController.cs
        │   ├── AsesorIAController.cs
        │   ├── CategoriasController.cs
        │   ├── HomeController.cs
        │   ├── InventarioController.cs
        │   ├── ProductosController.cs
        │   ├── TiendaController.cs
        │   └── VentasController.cs
        ├── Data/                    # Contexto EF Core e Inicializador de Datos
        │   ├── ApplicationDbContext.cs
        │   └── DbInitializer.cs
        ├── Models/                  # Entidades de dominio, ViewModels y DTOs
        │   ├── AsesorIAModel.cs
        │   ├── CarritoItem.cs
        │   ├── Categoria.cs
        │   ├── CheckoutViewModel.cs
        │   ├── DashboardViewModel.cs
        │   ├── DetalleVenta.cs
        │   ├── ErrorViewModel.cs
        │   ├── MovimientoInventario.cs
        │   ├── Producto.cs
        │   ├── Usuario.cs
        │   └── Venta.cs
        ├── Repositories/            # Patrón Repositorio genérico y especializado
        │   ├── IProductoRepository.cs
        │   ├── IRepository.cs
        │   ├── IVentaRepository.cs
        │   ├── ProductoRepository.cs
        │   ├── Repository.cs
        │   └── VentaRepository.cs
        ├── Services/                # Lógica de negocio y motor de Inteligencia Artificial
        │   ├── CarritoService.cs
        │   ├── CategoriaService.cs
        │   ├── IAService.cs
        │   ├── ICarritoService.cs
        │   ├── ICategoriaService.cs
        │   ├── IIAService.cs
        │   ├── IProductoService.cs
        │   ├── IVentaService.cs
        │   └── ProductoService.cs
        ├── Views/                   # Vistas Razor del portal cliente y administrativo
        ├── wwwroot/                 # Recursos estáticos (CSS, JS, librerías)
        ├── appsettings.json         # Configuración y cadenas de conexión
        ├── Dockerfile               # Dockerfile integrado en el proyecto
        ├── MaquillajeInventory.csproj
        └── Program.cs               # Punto de entrada y composición del contenedor IoC
```

---

## 4. MODELO DE DATOS ENTIDAD-RELACIÓN (DATABASE SCHEMA)

La persistencia de información en `MakeupInventoryDB` está conformada por 6 entidades relacionales fuertemente tipadas:

### 4.1 Entidades Principales
1. **`Categorias`:** Clasificación funcional de los productos (Labiales, Sombras, Bases, Rubores, Delineadores, Cuidado Facial).
2. **`Productos`:** Inventario de cosméticos con campos `Id`, `Nombre`, `Descripcion`, `Precio` (decimal 18,2), `Stock` (entero no negativo), `CategoriaId` (FK), `Proveedor`, `ImagenUrl` y `FechaIngreso`.
3. **`Usuarios`:** Credenciales y perfiles de acceso con campos `Id`, `Nombre`, `Email` (índice único), `PasswordHash` y `Rol` (Admin, Vendedor, Cliente).
4. **`Ventas`:** Cabecera de transacciones comerciales con campos `Id`, `Fecha`, `Total`, `MetodoPago`, `UsuarioId` (FK nullable), `ClienteNombre`, `ClienteEmail`, `ClienteTelefono`, `DireccionEntrega` y `Estado`.
5. **`DetalleVentas`:** Líneas individuales de cada venta con `Id`, `VentaId` (FK en cascada), `ProductoId` (FK restrict), `Cantidad`, `PrecioUnitario` y `Subtotal`.
6. **`MovimientoInventario`:** Kárdex de auditoría para trazabilidad de existencias con `Id`, `ProductoId` (FK), `Cantidad` (positiva para entradas, negativa para salidas), `Fecha`, `Motivo` y `UsuarioId` (FK).

### 4.2 Restricciones y Reglas de Integridad en `ApplicationDbContext.cs`
* Restricción de eliminación referencial (`DeleteBehavior.Restrict`) entre Categorías y Productos para proteger el histórico de inventario.
* Borrado en cascada (`DeleteBehavior.Cascade`) entre `Ventas` y `DetalleVentas`.
* Precisión fija monetaria en `decimal(18,2)` para evitar pérdidas de precisión en cálculos fiscales de IVA.

---

## 5. ESPECIFICACIÓN DEL MÓDULO DE INTELIGENCIA ARTIFICIAL (IA)

Con base en los lineamientos de la Investigación Aplicada II y la Rúbrica de Evaluación de la Fase II, el sistema integra el servicio `IAService` implementando un **motor híbrido de Inteligencia Artificial y Procesamiento de Lenguaje Natural (NLP)**:

### 5.1 BeautyAdvisor AI (Asesora Virtual para Clientes)
* **Datos de Entrada:**
  * Consulta en texto libre formulada por el cliente.
  * Metadatos de personalización: Tipo de Piel (Grasa, Seca, Mixta, Sensible), Ocasión (Diario, Noche, Fiesta, Boda) y Presupuesto Máximo ($).
  * Catálogo de productos disponibles con existencias en tiempo real desde EF Core.
* **Procesamiento Realizado:**
  1. *Tokenización y Normalización Semántica:* Limpieza y ponderación de términos clave de cosmetología (acabado mate, fijación, cobertura, hidratación).
  2. *Motor de Inferencia de Compatibilidad:* Asignación de puntajes (scoring) cruzando el tipo de piel y la ocasión contra la composición de cada cosmético en base de datos.
  3. *Generador Estructurado de Diagnósticos:* Redacción contextualizada de recomendaciones y consejos cosmetológicos expertos.
* **Resultados Generados:** Objeto `RecomendacionIAResponse` con diagnóstico en lenguaje natural, tip de aplicación profesional y lista de productos rankeados con enlace de compra directa.
* **Beneficio Aportado:** Incremento de la tasa de conversión en la tienda online, reducción del tiempo de decisión del comprador y asesoramiento dermatológico preventivo sin costo.

### 5.2 Smart Inventory Analytics (Diagnóstico Predictivo para Vendedoras)
* **Datos de Entrada:** Niveles actuales de stock en almacén e histórico de transacciones comerciales de los últimos 30 días.
* **Procesamiento Realizado:** Análisis de rotación de existencias, cálculo de días de inventario restante y detección de umbrales críticos de desabastecimiento.
* **Resultados Generados:** Reporte ejecutivo con alertas tempranas, órdenes de reposición sugeridas a proveedores específicos y oportunidades de venta.
* **Beneficio Aportado:** Prevención de roturas de stock, optimización del capital de trabajo y automatización de la gestión de compras a distribuidores.

---

## 6. CONFIGURACIÓN DE CONTENERIZACIÓN CON DOCKER

### 6.1 Optimización Multi-Etapa (Multi-Stage Build)
El archivo `docker/Dockerfile` utiliza un patrón de 4 etapas que minimiza el tamaño de la imagen final:
1. `base`: Imagen ligera `mcr.microsoft.com/dotnet/aspnet:10.0` (exclusivamente el runtime de producción).
2. `build`: Imagen completa `mcr.microsoft.com/dotnet/sdk:10.0` para restauración de paquetes y compilación de código fuente.
3. `publish`: Generación de binarios optimizados en modo `Release`.
4. `final`: Copia únicamente de los binarios compilados hacia la imagen base, descartando las herramientas del SDK para garantizar un contenedor ligero, rápido y seguro.

### 6.2 Orquestación con Docker Compose
El archivo `docker-compose.yml` orquesta:
* **Contenedor `makeup_sqlserver_db`:** Imagen `mcr.microsoft.com/mssql/server:2022-latest`, con persistencia en volumen `sqlserver_data` y mecanismo de `healthcheck` que verifica la disponibilidad del motor mediante `sqlcmd`.
* **Contenedor `makeup_web_app`:** Servicio web ASP.NET Core que depende del estado saludable (`condition: service_healthy`) del motor de base de datos antes de iniciar.
