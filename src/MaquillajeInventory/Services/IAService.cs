using MakeupInventory.Data;
using MakeupInventory.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace MakeupInventory.Services
{
    public class IAService : IIAService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IAService> _logger;

        public IAService(ApplicationDbContext context, ILogger<IAService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RecomendacionIAResponse> ObtenerRecomendacionClienteAsync(ConsultaIARequest request)
        {
            var consulta = (request.Mensaje ?? string.Empty).ToLowerInvariant();
            var tipoPiel = (request.TipoPiel ?? string.Empty).ToLowerInvariant();
            var ocasion = (request.Ocasion ?? string.Empty).ToLowerInvariant();
            var tonoPiel = (request.TonoPiel ?? string.Empty).ToLowerInvariant();
            decimal presupuesto = request.PresupuestoMaximo ?? 1000m;

            // Obtener productos activos de la base de datos con sus categorías
            var productosDisponibles = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0 && p.Precio <= presupuesto)
                .ToListAsync();

            var scored = new List<(Producto Producto, int Score, string Motivo)>();

            foreach (var prod in productosDisponibles)
            {
                int score = 0;
                var razones = new List<string>();
                var prodNombre = prod.Nombre.ToLowerInvariant();
                var prodDesc = (prod.Descripcion ?? string.Empty).ToLowerInvariant();
                var catNombre = (prod.Categoria?.Nombre ?? string.Empty).ToLowerInvariant();

                // 1. Análisis de Categoría
                if (consulta.Contains("labial") || consulta.Contains("labios") || consulta.Contains("lipstick"))
                {
                    if (catNombre.Contains("labial")) { score += 40; razones.Add("Línea de labios especializada"); }
                }
                else if (consulta.Contains("base") || consulta.Contains("corrector") || consulta.Contains("cobertura") || consulta.Contains("rostro"))
                {
                    if (catNombre.Contains("base")) { score += 40; razones.Add("Base unificadora de tono"); }
                }
                else if (consulta.Contains("sombra") || consulta.Contains("paleta") || consulta.Contains("ojos") || consulta.Contains("párpado"))
                {
                    if (catNombre.Contains("sombra")) { score += 40; razones.Add("Paleta de sombras para ojos"); }
                }
                else if (consulta.Contains("rubor") || consulta.Contains("mejillas") || consulta.Contains("iluminador") || consulta.Contains("blush"))
                {
                    if (catNombre.Contains("rubor")) { score += 40; razones.Add("Aporte de color y luminosidad"); }
                }
                else if (consulta.Contains("delineador") || consulta.Contains("eyeliner") || consulta.Contains("trazo"))
                {
                    if (catNombre.Contains("delineador")) { score += 40; razones.Add("Delineado de alta definición"); }
                }

                // 2. Análisis por Tipo de Piel
                if (tipoPiel.Contains("grasa") || consulta.Contains("grasa") || consulta.Contains("brillo"))
                {
                    if (prodNombre.Contains("mate") || prodDesc.Contains("mate") || catNombre.Contains("base") || catNombre.Contains("polvo"))
                    {
                        score += 30;
                        razones.Add("Fórmula con acabado mate para control de brillo y sebo");
                    }
                }
                else if (tipoPiel.Contains("seca") || consulta.Contains("seca") || consulta.Contains("hidrat"))
                {
                    if (prodNombre.Contains("líquida") || prodDesc.Contains("líquid") || prodNombre.Contains("hidrat") || catNombre.Contains("base"))
                    {
                        score += 30;
                        razones.Add("Textura hidratante que previene cuarteaduras en piel seca");
                    }
                }
                else if (tipoPiel.Contains("mixta") || consulta.Contains("mixta"))
                {
                    if (catNombre.Contains("base") || catNombre.Contains("rubor"))
                    {
                        score += 25;
                        razones.Add("Equilibrio ideal para zona T y mejillas");
                    }
                }

                // 3. Análisis por Ocasión
                if (ocasion.Contains("noche") || ocasion.Contains("fiesta") || consulta.Contains("noche") || consulta.Contains("fiesta") || consulta.Contains("elegante"))
                {
                    if (prodNombre.Contains("rojo") || prodNombre.Contains("delineador") || catNombre.Contains("sombra") || prodNombre.Contains("coral"))
                    {
                        score += 25;
                        razones.Add("Pigmentación intensa ideal para eventos nocturnos y de gala");
                    }
                }
                else if (ocasion.Contains("diario") || ocasion.Contains("trabajo") || ocasion.Contains("casual") || consulta.Contains("diario") || consulta.Contains("natural"))
                {
                    if (prodNombre.Contains("nude") || prodNombre.Contains("rosa") || prodNombre.Contains("coral") || prodNombre.Contains("medio"))
                    {
                        score += 25;
                        razones.Add("Tonos sutiles y elegantes para el día a día");
                    }
                }

                // 4. Coincidencias de palabras clave en texto libre
                var palabrasConsulta = Regex.Split(consulta, @"\W+").Where(w => w.Length > 3);
                foreach (var palabra in palabrasConsulta)
                {
                    if (prodNombre.Contains(palabra) || prodDesc.Contains(palabra))
                    {
                        score += 15;
                    }
                }

                if (score > 0)
                {
                    var motivoFinal = razones.Any()
                        ? string.Join("; ", razones)
                        : "Producto afín con alta compatibilidad y disponibilidad inmediata.";
                    scored.Add((prod, score, motivoFinal));
                }
            }

            // Si no hubo coincidencias estrictas, tomar los productos más populares o con buen stock
            if (!scored.Any())
            {
                var topProductos = productosDisponibles.OrderByDescending(p => p.Stock).Take(3);
                foreach (var p in topProductos)
                {
                    scored.Add((p, 10, "Producto estrella recomendado de nuestra colección principal."));
                }
            }

            var seleccionados = scored
                .OrderByDescending(s => s.Score)
                .Take(4)
                .Select(s => new ProductoRecomendadoDTO
                {
                    Id = s.Producto.Id,
                    Nombre = s.Producto.Nombre,
                    Precio = s.Producto.Precio,
                    Categoria = s.Producto.Categoria?.Nombre ?? "Cosméticos",
                    ImagenUrl = s.Producto.ImagenUrl,
                    Stock = s.Producto.Stock,
                    MotivoRecomendacion = s.Motivo
                })
                .ToList();

            // Construir respuesta generativa estructurada
            var respuestaBuilder = new StringBuilder();
            respuestaBuilder.AppendLine("¡Hola! Con base en tus preferencias y necesidades cosméticas, nuestro Asistente Inteligente ha analizado el catálogo en tiempo real.");

            if (!string.IsNullOrEmpty(tipoPiel))
            {
                respuestaBuilder.AppendLine($"\nPara tu tipo de piel ({tipoPiel.ToUpperInvariant()}), priorizamos fórmulas que preserven la textura y aseguren una óptima adherencia.");
            }

            if (!string.IsNullOrEmpty(ocasion))
            {
                respuestaBuilder.AppendLine($"Considerando tu evento u ocasión ({ocasion}), seleccionamos una paleta de color y duración adecuada.");
            }

            var consejo = ObtenerConsejoCosmetico(tipoPiel, ocasion);

            return new RecomendacionIAResponse
            {
                Exitoso = true,
                Respuesta = respuestaBuilder.ToString(),
                ConsejoExperto = consejo,
                ProductosRecomendados = seleccionados
            };
        }

        public async Task<ReporteIAViewModel> GenerarReporteInventarioIAAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();

            var ventas = await _context.Ventas
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            var criticos = productos.Where(p => p.Stock <= 5).ToList();
            var totalVentasMes = ventas
                .Where(v => v.Fecha >= DateTime.Now.AddDays(-30))
                .Sum(v => v.Total);

            var alertasStock = new List<string>();
            var recomendacionesAbastecimiento = new List<string>();
            var oportunidades = new List<string>();

            foreach (var crit in criticos)
            {
                alertasStock.Add($"Alerta Crítica: '{crit.Nombre}' (Categoría: {crit.Categoria?.Nombre}) cuenta con únicamente {crit.Stock} unidades en almacén.");
                recomendacionesAbastecimiento.Add($"Generar orden de compra a proveedor '{crit.Proveedor ?? "Principal"}' por un mínimo de 30 unidades de '{crit.Nombre}'.");
            }

            if (!alertasStock.Any())
            {
                alertasStock.Add("Niveles de inventario óptimos. Ningún producto está por debajo del umbral de seguridad de 5 unidades.");
                recomendacionesAbastecimiento.Add("Mantener el ritmo de rotación actual y revisar lotes de reposición para el próximo ciclo.");
            }

            // Oportunidades detectadas
            var masVendidos = ventas
                .SelectMany(v => v.Detalles ?? Enumerable.Empty<DetalleVenta>())
                .GroupBy(d => d.ProductoId)
                .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .OrderByDescending(x => x.Cantidad)
                .Take(3)
                .ToList();

            foreach (var mv in masVendidos)
            {
                var p = productos.FirstOrDefault(x => x.Id == mv.ProductoId);
                if (p != null)
                {
                    oportunidades.Add($"Alta Demanda: '{p.Nombre}' registra {mv.Cantidad} unidades vendidas. Se sugiere mantener stock de seguridad ampliado.");
                }
            }

            if (!oportunidades.Any())
            {
                oportunidades.Add("Campaña sugerida: Impulsar la categoría de 'Labiales' y 'Sombras' mediante descuentos de temporada.");
            }

            var sbResumen = new StringBuilder();
            sbResumen.AppendLine($"Análisis de Inventario e Inteligencia de Negocio ejecutado el {DateTime.Now:dd/MM/yyyy HH:mm}. ");
            sbResumen.AppendLine($"Se auditaron {productos.Count} referencias activas y {ventas.Count} transacciones comerciales.");
            if (criticos.Any())
            {
                sbResumen.AppendLine($"Se detectaron {criticos.Count} referencias con riesgo inminente de desabastecimiento, requiriendo atención prioritaria de adquisiciones.");
            }
            else
            {
                sbResumen.AppendLine("El inventario se encuentra balanceado con niveles de stock dentro de los márgenes seguros de operación.");
            }

            return new ReporteIAViewModel
            {
                ResumenEjecutivo = sbResumen.ToString(),
                AlertasStock = alertasStock,
                OportunidadesVenta = oportunidades,
                RecomendacionesAbastecimiento = recomendacionesAbastecimiento,
                TotalVentasMes = totalVentasMes,
                TotalProductosStockCritico = criticos.Count,
                FechaGeneracion = DateTime.Now
            };
        }

        private static string ObtenerConsejoCosmetico(string tipoPiel, string ocasion)
        {
            if (tipoPiel.Contains("grasa"))
            {
                return "Tip de Maquillista: Limpia y tonifica tu piel antes de aplicar la base. Utiliza primer matificante y sella con polvos traslúcidos para prolongar la duración sin retoques.";
            }
            if (tipoPiel.Contains("seca"))
            {
                return "Tip de Maquillista: Aplica un suero con ácido hialurónico antes de maquillar. Prefiere fórmulas cremosas y evita el exceso de polvos para lograr un brillo saludable (dewy glow).";
            }
            if (ocasion.Contains("noche") || ocasion.Contains("fiesta"))
            {
                return "Tip de Maquillista: Para eventos de noche, enfoca la atención en una sola zona protagónica: si usas un labial rojo intenso, mantén los ojos con sombras neutras y delineado limpio.";
            }

            return "Tip de Maquillista: Recuerda siempre preparar tu rostro con protector solar y desmaquillar al final del día para mantener tu piel radiante.";
        }
    }
}
