using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Entities.Entidades
{
    public class DetallePedido
    {
        [Key]
        public int DetallePedidoId { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public string CodigoPedido { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double SubTotal { get; set; }

    }
}
