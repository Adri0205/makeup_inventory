using MakeupInventory.Models;
using MakeupInventory.Repositories;

namespace MakeupInventory.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IRepository<Categoria> _categoriaRepository;

        public CategoriaService(IRepository<Categoria> categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task CrearAsync(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new InvalidOperationException("El nombre es obligatorio.");

            await _categoriaRepository.AddAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new InvalidOperationException("El nombre es obligatorio.");

            _categoriaRepository.Update(categoria);
            await _categoriaRepository.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                throw new InvalidOperationException("La categoría no existe.");

            _categoriaRepository.Remove(categoria);
            await _categoriaRepository.SaveChangesAsync();
        }
    }
}