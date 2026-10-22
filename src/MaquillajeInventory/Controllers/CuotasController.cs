using MakeupInventory.Data;
using MakeupInventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Controllers
{
    public class CuotasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cuotas (Módulo Clientes y Cuotas - Mockup 5.1.4.f)
        public async Task<IActionResult> Index()
        {
            var cuotas = await _context.Cuotas
                .Include(c => c.Venta)
                .Include(c => c.Cliente)
                .OrderBy(c => c.FechaVencimiento)
                .ToListAsync();

            var totalCuotas = cuotas.Sum(c => c.Monto);
            var totalCobrado = cuotas.Where(c => c.Estado == "Pagada").Sum(c => c.Monto);
            var saldoPendiente = cuotas.Where(c => c.Estado != "Pagada").Sum(c => c.Monto);
            var cuotasVencidas = cuotas.Count(c => c.Estado != "Pagada" && c.FechaVencimiento < DateTime.Now);
            var proximasVencer = cuotas.Count(c => c.Estado != "Pagada" && c.FechaVencimiento >= DateTime.Now && c.FechaVencimiento <= DateTime.Now.AddDays(7));

            ViewBag.TotalCuotas = totalCuotas;
            ViewBag.TotalCobrado = totalCobrado;
            ViewBag.SaldoPendiente = saldoPendiente;
            ViewBag.CuotasVencidas = cuotasVencidas;
            ViewBag.ProximasVencer = proximasVencer;

            return View(cuotas);
        }

        // POST: /Cuotas/RegistrarPago/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPago(int id)
        {
            var cuota = await _context.Cuotas.FindAsync(id);
            if (cuota == null) return NotFound();

            cuota.Estado = "Pagada";
            cuota.FechaPago = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Pago de cuota de ${cuota.Monto:0.00} registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cuotas/Nueva
        public async Task<IActionResult> Nueva()
        {
            ViewBag.Ventas = await _context.Ventas.OrderByDescending(v => v.Fecha).Take(20).ToListAsync();
            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            return View();
        }

        // POST: /Cuotas/Nueva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nueva(Cuota cuota)
        {
            if (cuota.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto de la cuota debe ser mayor a 0.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Ventas = await _context.Ventas.OrderByDescending(v => v.Fecha).Take(20).ToListAsync();
                ViewBag.Clientes = await _context.Clientes.ToListAsync();
                return View(cuota);
            }

            cuota.Estado = "Pendiente";
            await _context.Cuotas.AddAsync(cuota);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Plan de cuota programado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
