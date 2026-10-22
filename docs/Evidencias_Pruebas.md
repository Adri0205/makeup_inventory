# UNIVERSIDAD DON BOSCO
## FACULTAD DE ESTUDIOS TECNOLÓGICOS — CICLO 02/2026

---

# INFORME DE EVIDENCIAS DE PRUEBAS DE FUNCIONAMIENTO
### PROYECTO CÁTEDRA FASE II: SISTEMA WEB "GLOW & GLAMOUR"

---

## 1. INTRODUCCIÓN Y ALCANCE DE LAS PRUEBAS

En cumplimiento con el **Punto 6** de la guía del Proyecto de Cátedra Fase II, este documento presenta el registro detallado de las pruebas de funcionamiento, validación de casos de uso, integración de componentes, verificación del módulo de Inteligencia Artificial y comprobación de la contenerización en Docker.

---

## 2. MATRIZ DE CASOS DE PRUEBA FUNCIONALES Y RESULTADOS

| ID | Módulo / Componente | Caso de Prueba / Escenario | Datos de Entrada | Resultado Esperado | Resultado Obtenido | Estado |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| **CP-01** | Catálogo Cliente | Navegación y filtrado dinámico por categoría de cosméticos. | Clic en categoría *"Labiales"*. | Despliegue exclusivo de productos de la categoría Labiales. | Filtrado reactivo correcto con badge activo y contador de productos. | **APROBADO** |
| **CP-02** | Catálogo Cliente | Búsqueda por texto libre en el catálogo. | Término *"mate"*. | Filtrado de productos cuyo nombre o descripción incluya *"mate"*. | Lista filtrada mostrando únicamente cosméticos con acabado mate. | **APROBADO** |
| **CP-03** | Ficha Cosmética | Consulta detallada de producto y validación de stock. | Clic en *"Ver Detalles"* de Labial Rojo (Stock: 25). | Despliegue de fotos, precio, proveedor y selector de cantidad hasta 25. | Información técnica renderizada correctamente con límite de stock validado. | **APROBADO** |
| **CP-04** | Carrito de Compras | Adición de productos y cálculo de impuestos. | 2 unidades de Labial ($8.50 c/u). | Subtotal: \$17.00, IVA (13%): \$2.21, Total: \$19.21. Contador de navbar actualizado. | Cálculos matemáticos y fiscales exactos; badge del navbar en `2`. | **APROBADO** |
| **CP-05** | Carrito de Compras | Modificación y eliminación de ítems. | Cambio de cantidad a 1 y posterior eliminación. | Recálculo de totales en tiempo real y remoción del producto de la sesión. | Sesión de carrito actualizada sin inconsistencias. | **APROBADO** |
| **CP-06** | Checkout Cliente | Registro de orden de compra online. | Nombre, Email, Teléfono, Dirección y Pago por Tarjeta. | Creación de registro `Venta` y `DetalleVenta`, descarga de stock en BD y emisión de factura. | Venta creada con ID correlativo, stock decrementado en BD y redirección a ticket digital. | **APROBADO** |
| **CP-07** | Rastreo Cliente | Consulta histórica de órdenes por correo electrónico. | Email: `valeria.martinez@gmail.com`. | Despliegue del historial de compras previas con estado y enlace a ticket. | Lista cronológica de compras renderizada con montos y detalles completos. | **APROBADO** |
| **CP-08** | Asesora IA (Cliente) | Recomendación cosmetológica personalizada por tipo de piel y evento. | Piel: *"Grasa"*, Ocasión: *"Noche"*, Mensaje: *"Busco base matificante y labial"*. | La IA devuelve análisis dermatológico, tip de aplicación y cosméticos afines con stock. | Generación de dictamen contextualizado con productos en stock sugeridos para añadir al carrito. | **APROBADO** |
| **CP-09** | Diagnóstico IA (Admin) | Auditoría algorítmica de inventario y alertas tempranas. | Ejecución de `/AsesorIA/ReporteInventario`. | Identificación de productos con &le; 5 unidades y cálculo de órdenes sugeridas. | Detección de 'Rubor Compacto' (Stock: 4) y sugerencia de orden de compra a NYX. | **APROBADO** |
| **CP-10** | Punto de Venta POS | Registro de venta rápida en mostrador por la vendedora. | Selección de 1 Paleta de Sombras ($15.99) y pago en efectivo. | Descarga directa de inventario, registro en kárdex y comprobante de mostrador. | Registro generado con usuario de vendedora asociado y kárdex actualizado. | **APROBADO** |
| **CP-11** | CRUD Productos | Creación de nuevo producto cosmético con validaciones. | Producto con precio negativo o nombre vacío. | Bloqueo por Data Annotations en frontend y capa de servicio. | Mensajes de error mostrados al usuario impidiendo persistencia incorrecta. | **APROBADO** |
| **CP-12** | Kárdex Almacén | Auditoría de movimientos tras una venta y una entrada. | Venta #1 (-2 unidades) y Entrada de proveedor (+50 unidades). | Registro cronológico con fecha, hora, motivo, usuario y balance. | Trazabilidad completa con badges visuales de entrada (verde) y salida (rojo). | **APROBADO** |
| **CP-13** | Contenedor Docker | Construcción y ejecución del entorno en Docker Compose. | `docker compose up -d --build`. | Ambos contenedores (`web` y `sqlserver`) activos y comunicados por red virtual bridge. | Acceso satisfactorio a la aplicación en `http://localhost:8080` con base de datos enlazada. | **APROBADO** |

---

## 3. VALIDACIÓN DE COMPONENTES E INTEGRACIÓN TÉCNICA

### 3.1 Integración Frontend &harr; Backend
* Las peticiones AJAX desde la interfaz de la Asesora IA envían payloads JSON estructurados mediante `fetch` al endpoint `/AsesorIA/Consultar`, recibiendo respuestas en tiempo real con latencia menor a 80 ms en entorno local.
* Las validaciones de formularios en el checkout y en el mantenimiento de productos operan de forma reactiva con jQuery Unobtrusive Validation y son respaldadas obligatoriamente por validaciones de servicio en C#.

### 3.2 Integración Entity Framework Core &harr; Base de Datos SQL Server
* Las operaciones transaccionales complejas (como el Checkout que involucra inserción de cabecera de venta, líneas de detalle, actualización de stock y creación de registros de kárdex) se ejecutan bajo un bloque atómico `IDbContextTransaction`:
  ```csharp
  using var transaction = await _context.Database.BeginTransactionAsync();
  // Operaciones atómicas
  await transaction.CommitAsync();
  ```
  Si se presenta un error o insuficiencia de existencias en el último ítem, el sistema ejecuta automáticamente `RollbackAsync()`, garantizando la integridad de datos bajo principios ACID.

### 3.3 Validación de la Ejecución en Docker
1. **Compilación de Imagen:** Completada exitosamente utilizando el SDK .NET 10 y runtime optimizado `aspnet:10.0`.
2. **Healthcheck de Base de Datos:** El servicio `makeup_web_app` aguarda a que `makeup_sqlserver_db` reporte estado `healthy` mediante la consulta de validación `SELECT 1` antes de montar el pipeline HTTP.
3. **Persistencia de Volúmenes:** Los datos de catálogo, ventas y usuarios se conservan intactos en el volumen `sqlserver_data` incluso tras reiniciar o recrear los contenedores.
