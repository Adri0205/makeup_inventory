using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // GET: /Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerConCategoriaAsync();
            return View(productos);
        }

        // GET: /Productos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // GET: /Productos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
            return View();
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
                return View(producto);
            }

            try
            {
                await _productoService.CrearAsync(producto);
                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
                return View(producto);
            }
        }

        // GET: /Productos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();

            ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
            return View(producto);
        }

        // POST: /Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
                return View(producto);
            }

            try
            {
                await _productoService.ActualizarAsync(producto);
                TempData["Success"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
                return View(producto);
            }
        }

        // GET: /Productos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        // POST: /Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.EliminarAsync(id);
                TempData["Success"] = "Producto eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}