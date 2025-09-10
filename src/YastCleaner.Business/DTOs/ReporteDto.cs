using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class ReporteDto
    {
        public int ReporteId { get; set; }
        public int TrabajadorId { get; set; }
        public TrabajadorDto Trabajador { get; set; }
        public double MontoGenerado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
