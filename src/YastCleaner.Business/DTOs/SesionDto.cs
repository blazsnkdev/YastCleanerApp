using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.DTOs
{
    public class SesionDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public Rol Rol { get; set; }
    }
}
