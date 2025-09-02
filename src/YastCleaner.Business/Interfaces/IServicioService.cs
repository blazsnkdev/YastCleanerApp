using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Interfaces
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> ListaServiciosDisponibles();
        Task<List<ServicioDto>> ListaServiciosInactivos();
        Task<Result> RegistrarServicio(ServicioDto servicioDto);
        Task<Result> EditarServicio(int servicioId,ServicioDto servicioDto);
        Task<ServicioDto?> ObtenerServicio(int servicioId);
        Task<Result> AgregarServicioAlPedido(int servicioId, int cantidad);
        Task<List<EstadoServicio>> ListarEstadoServicios();
        //Task<Result> EliminarServicio(int servicioId);
        Task<Result> DesactivarServicio(int servicioId);
    }
}
