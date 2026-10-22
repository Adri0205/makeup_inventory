using MakeupInventory.Models;

namespace MakeupInventory.Services
{
    public interface IVentaService
    {
        Task<IEnumerable<Venta>> ObtenerTodasAsync();
        Task<Venta?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Venta>> ObtenerPorClienteEmailAsync(string email);
        Task<Venta> RegistrarVentaOnlineAsync(CheckoutViewModel model);
        Task<Venta> RegistrarVentaPOSAsync(NuevaVentaPOSViewModel model);
        Task<DashboardViewModel> ObtenerMetricasDashboardAsync();
    }
}
