using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeupInventory.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha de venta")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método de pago")]
        [StringLength(50)]
        [Display(Name = "Método de pago")]
        public string MetodoPago { get; set; } = string.Empty;

        [Display(Name = "Vendedor / Usuario")]
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [StringLength(150)]
        [Display(Name = "Nombre de la Clienta")]
        public string? ClienteNombre { get; set; }

        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "Correo de la Clienta")]
        public string? ClienteEmail { get; set; }

        [StringLength(50)]
        [Display(Name = "Teléfono de Contacto")]
        public string? ClienteTelefono { get; set; }

        [StringLength(250)]
        [Display(Name = "Dirección de Entrega")]
        public string? DireccionEntrega { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado del Pedido")]
        public string Estado { get; set; } = "Completada";

        public ICollection<DetalleVenta>? Detalles { get; set; }
        public ICollection<Cuota>? Cuotas { get; set; }
    }
}
