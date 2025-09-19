using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class ServicioDetalleDto
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Precio { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        //
        public int CantidadUtilizados { get; set; }
        public double MontoTotalGenerado { get; set; }
    }
}
