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
    public class PedidoEntregadoRepository : Repository<PedidoEntregado>, IPedidoEntregadoRepository
    {
        public PedidoEntregadoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
