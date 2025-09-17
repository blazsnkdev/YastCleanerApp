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
    public class PedidoAnuladoRepository : Repository<PedidoAnulado>, IPedidoAnuladoRepository
    {
        private readonly AppDbContext _appDbContext;
        public PedidoAnuladoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> ExistePedidoAnulado(int pedidoAnuladoId) => await _appDbContext.TblPedidoAnulado.AnyAsync(p=>p.Pedido.PedidoId ==pedidoAnuladoId);
        
    }
}
