using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class TrabajadorDto//NOTE : aqui es el modelo para la vista que se va mostrar en la tabla
    {
        public int TrabajadorId { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Dni { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
