using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Entities.Entidades
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NumeroCelular { get; set; }
        public string Direccion { get; set; }
        public EstadoCliente Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    }
}
