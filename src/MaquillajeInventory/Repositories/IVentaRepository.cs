using MakeupInventory.Models;

namespace MakeupInventory.Repositories
{
    public interface IVentaRepository : IRepository<Venta>
    {
        Task<IEnumerable<Venta>> GetVentasConDetallesAsync();
        Task<Venta?> GetVentaConDetallesPorIdAsync(int id);
        Task<IEnumerable<Venta>> GetVentasPorEmailClienteAsync(string email);
    }
}
