namespace MakeupInventory.Models
{
    public class DashboardViewModel
    {
        public string Saludo { get; set; } = "¡Hola, Propietaria!";
        public string PeriodoConsulta { get; set; } = "Octubre 2026";
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalVentas { get; set; }
        public decimal MontoTotalVentas { get; set; }
        public int ProductosStockBajo { get; set; }
        public int CuotasPendientes { get; set; }
        public decimal SaldoPendienteCuotas { get; set; }
        public List<Producto> UltimosProductos { get; set; } = new();
        public List<Venta> UltimasVentas { get; set; } = new();
        public List<Producto> ProductosCriticos { get; set; } = new();
        public List<ProductoMasVendidoDTO> ProductosMasVendidos { get; set; } = new();
        public List<AlertaDashboardDTO> AlertasAccionables { get; set; } = new();
        public List<VentaMensualDTO> HistorialGraficoVentas { get; set; } = new();
    }

    public class ProductoMasVendidoDTO
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int UnidadesVendidas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class AlertaDashboardDTO
    {
        public string Tipo { get; set; } = "warning"; // danger, warning, info
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string EnlaceAccion { get; set; } = string.Empty;
        public string TextoAccion { get; set; } = string.Empty;
    }

    public class VentaMensualDTO
    {
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}
