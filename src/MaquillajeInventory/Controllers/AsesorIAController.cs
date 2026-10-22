using MakeupInventory.Models;
using MakeupInventory.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeupInventory.Controllers
{
    public class AsesorIAController : Controller
    {
        private readonly IIAService _iaService;
        private readonly ICarritoService _carritoService;

        public AsesorIAController(IIAService iaService, ICarritoService carritoService)
        {
            _iaService = iaService;
            _carritoService = carritoService;
        }

        // GET: /AsesorIA (Portal del Cliente - Asesor de Belleza Virtual)
        public IActionResult Index()
        {
            ViewBag.TotalItemsCarrito = _carritoService.ObtenerCantidadTotal();
            return View();
        }

        // POST: /AsesorIA/Consultar (Endpoint para Chatbot IA y Asesoría)
        [HttpPost]
        public async Task<IActionResult> Consultar([FromBody] ConsultaIARequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Mensaje))
            {
                return Json(new { exitoso = false, respuesta = "Por favor ingresa una pregunta o consulta para poder ayudarte." });
            }

            try
            {
                var recomendacion = await _iaService.ObtenerRecomendacionClienteAsync(request);
                return Json(recomendacion);
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, respuesta = "Ocurrió un error al procesar la solicitud con IA: " + ex.Message });
            }
        }

        // GET: /AsesorIA/ReporteInventario (Portal Vendedora/Admin - Smart Inventory Insights)
        public async Task<IActionResult> ReporteInventario()
        {
            var reporte = await _iaService.GenerarReporteInventarioIAAsync();
            return View(reporte);
        }
    }
}
