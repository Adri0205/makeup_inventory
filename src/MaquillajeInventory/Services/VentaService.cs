using MakeupInventory.Data;
using MakeupInventory.Models;
using MakeupInventory.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Services
{
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;

        public VentaService(
            ApplicationDbContext context,
            IVentaRepository ventaRepository,
            IProductoRepository productoRepository)
        {
            _context = context;
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Venta>> ObtenerTodasAsync()
        {
            return await _ventaRepository.GetVentasConDetallesAsync();
        }

        public async Task<Venta?> ObtenerPorIdAsync(int id)
        {
            return await _ventaRepository.GetVentaConDetallesPorIdAsync(id);
        }

        public async Task<IEnumerable<Venta>> ObtenerPorClienteEmailAsync(string email)
        {
            return await _ventaRepository.GetVentasPorEmailClienteAsync(email);
        }

        public async Task<Venta> RegistrarVentaOnlineAsync(CheckoutViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
                throw new InvalidOperationException("El carrito no contiene productos.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalCalculado = 0;
                var detalles = new List<DetalleVenta>();

                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    MetodoPago = model.MetodoPago,
                    ClienteNombre = model.ClienteNombre.Trim(),
                    ClienteEmail = model.ClienteEmail.Trim(),
                    ClienteTelefono = model.ClienteTelefono.Trim(),
                    DireccionEntrega = model.DireccionEntrega.Trim(),
                    Estado = "Completada"
                };

                await _context.Ventas.AddAsync(venta);
                await _context.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null)
                        throw new InvalidOperationException($"El producto con ID {item.ProductoId} no existe.");

                    if (producto.Stock < item.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}.");

                    producto.Stock -= item.Cantidad;

                    var subtotal = producto.Precio * item.Cantidad;
                    totalCalculado += subtotal;

                    detalles.Add(new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotal
                    });

                    await _context.MovimientosInventario.AddAsync(new MovimientoInventario
                    {
                        ProductoId = producto.Id,
                        Cantidad = -item.Cantidad,
                        Fecha = DateTime.Now,
                        Motivo = $"Venta Online - Orden #{venta.Id}"
                    });
                }

                decimal iva = totalCalculado * 0.13m;
                venta.Total = totalCalculado + iva;

                await _context.DetalleVentas.AddRangeAsync(detalles);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return venta;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Venta> RegistrarVentaPOSAsync(NuevaVentaPOSViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
                throw new InvalidOperationException("Debe seleccionar al menos un producto para la venta.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalCalculado = 0;
                var detalles = new List<DetalleVenta>();

                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    MetodoPago = model.MetodoPago,
                    UsuarioId = model.UsuarioId,
                    ClienteNombre = string.IsNullOrWhiteSpace(model.ClienteNombre) ? "Clienta Mostrador" : model.ClienteNombre.Trim(),
                    ClienteEmail = model.ClienteEmail?.Trim(),
                    Estado = "Completada"
                };

                await _context.Ventas.AddAsync(venta);
                await _context.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    if (item.Cantidad <= 0) continue;

                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null)
                        throw new InvalidOperationException($"El producto con ID {item.ProductoId} no existe.");

                    if (producto.Stock < item.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para '{producto.Nombre}'.");

                    producto.Stock -= item.Cantidad;
                    var subtotal = producto.Precio * item.Cantidad;
                    totalCalculado += subtotal;

                    detalles.Add(new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotal
                    });

                    await _context.MovimientosInventario.AddAsync(new MovimientoInventario
                    {
                        ProductoId = producto.Id,
                        Cantidad = -item.Cantidad,
                        Fecha = DateTime.Now,
                        Motivo = $"Venta Mostrador Maquillaje SV #{venta.Id}",
                        UsuarioId = model.UsuarioId
                    });
                }

                venta.Total = totalCalculado;
                await _context.DetalleVentas.AddRangeAsync(detalles);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return venta;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DashboardViewModel> ObtenerMetricasDashboardAsync()
        {
            var totalProductos = await _context.Productos.CountAsync();
            var totalCategorias = await _context.Categorias.CountAsync();
            var ventas = await _context.Ventas
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            var totalVentas = ventas.Count;
            var montoTotal = ventas.Sum(v => v.Total);

            var productosCriticos = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .ToListAsync();

            var ultimosProductos = await _context.Productos
                .Include(p => p.Categoria)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToListAsync();

            var ultimasVentas = ventas
                .OrderByDescending(v => v.Fecha)
                .Take(5)
                .ToList();

            // Cuotas pendientes
            var cuotas = await _context.Cuotas.ToListAsync();
            var cuotasPendientes = cuotas.Count(c => c.Estado != "Pagada");
            var saldoPendienteCuotas = cuotas.Where(c => c.Estado != "Pagada").Sum(c => c.Monto);

            // Top productos más vendidos (rotación)
            var topVendidos = ventas
                .SelectMany(v => v.Detalles ?? Enumerable.Empty<DetalleVenta>())
                .GroupBy(d => new { d.ProductoId, Nombre = d.Producto != null ? d.Producto.Nombre : "Cosmético #" + d.ProductoId })
                .Select(g => new ProductoMasVendidoDTO
                {
                    ProductoId = g.Key.ProductoId,
                    Nombre = g.Key.Nombre,
                    UnidadesVendidas = g.Sum(x => x.Cantidad),
                    TotalRecaudado = g.Sum(x => x.Subtotal)
                })
                .OrderByDescending(x => x.UnidadesVendidas)
                .Take(5)
                .ToList();

            // Alertas accionables (Mockup 5.1.4.b)
            var alertas = new List<AlertaDashboardDTO>();
            if (productosCriticos.Any())
            {
                alertas.Add(new AlertaDashboardDTO
                {
                    Tipo = "danger",
                    Titulo = "Inventario Crítico Detectado",
                    Mensaje = $"Existen {productosCriticos.Count} cosméticos con 5 o menos existencias en bodega.",
                    EnlaceAccion = "/Inventario/Entrada",
                    TextoAccion = "Reabastecer ahora"
                });
            }

            var vencidas = cuotas.Where(c => c.Estado != "Pagada" && c.FechaVencimiento < DateTime.Now).ToList();
            if (vencidas.Any())
            {
                alertas.Add(new AlertaDashboardDTO
                {
                    Tipo = "warning",
                    Titulo = "Cuotas Vencidas Pendientes",
                    Mensaje = $"Hay {vencidas.Count} cuota(s) vencidas por un total de ${vencidas.Sum(c => c.Monto):0.00}.",
                    EnlaceAccion = "/Cuotas/Index",
                    TextoAccion = "Ver cuotas y cobros"
                });
            }

            // Datos para el gráfico de ventas
            var grafico = new List<VentaMensualDTO>
            {
                new VentaMensualDTO { Etiqueta = "Junio", Monto = 210.50m },
                new VentaMensualDTO { Etiqueta = "Julio", Monto = 345.00m },
                new VentaMensualDTO { Etiqueta = "Agosto", Monto = 420.75m },
                new VentaMensualDTO { Etiqueta = "Septiembre", Monto = 390.25m },
                new VentaMensualDTO { Etiqueta = "Octubre", Monto = montoTotal > 0 ? montoTotal : 520.00m }
            };

            return new DashboardViewModel
            {
                Saludo = "¡Hola, Propietaria!",
                PeriodoConsulta = DateTime.Now.ToString("MMMM yyyy"),
                TotalProductos = totalProductos,
                TotalCategorias = totalCategorias,
                TotalVentas = totalVentas,
                MontoTotalVentas = montoTotal,
                ProductosStockBajo = productosCriticos.Count,
                CuotasPendientes = cuotasPendientes,
                SaldoPendienteCuotas = saldoPendienteCuotas,
                ProductosCriticos = productosCriticos,
                UltimosProductos = ultimosProductos,
                UltimasVentas = ultimasVentas,
                ProductosMasVendidos = topVendidos,
                AlertasAccionables = alertas,
                HistorialGraficoVentas = grafico
            };
        }
    }
}
