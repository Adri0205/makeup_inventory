using MakeupInventory.Models;

namespace MakeupInventory.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Producto>> ObtenerConCategoriaAsync();
        Task<IEnumerable<Producto>> ObtenerStockBajoAsync(int umbral = 5);
        Task CrearAsync(Producto producto);
        Task ActualizarAsync(Producto producto);
        Task EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
    }
}