using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class VentasController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;

        public VentasController(IVentaService ventaService, IProductoService productoService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
        }

        // GET: /Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _ventaService.ObtenerTodasAsync();
            return View(ventas);
        }

        // GET: /Ventas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var venta = await _ventaService.ObtenerPorIdAsync(id);
            if (venta == null) return NotFound();
            return View(venta);
        }

        // GET: /Ventas/Nueva (POS para vendedora)
        public async Task<IActionResult> Nueva()
        {
            var productos = (await _productoService.ObtenerConCategoriaAsync()).Where(p => p.Stock > 0);
            ViewBag.Productos = productos;
            return View(new NuevaVentaPOSViewModel());
        }

        // POST: /Ventas/Nueva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nueva(NuevaVentaPOSViewModel model)
        {
            // Filtrar items con cantidad > 0
            model.Items = model.Items.Where(i => i.Cantidad > 0).ToList();

            if (!model.Items.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un producto con cantidad mayor a cero.");
                ViewBag.Productos = (await _productoService.ObtenerConCategoriaAsync()).Where(p => p.Stock > 0);
                return View(model);
            }

            try
            {
                var venta = await _ventaService.RegistrarVentaPOSAsync(model);
                TempData["Success"] = $"Venta #{venta.Id} registrada exitosamente.";
                return RedirectToAction(nameof(Details), new { id = venta.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al registrar la venta: " + ex.Message);
                ViewBag.Productos = (await _productoService.ObtenerConCategoriaAsync()).Where(p => p.Stock > 0);
                return View(model);
            }
        }
    }
}
