using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Repositorios
{
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly AppDbContext _appDbContext;
        public ServicioRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Servicio?> GetByNameAsync(string nombre)
        {
            return await _appDbContext.TblServicio.Where(s => s.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task<bool> ValidarNombreServicioDisponibleAsync(string nombre)
        {
            return !await _appDbContext.TblServicio.AnyAsync(s=>s.Nombre == nombre);
        }
    }
}
