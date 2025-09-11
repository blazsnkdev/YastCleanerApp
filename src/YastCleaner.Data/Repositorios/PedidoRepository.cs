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

        public async Task<IEnumerable<Pedido>> GetAllPedidosByTrabajadorHoy(int trabajadorId, DateTime fecha)
        {
            var inicio = fecha.Date;             // 2025-09-09 00:00:00
            var fin = inicio.AddDays(1);         // 2025-09-10 00:00:00

            return await _appDbContext.TblPedido
                .Where(p => p.UsuarioId == trabajadorId
                         && p.FechaRegistro >= inicio
                         && p.FechaRegistro < fin)
                .Include(c => c.Cliente)
                .ToListAsync();
        }

        public async Task<Pedido?> GetPedidoByCodigo(string codigoPedido)
        {
            return await _appDbContext.TblPedido
                .Where(p => p.CodigoPedido == codigoPedido)
                .Include(c => c.Cliente)
                .Include(u => u.Usuario)
                .Include(d => d.DetallePedidos)
                .ThenInclude(d => d.Servicio)
                .FirstOrDefaultAsync();
        }
    }
}
