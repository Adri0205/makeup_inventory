namespace MakeupInventory.Models
{
    public class ConsultaIARequest
    {
        public string Mensaje { get; set; } = string.Empty;
        public string? TipoPiel { get; set; } // Grasa, Seca, Mixta, Sensible, Normal
        public string? Ocasion { get; set; } // Diario, Noche, Fiesta, Boda, Trabajo
        public decimal? PresupuestoMaximo { get; set; }
        public string? TonoPiel { get; set; } // Claro, Medio, Moreno, Calido, Frio
    }

    public class RecomendacionIAResponse
    {
        public bool Exitoso { get; set; } = true;
        public string Respuesta { get; set; } = string.Empty;
        public string ConsejoExperto { get; set; } = string.Empty;
        public List<ProductoRecomendadoDTO> ProductosRecomendados { get; set; } = new();
    }

    public class ProductoRecomendadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public int Stock { get; set; }
        public string MotivoRecomendacion { get; set; } = string.Empty;
    }
}
