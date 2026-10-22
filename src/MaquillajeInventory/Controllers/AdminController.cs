using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class AdminController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public AdminController(
            IVentaService ventaService,
            IProductoService productoService,
            ICategoriaService categoriaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            var metricas = await _ventaService.ObtenerMetricasDashboardAsync();
            return View(metricas);
        }
    }
}
