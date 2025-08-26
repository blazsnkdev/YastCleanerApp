using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Entities.Entidades
{
    public class PedidoEntregado
    {
        [Key]
        public int PedidoEntregadoId { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        
        public string Observaciones { get; set; }

        //TODO : al momento de realizar el caso de uso EntregarPedido, se debe actualizar el metodoPago.
    }
}
