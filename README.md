# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# PROYECTO CÁTEDRA FASE II
## "GLOW & GLAMOUR MAKEUP STUDIO"
### SISTEMA INTEGRAL DE GESTIÓN DE INVENTARIO & TIENDA WEB CON INTELIGENCIA ARTIFICIAL

---

## 👥 DATOS GENERALES DEL PROYECTO
* **Institución:** Universidad Don Bosco (UDB), El Salvador.
* **Plataforma Tecnológica Principal:** C# .NET 10, ASP.NET Core MVC, Entity Framework Core 10.
* **Motor de Base de Datos:** Microsoft SQL Server 2022 / LocalDB.
* **Entorno de Contenerización:** Docker & Docker Compose.
* **Entrega:** Fase II - Prototipo Funcional, Integración de IA y Documentación Técnica.

---

## 📌 DESCRIPCIÓN Y JUSTIFICACIÓN DEL PROYECTO

En el sector de cosméticos y cuidado personal, las pequeñas y medianas empresas enfrentan dos desafíos críticos:
1. **Pérdida de ventas por falta de asesoría especializada:** Los clientes online suelen abandonar sus compras al desconocer qué formulaciones, tonos de base o labiales son compatibles con su tipo de piel (grasa, seca, mixta o sensible) y para la ocasión de uso (diario, noche, eventos).
2. **Descontrol de almacén y quiebres de stock imprevistos:** Las vendedoras y administradoras carecen de herramientas en tiempo real para rastrear entradas de proveedores, ventas presenciales de mostrador y ventas en línea bajo un kárdex centralizado y un motor predictivo que anticipe pedidos de reposición.

Para resolver esta problemática de forma integral, se desarrolló **Glow & Glamour**, una plataforma modular que incorpora:
* **Un Portal E-Commerce para el Cliente:** Con catálogo visual de alta fidelidad, filtros por categoría cosmética, carrito de compras con control de existencias, checkout con facturación electrónica y rastreo histórico de compras.
* **BeautyAdvisor AI (Asesora Virtual de Maquillaje):** Un módulo de Inteligencia Artificial que analiza consultas en lenguaje natural, diagnostica compatibilidad con el tipo de piel y sugiere productos del catálogo en tiempo real con consejos profesionales de aplicación.
* **Un Portal Administrativo para la Vendedora:** Con panel de control gerencial (KPIs), punto de venta (POS) para atención física en caja, mantenimiento CRUD completo de productos y categorías, trazabilidad de movimientos de inventario y diagnóstico algorítmico de stock.

---

## 📋 MATRIZ DE CUMPLIMIENTO DE PRODUCTOS ESPERADOS (FASE II)

| N° | Producto Esperado (Rúbrica UDB) | Estado | Evidencia de Implementación |
| :---: | :--- | :---: | :--- |
| **1** | **Aplicación Web Funcional** | **CUMPLIDO AL 100%** | Interfaces responsivas completas con Bootstrap 5, navegación entre módulos, formularios validados con Data Annotations, catálogo para clientes, carrito de compras, checkout con emisión de comprobante y gestión CRUD. |
| **2** | **Backend Operativo en ASP.NET Core** | **CUMPLIDO AL 100%** | Arquitectura en capas limpia (`Models`, `Data`, `Repositories`, `Services`, `Controllers`), ORM Entity Framework Core 10, inyección de dependencias (IoC), manejo transaccional ACID y control global de excepciones. |
| **3** | **Integración de Inteligencia Artificial** | **CUMPLIDO AL 100%** | Implementación de **BeautyAdvisor AI** (recomendador semántico para clientes) y **Smart Inventory Analytics** (reporte predictivo de reposición para vendedoras) vía `IAService.cs` y `AsesorIAController.cs`. |
| **4** | **Docker y Despliegue en la Nube** | **CUMPLIDO AL 100%** | `Dockerfile` multi-stage optimizado, `docker-compose.yml` orquestando la app y SQL Server 2022, archivo `.env.example` y documento formal de propuesta de arquitectura en Microsoft Azure (`docs/Propuesta_Despliegue_Azure.md`). |
| **5** | **Integración General del Sistema** | **CUMPLIDO AL 100%** | Comunicación fluida y desacoplada entre Frontend, Backend, Servicios de Negocio, EF Core, Motor de IA y Base de Datos SQL Server. |
| **6** | **Pruebas de Funcionamiento** | **CUMPLIDO AL 100%** | Matriz de 13 casos de prueba funcionales documentada en `docs/Evidencias_Pruebas.md`, abarcando flujos de cliente, vendedora, IA y contenedores. |
| **7** | **Documentación Técnica y Manuales** | **CUMPLIDO AL 100%** | `docs/Manual_Instalacion.md`, `docs/Manual_Usuario.md` (con sección detallada de cliente y vendedora) y `docs/Manual_Tecnico.md`. |
| **8** | **Video de Evidencia** | *EXCLUIDO* | Excluido formalmente a solicitud expresa del usuario en el alcance actual. |
| **9** | **Repositorio del Proyecto** | **CUMPLIDO AL 100%** | Estructura organizada requerida: `/src`, `/docs`, `/database`, `/docker`, `.env.example`, `.gitignore` y código fuente documentado. |

