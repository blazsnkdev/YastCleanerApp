using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;

namespace YastCleaner.Business.Interfaces
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> ListaServicios();
        Task<ServicioDto?> ObtenerServicio(int servicioId);
        Task<Result> AgregarServicioAlPedido(int servicioId, int cantidad);
    }
}
