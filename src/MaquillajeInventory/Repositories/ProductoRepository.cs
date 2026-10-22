using MakeupInventory.Data;
using MakeupInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Repositories
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public ProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetProductosConCategoriaAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosStockBajoAsync(int umbral)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock <= umbral)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }

        public async Task<Producto?> GetProductoConDetallesAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.DetallesVenta)
                .Include(p => p.Movimientos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}