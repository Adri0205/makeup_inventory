namespace MakeupInventory.Models
{
    public class NuevaVentaItemDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class NuevaVentaPOSViewModel
    {
        public int? UsuarioId { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteEmail { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public List<NuevaVentaItemDTO> Items { get; set; } = new();
    }
}
