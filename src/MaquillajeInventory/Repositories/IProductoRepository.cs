using MakeupInventory.Models;

namespace MakeupInventory.Repositories
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetProductosConCategoriaAsync();
        Task<IEnumerable<Producto>> GetProductosStockBajoAsync(int umbral);
        Task<Producto?> GetProductoConDetallesAsync(int id);
    }
}