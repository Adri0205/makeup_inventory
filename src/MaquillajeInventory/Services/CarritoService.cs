using System.Text.Json;
using MakeupInventory.Models;

namespace MakeupInventory.Services
{
    public class CarritoService : ICarritoService
    {
        private const string CartSessionKey = "_MakeupCartSessionKey";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarritoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public List<CarritoItem> ObtenerCarrito()
        {
            if (Session == null) return new List<CarritoItem>();

            var json = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<CarritoItem>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
            }
            catch
            {
                return new List<CarritoItem>();
            }
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            if (Session == null) return;
            var json = JsonSerializer.Serialize(carrito);
            Session.SetString(CartSessionKey, json);
        }

        public void AgregarItem(Producto producto, int cantidad = 1)
        {
            if (cantidad <= 0 || producto.Stock <= 0) return;

            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoId == producto.Id);

            if (item != null)
            {
                var nuevaCantidad = item.Cantidad + cantidad;
                item.Cantidad = Math.Min(nuevaCantidad, producto.Stock);
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = Math.Min(cantidad, producto.Stock),
                    ImagenUrl = producto.ImagenUrl,
                    CategoriaNombre = producto.Categoria?.Nombre ?? "Cosméticos",
                    StockDisponible = producto.Stock
                });
            }

            GuardarCarrito(carrito);
        }

        public void ActualizarCantidad(int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoId == productoId);

            if (item != null)
            {
                if (cantidad <= 0)
                {
                    carrito.Remove(item);
                }
                else
                {
                    item.Cantidad = Math.Min(cantidad, item.StockDisponible);
                }
                GuardarCarrito(carrito);
            }
        }

        public void EliminarItem(int productoId)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.ProductoId == productoId);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }
        }

        public void LimpiarCarrito()
        {
            if (Session == null) return;
            Session.Remove(CartSessionKey);
        }

        public int ObtenerCantidadTotal()
        {
            return ObtenerCarrito().Sum(i => i.Cantidad);
        }

        public decimal ObtenerSubtotal()
        {
            return ObtenerCarrito().Sum(i => i.Subtotal);
        }
    }
}
