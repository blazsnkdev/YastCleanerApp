using Microsoft.EntityFrameworkCore;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

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

        public async Task<List<Pedido>> GetPedidosByClienteId(int clienteId)
        {
            return await _appDbContext.TblPedido
                .Where(p => p.ClienteId == clienteId)
                .Include(c => c.Cliente)
                .Include(u => u.Usuario)
                .Include(d => d.DetallePedidos)
                .ThenInclude(d => d.Servicio)
                .ToListAsync();
        }

        public int ContarPedidos(DateTime fecha)
        {
            var inicio = fecha.Date;             // 2025-09-09 00:00:00
            var fin = inicio.AddDays(1);         // 2025-09-10 00:00:00

            return _appDbContext.TblPedido.Count(p => p.FechaRegistro >= inicio && p.FechaRegistro < fin);
        }
        public int ContarPedidosEntregados(DateTime fecha)
        {
            var inicio = fecha.Date;             // 2025-09-09 00:00:00
            var fin = inicio.AddDays(1);         // 2025-09-10 00:00:00
            return _appDbContext.TblPedido.Count(p => p.Estado == EstadoPedido.Entregado && p.FechaRegistro >= inicio && p.FechaRegistro < fin);
        }
        public double SumarMontoTotal(DateTime fecha)
        {
            var inicio = fecha.Date;             // 2025-09-09 00:00:00
            var fin = inicio.AddDays(1);         // 2025-09-10 00:00:00
            return _appDbContext.TblPedido.Where(p => p.Fecha >= inicio && p.FechaRegistro < fin).Sum(p => p.MontoTotal);
        }
        public List<Pedido> GetPedidosRecientes(int cantidad) => _appDbContext.TblPedido.OrderByDescending(p=>p.Fecha).Take(cantidad).ToList();
    }
}
