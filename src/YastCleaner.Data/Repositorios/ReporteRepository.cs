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
    public class ReporteRepository : Repository<Reporte>, IReporteRepository
    {
        private readonly AppDbContext _appDbContext;
        public ReporteRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<bool> ExisteReporteTrabajadorHoy(int trabajdorId)
        {
            var inicio = DateTime.Now.Date;
            var fin = inicio.AddDays(1);
            return await _appDbContext.TblReporte
                .AnyAsync(r => r.Usuario.UsuarioId == trabajdorId
                && r.FechaReporte >= inicio
                && r.FechaReporte < fin);
                
        }
    }
}
