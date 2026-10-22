# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# MANUAL DE USUARIO
### PROYECTO CÁTEDRA FASE II: SISTEMA INTEGRAL "GLOW & GLAMOUR MAKEUP STUDIO"
#### MÓDULO DEL CLIENTE (E-COMMERCE & ASESORÍA IA) Y MÓDULO DE LA VENDEDORA (INVENTARIO & FACTURACIÓN)

---

## 1. INTRODUCCIÓN GENERAL DEL SISTEMA

**Glow & Glamour Makeup Studio** es una solución web empresarial desarrollada bajo plataforma ASP.NET Core y Entity Framework Core, diseñada para cubrir de forma integral el ciclo comercial de un negocio cosmético. 

El sistema cuenta con una arquitectura de doble cara (B2C & B2B/Gestión Interna):
1. **Portal de Cara al Cliente (Tienda Online & Asesoría IA):** Permite a los compradores explorar cosméticos, filtrar por tipo de producto, recibir asesoramiento dermatológico y estético personalizado mediante Inteligencia Artificial, gestionar su carrito de compras y registrar órdenes con emisión de comprobante digital.
2. **Portal de Cara a la Vendedora / Administración:** Proporciona un centro de mando operativo con métricas de ventas en tiempo real, gestión de existencias de almacén (CRUD de productos y categorías), punto de venta presencial (POS), auditoría mediante kárdex de movimientos y reportes predictivos de desabastecimiento con IA.

---

## 2. NAVEGACIÓN Y SELECTOR DE VISTAS (MODALIDADES DE USO)

En la barra de navegación superior (Navbar), los usuarios pueden interactuar de forma intuitiva entre los dos perfiles del sistema:
* **Área Cliente:** Enlaces directos a *Catálogo Cosmético*, *Asesora Virtual IA*, *Mis Pedidos* y el *Carrito de Compras* con contador reactivo de artículos.
* **Área Vendedora:** Menú desplegable *Panel Vendedora* con acceso a Dashboard, Punto de Venta, Historial de Ventas, Kárdex y Diagnóstico IA, así como un botón destacado con degradado `"Modo Vendedora"` para alternar con un solo clic.

---

## 3. GUÍA DE USO: MÓDULO DEL CLIENTE

### 3.1 Catálogo de Productos y Búsqueda Inteligente
* **Ubicación:** Menú principal &rarr; `Catálogo Cosmético` (Ruta: `/Tienda/Index`).
* **Píldoras de Categoría:** En la parte superior del catálogo se encuentran filtros rápidos redondeados para consultar rubros como: *Labiales*, *Sombras*, *Bases y Correctores*, *Rubor e Iluminador*, *Delineadores y Máscaras*.
* **Búsqueda y Ordenamiento:** La barra de búsqueda permite ingresar nombres de cosméticos, ingredientes o marcas. Además, el selector de orden permite clasificar por menor precio, mayor precio o alfabéticamente.
* **Tarjetas de Producto:** Cada tarjeta expone la imagen en alta resolución del cosmético, la marca proveedora, precio en dólares americanos, badge de categoría y el estado de stock (alertando en color naranja si quedan 5 o menos unidades).

### 3.2 Ficha Técnica y Detalle del Cosmético
* **Ubicación:** Clic en `Ver Detalles` en cualquier producto (Ruta: `/Tienda/Detalle/{id}`).
* **Funcionalidad:** Muestra la fotografía ampliada, descripción dermatológica, tono, modo de aplicación sugerido y selector de cantidad con límite validado según el stock físico disponible en bodega.
* **Sección de Recomendación Complementaria:** Incluye productos afines de la misma categoría para compras cruzadas (cross-selling).

### 3.3 Carrito de Compras (Shopping Cart)
* **Ubicación:** Icono de Carrito en la esquina superior derecha (Ruta: `/Tienda/Carrito`).
* **Operaciones Disponibles:**
  * Incrementar o decrementar unidades con cálculo automático de subtotales.
  * Eliminar cosméticos individuales del pedido.
  * Visualización del desglose financiero: Subtotal neto, IVA (13% correspondiente a la legislación salvadoreña) y Total Final.
  * Clic en `Proceder al Checkout` para iniciar la finalización de compra.

### 3.4 Proceso de Checkout y Emisión de Factura Digital
* **Ubicación:** Ruta `/Tienda/Checkout`.
* **Formulario de Facturación:** El cliente ingresa su Nombre Completo, Correo Electrónico (donde recibirá el comprobante), Teléfono de Contacto y Dirección de Entrega.
* **Forma de Pago:** Selección entre Tarjeta de Crédito/Débito, Transferencia Bancaria / QR Chivo Wallet o Efectivo contra entrega.
* **Confirmación Inmediata (Ticket Digital):** Al hacer clic en `Confirmar y Realizar Pedido`, el sistema:
  1. Valida y descuenta automáticamente las unidades del inventario en base de datos.
  2. Genera un registro inmutable en la tabla `MovimientoInventario` con concepto de salida por venta web.
  3. Despliega la pantalla de éxito (`/Tienda/PedidoConfirmado/{id}`) con el número de orden oficial, detalle de cada ítem, impuestos desglosados y botón para **Imprimir Comprobante**.

### 3.5 Rastreo de Compras Anteriores ("Mis Pedidos")
* **Ubicación:** Menú `Mis Pedidos` (Ruta: `/Tienda/MisPedidos`).
* **Uso:** El cliente escribe su correo electrónico registrado y el sistema consulta y despliega de inmediato todas las órdenes históricas asociadas, mostrando su estado, fecha, monto total cancelado y enlace a la factura digital.

