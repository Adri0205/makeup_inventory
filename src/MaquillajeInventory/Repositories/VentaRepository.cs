using MakeupInventory.Data;
using MakeupInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Repositories
{
    public class VentaRepository : Repository<Venta>, IVentaRepository
    {
        public VentaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Venta>> GetVentasConDetallesAsync()
        {
            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<Venta?> GetVentaConDetallesPorIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Venta>> GetVentasPorEmailClienteAsync(string email)
        {
            return await _context.Ventas
                .Include(v => v.Detalles!)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.ClienteEmail != null && v.ClienteEmail.ToLower() == email.Trim().ToLower())
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }
    }
}
