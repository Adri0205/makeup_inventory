using System.ComponentModel.DataAnnotations;

namespace MakeupInventory.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la clienta es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre de la Clienta")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Teléfono / WhatsApp")]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        public ICollection<Venta>? Ventas { get; set; }
        public ICollection<Cuota>? Cuotas { get; set; }
    }
}