### 3.6 Asesora Virtual de Maquillaje (BeautyAdvisor AI)
* **Ubicación:** Menú `Asesora Virtual IA` (Ruta: `/AsesorIA/Index`).
* **Propósito:** Asistente inteligente que orienta a clientes indecisos o con necesidades específicas de cosmetología.
* **Campos de Entrada:**
  * **Tipo de Piel:** Grasa, Seca, Mixta, Sensible o Normal.
  * **Ocasión:** Diario/Oficina, Fiesta/Noche, Boda/Gala, Casual.
  * **Presupuesto Máximo:** Límite económico en dólares.
  * **Consulta en Lenguaje Natural:** Campo abierto donde la usuaria describe sus dudas (ej: *"Busco un labial llamativo de noche pero que no reseque mis labios"*).
  * **Consultas Rápidas:** Botones preconfigurados de un toque para looks típicos.
* **Procesamiento y Resultados Generados:**
  1. La IA evalúa la consulta mediante procesamiento semántico de lenguaje natural (NLP).
  2. Genera un análisis técnico explicando el motivo de las recomendaciones para ese tipo de piel.
  3. Brinda un **"Consejo Cosmetológico Experto"** (técnicas de preparación de rostro y fijación).
  4. Presenta las tarjetas de los cosméticos en stock que mejor resuelven la necesidad, con botón instantáneo `Añadir al Carrito`.

---

## 4. GUÍA DE USO: MÓDULO DE LA VENDEDORA Y ADMINISTRACIÓN

### 4.1 Dashboard Ejecutivo de Negocio
* **Ubicación:** `Panel Vendedora` &rarr; `Dashboard General` (Ruta: `/Admin/Index`).
* **Métricas Principales (KPIs):**
  * **Total de Productos:** Cantidad de referencias registradas.
  * **Ingresos Totales ($):** Facturación acumulada del negocio.
  * **Ventas Realizadas:** Número global de transacciones.
  * **Productos en Stock Crítico:** Cantidad de cosméticos con 5 o menos unidades.
* **Tablas de Monitoreo:** Resumen rápido de las últimas ventas efectuadas y tabla de productos con riesgo de agotamiento con enlace directo para solicitar reposición a proveedores.

### 4.2 Punto de Venta en Mostrador (POS)
* **Ubicación:** `Panel Vendedora` &rarr; `Punto de Venta (POS)` (Ruta: `/Ventas/Nueva`).
* **Uso Operativo:**
  1. Diseñado para atención presencial rápida a clientes en tienda física.
  2. La vendedora visualiza el catálogo de productos con sus precios y stock disponible en tiempo real.
  3. Ingresa las cantidades vendidas por fila y el sistema actualiza automáticamente el total a cobrar de forma dinámica mediante JavaScript.
  4. Ingresa el nombre del cliente y método de pago (Efectivo, Tarjeta POS, Transferencia).
  5. Al hacer clic en `Cobrar y Finalizar Venta`, se descarga el inventario y se abre la factura oficial de mostrador para impresión.

### 4.3 Control de Inventario de Productos (CRUD Completo)
* **Ubicación:** `Panel Vendedora` &rarr; `Gestión de Productos` (Ruta: `/Productos/Index`).
* **Operaciones:**
  * **Crear Producto (`/Productos/Create`):** Formulario con validaciones para Nombre, Categoría, Proveedor, Precio unitario, Stock inicial, URL de fotografía y Descripción detallada.
  * **Editar Producto (`/Productos/Edit/{id}`):** Modificación de precios, existencias o datos comerciales.
  * **Consultar Ficha (`/Productos/Details/{id}`):** Vista técnica del producto con métricas de entrada y enlace para previsualizarlo como cliente.
  * **Eliminar Producto (`/Productos/Delete/{id}`):** Baja de inventario protegida por confirmación.

### 4.4 Gestión de Categorías
* **Ubicación:** `Panel Vendedora` &rarr; `Categorías` (Ruta: `/Categorias/Index`).
* **Operaciones:** Creación, edición y eliminación de rubros de cosméticos, con validación de integridad referencial para evitar inconsistencias de datos.

### 4.5 Kárdex de Movimientos de Inventario
* **Ubicación:** `Panel Vendedora` &rarr; `Kárdex de Movimientos` (Ruta: `/Inventario/Movimientos`).
* **Auditoría:** Registro cronológico de todas las entradas y salidas de existencias. Muestra fecha y hora exacta, producto afectado, impacto positivo (+Entrada) o negativo (-Salida), motivo de la transacción y usuario responsable.

### 4.6 Reabastecimiento de Proveedores
* **Ubicación:** `Panel Vendedora` &rarr; `Reabastecer Proveedor` (Ruta: `/Inventario/Entrada`).
* **Operaciones:** Permite registrar la recepción de nuevos lotes de cosméticos desde fabricantes (Maybelline, NYX, L'Oréal, etc.), incrementando el stock del producto e ingresando el número de guía de remisión o factura para trazabilidad en el kárdex.

### 4.7 Diagnóstico Inteligente de Inventario con IA (Smart Stock AI)
* **Ubicación:** `Panel Vendedora` &rarr; `Diagnóstico Inteligente IA` (Ruta: `/AsesorIA/ReporteInventario`).
* **Funcionalidad:**
  * Auditoría algorítmica de todas las existencias y ventas de los últimos 30 días.
  * **Dictamen Ejecutivo:** Resumen cualitativo de la salud del inventario.
  * **Alertas Tempranas:** Identificación de productos próximos a agotarse.
  * **Órdenes de Reposición Sugeridas:** La IA calcula el lote óptimo a solicitar a cada proveedor (por ejemplo: *"Generar orden a L'Oréal por 30 unidades de Base Líquida"*).
  * **Detección de Tendencias:** Cosméticos con mayor velocidad de rotación.
