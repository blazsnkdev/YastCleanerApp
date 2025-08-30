using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class PedidoTemporalDto
    {
        public int Id { get; set; }//esto es del servicio
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Importe => Cantidad * Precio;
    }
}
