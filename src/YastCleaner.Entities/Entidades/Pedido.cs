using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Entities.Entidades
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }// TODO : agregar entidad cliente para la fk
        public Cliente Cliente { get; set; }
        public int UsuarioId { get; set; } // TODO : agregar la entidad usuario para la fk
        public Usuario Usuario { get; set; }
        public double MontoAdelantado { get; set; }
        public double Vuelto { get; set; }
        public double MontoTotal { get; set; }
        public MetodoPago MetodoPago { get; set; }  
        public EstadoPedido Estado { get; set; }


        public ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();


        //TODO : esta parte es para tener la relacion 1 : 0
        public PedidoEntregado PedidoEntregado { get; set; }
        public PedidoAnulado PedidoAnulado { get; set; }
    }
}
