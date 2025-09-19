using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Data.Repositorios
{
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly AppDbContext _appDbContext;
        public ServicioRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task ActivarServicio(int servicioId)
        {
            var servicio = new Servicio { ServicioId = servicioId };
            _appDbContext.Attach(servicio);
            _appDbContext.Entry(servicio).Property(s => s.Estado).CurrentValue = EstadoServicio.Disponible;
            _appDbContext.Entry(servicio).Property(s => s.Estado).IsModified = true;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DesactivarServicio(int servicioId)
        {
            var servicio = new Servicio { ServicioId = servicioId };
            _appDbContext.Attach(servicio);
            _appDbContext.Entry(servicio).Property(s => s.Estado).CurrentValue = EstadoServicio.Inactivo;
            _appDbContext.Entry(servicio).Property(s => s.Estado).IsModified = true;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<string?> GetEstadoById(int servicioId)
        {
            var estado = await _appDbContext.TblServicio
                .Where(s => s.ServicioId == servicioId)
                .Select(s => s.Estado)
                .FirstOrDefaultAsync();

            return estado.ToString();
        }

        public async Task<Servicio?> GetByNameAsync(string nombre)
        {
            return await _appDbContext.TblServicio.Where(s => s.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task<bool> ValidarNombreServicioDisponibleAsync(string nombre)
        {
            return !await _appDbContext.TblServicio.AnyAsync(s=>s.Nombre == nombre);
        }

        public async Task<Servicio?> GetServicioById(int servicioId)
        {
            return await _appDbContext.TblServicio
                .Where(s => s.ServicioId == servicioId)
                .Include(s => s.DetallePedidos)
                .FirstOrDefaultAsync();
        }
        
    }
}
