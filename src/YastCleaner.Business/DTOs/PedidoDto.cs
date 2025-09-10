using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.DTOs
{
    public class PedidoDto
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int UsuarioId { get; set; }
        public TrabajadorDto Trabajador { get; set; }
        //public Usuario Usuario { get; set; }
        public double MontoAdelantado { get; set; }
        public double MontoFaltante { get; set; }
        public double MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }

        public List<DetallePedidoDto> Detalles { get; set; } = new();
    }
}
