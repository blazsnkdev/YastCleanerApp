using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Repositorios
{
    public class PedidoRepository : Repository<Pedido>, IPedidoRepository
    {
        private readonly AppDbContext _appDbContext;
        public PedidoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Pedido?> GetPedidoById(int pedidoId)
        {
            var pedido = await _appDbContext.TblPedido
                .Where(p => p.PedidoId == pedidoId)
                .Include(c => c.Cliente)
                .Include(u =>u.Usuario)
                .Include(d=>d.DetallePedidos)
                .ThenInclude(d => d.Servicio)
                .FirstOrDefaultAsync();

            return pedido;
        }
    }
}
