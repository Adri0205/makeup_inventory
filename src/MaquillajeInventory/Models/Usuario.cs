using System.ComponentModel.DataAnnotations;

namespace MakeupInventory.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string Rol { get; set; } = "Vendedor";

        public ICollection<Venta>? Ventas { get; set; }
        public ICollection<MovimientoInventario>? Movimientos { get; set; }
    }
}