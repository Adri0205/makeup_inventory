using System.ComponentModel.DataAnnotations;

namespace MakeupInventory.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string ClienteNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida")]
        [StringLength(150)]
        [Display(Name = "Correo Electrónico")]
        public string ClienteEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un número telefónico válido")]
        [StringLength(50)]
        [Display(Name = "Teléfono de Contacto")]
        public string ClienteTelefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de entrega es obligatoria")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres")]
        [Display(Name = "Dirección de Envío y Referencias")]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un método de pago")]
        [Display(Name = "Método de Pago")]
        public string MetodoPago { get; set; } = "Tarjeta de Crédito / Débito";

        public List<CarritoItem> Items { get; set; } = new();

        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal ImpuestoIVA => Subtotal * 0.13m; // IVA El Salvador 13%
        public decimal Total => Subtotal + ImpuestoIVA;
    }
}
