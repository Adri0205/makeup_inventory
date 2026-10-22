using MakeupInventory.Models;

namespace MakeupInventory.Services
{
    public interface ICarritoService
    {
        List<CarritoItem> ObtenerCarrito();
        void AgregarItem(Producto producto, int cantidad = 1);
        void ActualizarCantidad(int productoId, int cantidad);
        void EliminarItem(int productoId);
        void LimpiarCarrito();
        int ObtenerCantidadTotal();
        decimal ObtenerSubtotal();
    }
}
