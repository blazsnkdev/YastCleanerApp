using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Entities.Entidades
{
    public class PedidoAnulado
    {
        [Key]
        public int PedidoAnuladoId { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaAnulacion { get; set; }
    }
}
