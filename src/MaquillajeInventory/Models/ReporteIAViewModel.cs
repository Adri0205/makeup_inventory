namespace MakeupInventory.Models
{
    public class ReporteIAViewModel
    {
        public string ResumenEjecutivo { get; set; } = string.Empty;
        public List<string> AlertasStock { get; set; } = new();
        public List<string> OportunidadesVenta { get; set; } = new();
        public List<string> RecomendacionesAbastecimiento { get; set; } = new();
        public decimal TotalVentasMes { get; set; }
        public int TotalProductosStockCritico { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
}
