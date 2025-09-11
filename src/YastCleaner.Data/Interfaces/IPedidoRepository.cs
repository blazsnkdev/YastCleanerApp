using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Interfaces
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido?> GetPedidoById(int pedidoId);
        Task<IEnumerable<Pedido>> GetAllPedidosByTrabajadorHoy(int trabajadorId, DateTime fecha);
        Task<Pedido?> GetPedidoByCodigo(string codigoPedido);
    }
}
