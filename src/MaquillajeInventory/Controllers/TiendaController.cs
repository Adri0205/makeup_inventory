using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class TiendaController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ICarritoService _carritoService;
        private readonly IVentaService _ventaService;

        public TiendaController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            ICarritoService carritoService,
            IVentaService ventaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _carritoService = carritoService;
            _ventaService = ventaService;
        }

        // GET: /Tienda o /
        public async Task<IActionResult> Index(int? categoriaId, string? buscar, string? ordenar)
        {
            var productos = (await _productoService.ObtenerConCategoriaAsync()).Where(p => p.Stock > 0);

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var q = buscar.Trim().ToLower();
                productos = productos.Where(p =>
                    p.Nombre.ToLower().Contains(q) ||
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(q)) ||
                    (p.Proveedor != null && p.Proveedor.ToLower().Contains(q)));
            }

            // Ordenamiento
            productos = ordenar switch
            {
                "precio_asc" => productos.OrderBy(p => p.Precio),
                "precio_desc" => productos.OrderByDescending(p => p.Precio),
                "nombre" => productos.OrderBy(p => p.Nombre),
                _ => productos.OrderBy(p => p.Id)
            };

            ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
            ViewBag.CategoriaSeleccionada = categoriaId;
            ViewBag.Buscar = buscar;
            ViewBag.Ordenar = ordenar;
            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();

            return View(productos.ToList());
        }

        // GET: /Tienda/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();

            var relacionados = (await _productoService.ObtenerConCategoriaAsync())
                .Where(p => p.CategoriaId == producto.CategoriaId && p.Id != producto.Id && p.Stock > 0)
                .Take(4)
                .ToList();

            ViewBag.Relacionados = relacionados;
            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
            return View(producto);
        }

        // GET: /Tienda/Carrito
        public IActionResult Carrito()
        {
            var items = _carritoService.ObtenerCarrito();
            var subtotal = _carritoService.ObtenerSubtotal();
            var iva = subtotal * 0.13m;
            var total = subtotal + iva;

            ViewBag.Subtotal = subtotal;
            ViewBag.IVA = iva;
            ViewBag.Total = total;
            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();

            return View(items);
        }

        // POST: /Tienda/AgregarAlCarrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int productoId, int cantidad = 1, bool json = false)
        {
            var producto = await _productoService.ObtenerPorIdAsync(productoId);
            if (producto == null)
            {
                if (json) return Json(new { success = false, message = "Producto no encontrado" });
                return NotFound();
            }

            if (producto.Stock < cantidad)
            {
                if (json) return Json(new { success = false, message = $"Stock insuficiente. Solo quedan {producto.Stock} unidades." });
                TempData["Error"] = $"Solo quedan {producto.Stock} unidades de '{producto.Nombre}'.";
                return RedirectToAction(nameof(Carrito));
            }

            _carritoService.AgregarItem(producto, cantidad);

            if (json)
            {
                return Json(new
                {
                    success = true,
                    totalItems = _carritoService.ObtenerCantidadTotal(),
                    subtotal = _carritoService.ObtenerSubtotal(),
                    message = $"¡'{producto.Nombre}' agregado al carrito!"
                });
            }

            TempData["Success"] = $"'{producto.Nombre}' añadido al carrito.";
            return RedirectToAction(nameof(Carrito));
        }

        // POST: /Tienda/ActualizarCantidad
        [HttpPost]
        public IActionResult ActualizarCantidad(int productoId, int cantidad)
        {
            _carritoService.ActualizarCantidad(productoId, cantidad);
            return RedirectToAction(nameof(Carrito));
        }

        // POST: /Tienda/EliminarDelCarrito
        [HttpPost]
        public IActionResult EliminarDelCarrito(int productoId)
        {
            _carritoService.EliminarItem(productoId);
            TempData["Success"] = "Producto eliminado del carrito.";
            return RedirectToAction(nameof(Carrito));
        }

        // GET: /Tienda/Checkout
        public IActionResult Checkout()
        {
            var items = _carritoService.ObtenerCarrito();
            if (!items.Any())
            {
                TempData["Error"] = "Tu carrito de compras está vacío.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CheckoutViewModel
            {
                Items = items
            };

            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
            return View(model);
        }

        // POST: /Tienda/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            model.Items = _carritoService.ObtenerCarrito();
            if (!model.Items.Any())
            {
                TempData["Error"] = "El carrito está vacío. Agrega productos antes de continuar.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
                return View(model);
            }

            try
            {
                var venta = await _ventaService.RegistrarVentaOnlineAsync(model);
                _carritoService.LimpiarCarrito();
                return RedirectToAction(nameof(PedidoConfirmado), new { id = venta.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el pedido: " + ex.Message);
                ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
                return View(model);
            }
        }

        // GET: /Tienda/PedidoConfirmado/5
        public async Task<IActionResult> PedidoConfirmado(int id)
        {
            var venta = await _ventaService.ObtenerPorIdAsync(id);
            if (venta == null) return NotFound();

            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
            return View(venta);
        }

        // GET: /Tienda/MisPedidos
        public async Task<IActionResult> MisPedidos(string? email)
        {
            ViewBag.Email = email;
            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();

            if (string.IsNullOrWhiteSpace(email))
            {
                return View(new List<Venta>());
            }

            var pedidos = await _ventaService.ObtenerPorClienteEmailAsync(email);
            return View(pedidos);
        }
    }
}
