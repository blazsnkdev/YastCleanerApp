using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class EntregaDto
    {
        public int PedidoEntregadoId { get; set; }
        public int PedidoId { get; set; }
        public PedidoDto Pedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Observaciones { get; set; }
    }
}
