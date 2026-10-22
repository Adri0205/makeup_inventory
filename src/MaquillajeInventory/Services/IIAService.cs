using MakeupInventory.Models;

namespace MakeupInventory.Services
{
    public interface IIAService
    {
        Task<RecomendacionIAResponse> ObtenerRecomendacionClienteAsync(ConsultaIARequest request);
        Task<ReporteIAViewModel> GenerarReporteInventarioIAAsync();
    }
}
