using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Entities.Entidades
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public double MontoAdelantado { get; set; }
        public double Vuelto { get; set; }
        public double MontoTotal { get; set; }
        public MetodoPago MetodoPago { get; set; }  
        public EstadoPedido Estado { get; set; }


        public ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();


        public PedidoEntregado PedidoEntregado { get; set; }
        public PedidoAnulado PedidoAnulado { get; set; }
    }
}
