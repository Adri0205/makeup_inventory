using MakeupInventory.Data;
using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Controllers
{
    public class InventarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductoService _productoService;

        public InventarioController(ApplicationDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        // GET: /Inventario/Movimientos
        public async Task<IActionResult> Movimientos()
        {
            var movimientos = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .Include(m => m.Usuario)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return View(movimientos);
        }

        // GET: /Inventario/Entrada
        public async Task<IActionResult> Entrada()
        {
            ViewBag.Productos = await _productoService.ObtenerTodosAsync();
            return View();
        }

        // POST: /Inventario/Entrada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entrada(int productoId, int cantidad, string motivo, string? proveedor)
        {
            if (productoId <= 0 || cantidad <= 0 || string.IsNullOrWhiteSpace(motivo))
            {
                TempData["Error"] = "Complete todos los campos obligatorios y asegure una cantidad válida mayor a 0.";
                ViewBag.Productos = await _productoService.ObtenerTodosAsync();
                return View();
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return NotFound();

            // Actualizar stock
            producto.Stock += cantidad;
            if (!string.IsNullOrWhiteSpace(proveedor))
            {
                producto.Proveedor = proveedor.Trim();
            }

            // Registrar movimiento
            await _context.MovimientosInventario.AddAsync(new MovimientoInventario
            {
                ProductoId = producto.Id,
                Cantidad = cantidad,
                Fecha = DateTime.Now,
                Motivo = $"Entrada Almacén: {motivo.Trim()}"
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Se han agregado {cantidad} unidades al producto '{producto.Nombre}'. Nuevo stock: {producto.Stock}.";
            return RedirectToAction(nameof(Movimientos));
        }
    }
}