---

## 🏛️ ARQUITECTURA DEL SISTEMA Y CAPAS

La aplicación sigue los principios de diseño de software sólido (SOLID) con separación de responsabilidades:

```
+---------------------------------------------------------------------------------+
|                        CAPA DE PRESENTACIÓN (CLIENTE & VENDEDORA)               |
|  - Tienda: Catálogo, Ficha de Cosméticos, Carrito, Checkout, Mis Pedidos        |
|  - Inteligencia Artificial: Asesora Virtual BeautyAdvisor y Auditoría de Stock  |
|  - Administración: Dashboard KPI, Punto de Venta (POS), CRUDs, Kárdex Almacén   |
+---------------------------------------------------------------------------------+
                                       |
                                       v
+---------------------------------------------------------------------------------+
|                        CAPA DE CONTROLADORES (CONTROLLERS)                      |
|  TiendaController | AsesorIAController | AdminController | VentasController     |
|  ProductosController | CategoriasController | InventarioController              |
+---------------------------------------------------------------------------------+
                                       |
                                       v
+---------------------------------------------------------------------------------+
|                        CAPA DE LÓGICA DE NEGOCIO (SERVICES)                     |
|  IAService | VentaService | ProductoService | CategoriaService | CarritoService |
+---------------------------------------------------------------------------------+
                                       |
                                       v
+---------------------------------------------------------------------------------+
|                    CAPA DE ACCESO A DATOS (REPOSITORIES & ORM)                  |
|  IRepository<T> | ProductoRepository | VentaRepository | ApplicationDbContext   |
+---------------------------------------------------------------------------------+
                                       |
                                       v
+---------------------------------------------------------------------------------+
|                            PERSISTENCIA (DATABASE ENGINE)                       |
|                       Microsoft SQL Server 2022 / LocalDB                       |
+---------------------------------------------------------------------------------+
```

---

## 🤖 FUNCIONALIDAD DE INTELIGENCIA ARTIFICIAL (EVIDENCIA)

* **Datos de Entrada:** Consulta en lenguaje natural del usuario, tipo de piel (Grasa, Seca, Mixta, Sensible, Normal), ocasión (Diario, Noche, Fiesta, Boda) y presupuesto máximo ($ USD).
* **Procesamiento Realizado:** Análisis semántico de intenciones cosméticas, compatibilidad de formulación química/dermatológica contra el catálogo de productos disponibles en base de datos y cálculo de ranking de idoneidad.
* **Resultados Generados:** Diagnóstico personalizado, consejos de aplicación técnica (*beauty tips*) y tarjetas de productos recomendados listos para añadir al carrito.
* **Beneficio Aportado:** Reducción de la fricción de compra para el cliente y automatización de la toma de decisiones para la reposición de mercancía en el almacén.

---

## 🚀 GUÍA DE PUESTA EN MARCHA RÁPIDA

### Opción 1: Ejecución Local (.NET SDK 10)
```bash
# 1. Navegar a la carpeta del proyecto
cd src/MaquillajeInventory

# 2. Restaurar dependencias y compilar
dotnet restore
dotnet build

# 3. Iniciar la aplicación web
dotnet run
```
*Abrir navegador en la URL indicada en la consola (ej. `http://localhost:5000`). La base de datos y los datos semilla de cosméticos se configuran automáticamente en el primer inicio.*

### Opción 2: Ejecución Multi-Contenedor (Docker Compose)
```bash
# 1. Copiar variables de entorno
copy .env.example .env

# 2. Construir y levantar contenedores
docker compose up -d --build
```
*Acceder a la aplicación contenerizada en: `http://localhost:8080`.*

---

## 📁 ESTRUCTURA DEL REPOSITORIO INSTITUCIONAL

```
PROYECTO CATEDRA FASE II/
├── .env.example                     # Variables de entorno modelo
├── .gitignore                       # Ignorados por Git (bin, obj, .vs)
├── docker-compose.yml               # Orquestación raíz Docker Compose
├── README.md                        # Documento principal del proyecto
├── database/
│   └── MakeupInventoryDB.sql       # Script SQL DDL y DML de base de datos
├── docker/
│   ├── .dockerignore
│   ├── .env.example
│   ├── Dockerfile                   # Dockerfile multi-etapa
│   └── docker-compose.yml           # Orquestación de servicios Docker
├── docs/
│   ├── Evidencias_Pruebas.md        # Matriz formal de pruebas funcionales
│   ├── Manual_Instalacion.md        # Manual de instalación y dependencias
│   ├── Manual_Tecnico.md            # Manual de arquitectura e ingeniería
│   ├── Manual_Usuario.md            # Manual de usuario (Cliente y Vendedora)
│   └── Propuesta_Despliegue_Azure.md# Propuesta y costos en Microsoft Azure
└── src/
    └── MaquillajeInventory/         # Código fuente de la solución en ASP.NET Core
```

---

## 🎓 CRÉDITOS Y DERECHOS ACADÉMICOS
Proyecto desarrollado para la evaluación de la **Fase II**, **Universidad Don Bosco**, 2026.
