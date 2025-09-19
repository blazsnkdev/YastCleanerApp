using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Interfaces
{
    public interface IServicioRepository :IRepository<Servicio>
    {
        Task<Servicio?> GetByNameAsync(string nombre);
        Task<bool> ValidarNombreServicioDisponibleAsync(string nombre);
        Task DesactivarServicio(int servicioId);
        Task ActivarServicio(int servicioId);
        Task<string?> GetEstadoById(int servicioId);
    }
}
