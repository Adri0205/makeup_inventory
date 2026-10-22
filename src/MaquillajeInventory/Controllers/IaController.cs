using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MakeupInventory.Data;

namespace MakeupInventory.Controllers
{
    public class IaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IaController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET:Ia/AsesoraVirtual
        public IActionResult AsesoraVirtual()
        {
            return View();
        }

        //POST:Ia/ConsultarIa
        [HttpPost]
        public async Task<IActionResult> ConsultarIa([FromBody] ConsultaIaRequest model)
        {
            if (string.IsNullOrWhiteSpace(model?.Prompt))
                return Json(new { respuesta = "Por favor ingresa una consulta válida." });

            string promptLower = model.Prompt.ToLower().Trim();

            //TOP PRODUCTOS MÁS VENDIDOS ESTA SEMANA (Últimos 7 días)
            if (promptLower.Contains("mas vendido") || promptLower.Contains("más vendido") || promptLower.Contains("mas vendidos") || promptLower.Contains("más vendidos"))
            {
                if (promptLower.Contains("semana"))
                {
                    var fechaInicioSemana = DateTime.Now.AddDays(-7);

                    var masVendidosSemana = await _context.DetalleVentas
                        .Include(dv => dv.Venta)
                        .Include(dv => dv.Producto)
                        .ThenInclude(p => p.Categoria)
                        .Where(dv => dv.Venta.Fecha >= fechaInicioSemana)
                        .GroupBy(dv => new { dv.ProductoId, dv.Producto.Nombre, dv.Producto.Precio, dv.Producto.Stock, CategoriaNombre = dv.Producto.Categoria.Nombre })
                        .Select(g => new ProductoTopDto
                        {
                            Nombre = g.Key.Nombre,
                            Categoria = g.Key.CategoriaNombre,
                            Precio = g.Key.Precio,
                            Stock = g.Key.Stock,
                            TotalUnidadesVendidas = g.Sum(x => x.Cantidad),
                            TotalRecaudado = g.Sum(x => x.Subtotal)
                        })
                        .OrderByDescending(x => x.TotalUnidadesVendidas)
                        .Take(5)
                        .ToListAsync();

                    return Json(new { respuesta = ConstruirTablaTopVendidos("Top Productos Más Vendidos esta Semana (Últimos 7 días)", masVendidosSemana) });
                }

                //TOP PRODUCTOS MÁS VENDIDOS DEL ÚLTIMO MES (Últimos 30 días)
                if (promptLower.Contains("mes") || promptLower.Contains("ultimo mes") || promptLower.Contains("último mes"))
                {
                    var fechaInicioMes = DateTime.Now.AddDays(-30);

                    var masVendidosMes = await _context.DetalleVentas
                        .Include(dv => dv.Venta)
                        .Include(dv => dv.Producto)
                        .ThenInclude(p => p.Categoria)
                        .Where(dv => dv.Venta.Fecha >= fechaInicioMes)
                        .GroupBy(dv => new { dv.ProductoId, dv.Producto.Nombre, dv.Producto.Precio, dv.Producto.Stock, CategoriaNombre = dv.Producto.Categoria.Nombre })
                        .Select(g => new ProductoTopDto
                        {
                            Nombre = g.Key.Nombre,
                            Categoria = g.Key.CategoriaNombre,
                            Precio = g.Key.Precio,
                            Stock = g.Key.Stock,
                            TotalUnidadesVendidas = g.Sum(x => x.Cantidad),
                            TotalRecaudado = g.Sum(x => x.Subtotal)
                        })
                        .OrderByDescending(x => x.TotalUnidadesVendidas)
                        .Take(5)
                        .ToListAsync();

                    return Json(new { respuesta = ConstruirTablaTopVendidos("Top Productos Más Vendidos del Último Mes (Últimos 30 días)", masVendidosMes) });
                }
            }

            //MOSTRAR CATÁLOGO COMPLETO
            if (promptLower.Contains("todos") || promptLower.Contains("catalogo") || promptLower.Contains("catálogo") || promptLower.Equals("ver productos"))
            {
                var todosProductos = await _context.Productos
                    .Include(p => p.Categoria)
                    .OrderBy(p => p.Categoria.Nombre)
                    .ToListAsync();

                return Json(new { respuesta = ConstruirTablaProductos($"Catálogo Completo ({todosProductos.Count} referencias)", todosProductos) });
            }

            //CONSULTA DE STOCK BAJO / CRÍTICO
            if (promptLower.Contains("stock") && (promptLower.Contains("bajo") || promptLower.Contains("critico") || promptLower.Contains("crítico") || promptLower.Contains("5")))
            {
                var productosCriticos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.Stock <= 5)
                    .ToListAsync();

                if (productosCriticos.Any())
                {
                    return Json(new { respuesta = ConstruirTablaProductos("Productos en Stock Crítico (≤ 5 unidades)", productosCriticos) });
                }

                return Json(new { respuesta = " Todo el catálogo cuenta con más de 5 unidades en existencia." });
            }

