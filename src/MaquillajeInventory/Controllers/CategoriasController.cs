using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: /Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            return View(categorias);
        }

        // GET: /Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            try
            {
                await _categoriaService.CrearAsync(categoria);
                TempData["Success"] = "Categoría creada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(categoria);
            }
        }

        // GET: /Categorias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // POST: /Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();
            if (!ModelState.IsValid) return View(categoria);

            try
            {
                await _categoriaService.ActualizarAsync(categoria);
                TempData["Success"] = "Categoría actualizada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(categoria);
            }
        }

        // GET: /Categorias/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // POST: /Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoriaService.EliminarAsync(id);
                TempData["Success"] = "Categoría eliminada con éxito.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
