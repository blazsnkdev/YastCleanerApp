using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.DTOs
{
    public class ClienteDto
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NumeroCelular { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        //Añadido para relacionar con pedidos
        public List<PedidoDto> Pedidos { get; set; } = new List<PedidoDto>();
    }
}