            //BÚSQUEDA DINÁMICA POR NOMBRE, CATEGORÍA O PROVEEDOR (ej. "proveedor Maybelline", "L'Oréal", "Sombras")
            string busqueda = promptLower
                .Replace("ver ", "")
                .Replace("buscar ", "")
                .Replace("mostrar ", "")
                .Replace("dame ", "")
                .Replace("proveedor ", "")
                .Replace("proveedores ", "")
                .Replace("marca ", "")
                .Replace("los ", "")
                .Replace("las ", "")
                .Trim();

            if (!string.IsNullOrWhiteSpace(busqueda) && busqueda.Length >= 2)
            {
                var coincidencias = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => EF.Functions.Like(p.Nombre, $"%{busqueda}%")
                             || EF.Functions.Like(p.Categoria.Nombre, $"%{busqueda}%")
                             || EF.Functions.Like(p.Proveedor, $"%{busqueda}%"))
                    .ToListAsync();

                if (coincidencias.Any())
                {
                    return Json(new { respuesta = ConstruirTablaProductos($"Resultados para «{busqueda}» ({coincidencias.Count} encontrados)", coincidencias) });
                }
            }

            //Fallback si no encuentra coincidencias
            string respuestaFallback = $"No encontré información para «<em>{model.Prompt}</em>». Puedes probar buscando por proveedor (ej. <em>\"Maybelline\"</em>), o pedir <strong>\"más vendidos esta semana\"</strong> o <strong>\"más vendidos del último mes\"</strong>.";
            return Json(new { respuesta = respuestaFallback });
        }

        //Tabla HTML para catálogo general o filtrado
        private string ConstruirTablaProductos(string titulo, List<MakeupInventory.Models.Producto> productos)
        {
            string html = $" <strong>{titulo}:</strong><br/>";
            html += "<div class='table-responsive mt-2'><table class='table table-sm table-bordered align-middle mb-0' style='font-size: 0.85rem;'>";
            html += "<thead class='table-light'><tr><th>Producto</th><th>Categoría</th><th>Proveedor</th><th>Precio</th><th>Stock</th></tr></thead><tbody>";

            foreach (var p in productos)
            {
                string badgeStock = p.Stock <= 5
                    ? $"<span class='badge bg-danger'>{p.Stock} unds.</span>"
                    : $"<span class='badge bg-success'>{p.Stock} unds.</span>";

                html += $"<tr>" +
                        $"<td><strong>{p.Nombre}</strong></td>" +
                        $"<td>{p.Categoria?.Nombre ?? "Sin Categ."}</td>" +
                        $"<td>{p.Proveedor ?? "N/A"}</td>" +
                        $"<td>${p.Precio:N2}</td>" +
                        $"<td>{badgeStock}</td>" +
                        $"</tr>";
            }

            html += "</tbody></table></div>";
            return html;
        }

        //Tabla HTML para el ranking de más vendidos
        private string ConstruirTablaTopVendidos(string titulo, List<ProductoTopDto> productosTop)
        {
            if (!productosTop.Any())
            {
                return $"<strong>{titulo}:</strong><br/><em class='text-muted'>No se han registrado ventas en este período.</em>";
            }

            string html = $"<strong>{titulo}:</strong><br/>";
            html += "<div class='table-responsive mt-2'><table class='table table-sm table-bordered align-middle mb-0' style='font-size: 0.85rem;'>";
            html += "<thead class='table-light'><tr><th>Producto</th><th>Categoría</th><th>Vendidos</th><th>Total Recaudado</th><th>Stock Actual</th></tr></thead><tbody>";

            foreach (var item in productosTop)
            {
                html += $"<tr>" +
                        $"<td><strong>{item.Nombre}</strong></td>" +
                        $"<td>{item.Categoria}</td>" +
                        $"<td><span class='badge bg-primary'>{item.TotalUnidadesVendidas} unid.</span></td>" +
                        $"<td class='fw-bold text-success'>${item.TotalRecaudado:N2}</td>" +
                        $"<td>{item.Stock} unid.</td>" +
                        $"</tr>";
            }

            html += "</tbody></table></div>";
            return html;
        }
    }

    //DTO auxiliar para la proyección de ventas
    public class ProductoTopDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int TotalUnidadesVendidas { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class ConsultaIaRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
}