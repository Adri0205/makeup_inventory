using MakeupInventory.Models;
using MakeupInventory.Repositories;

namespace MakeupInventory.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _productoRepository.GetAllAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Producto>> ObtenerConCategoriaAsync()
        {
            return await _productoRepository.GetProductosConCategoriaAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerStockBajoAsync(int umbral = 5)
        {
            return await _productoRepository.GetProductosStockBajoAsync(umbral);
        }

        public async Task CrearAsync(Producto producto)
        {
            if (producto.Precio <= 0)
                throw new InvalidOperationException("El precio debe ser mayor a cero.");

            if (producto.Stock < 0)
                throw new InvalidOperationException("El stock no puede ser negativo.");

            producto.FechaIngreso = DateTime.Now;
            await _productoRepository.AddAsync(producto);
            await _productoRepository.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Producto producto)
        {
            if (producto.Precio <= 0)
                throw new InvalidOperationException("El precio debe ser mayor a cero.");

            if (producto.Stock < 0)
                throw new InvalidOperationException("El stock no puede ser negativo.");

            _productoRepository.Update(producto);
            await _productoRepository.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null)
                throw new InvalidOperationException("El producto no existe.");

            _productoRepository.Remove(producto);
            await _productoRepository.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            return producto != null;
        }
    }
}