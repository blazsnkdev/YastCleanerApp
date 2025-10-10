using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Interfaces
{
    public interface IPedidoRepository : IRepository<Pedido>
    {
        Task<Pedido?> GetPedidoById(int pedidoId);
        Task<IEnumerable<Pedido>> GetAllPedidosByTrabajadorHoy(int trabajadorId, DateTime fecha);
        Task<Pedido?> GetPedidoByCodigo(string codigoPedido);
        Task<List<Pedido>> GetPedidosByClienteId(int clienteId);
        int ContarPedidos(DateTime fecha);
        int ContarPedidosEntregados(DateTime fecha);
        double SumarMontoTotal(DateTime fecha);
        List<Pedido> GetPedidosRecientes(int cantidad);
    }
}
