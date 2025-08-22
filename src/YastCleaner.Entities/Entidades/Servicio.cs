using YastCleaner.Entities.Enums;

namespace YastCleaner.Entities.Entidades
{
    public class Servicio
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public string Descripcion { get; set; }
        public EstadoServicio Estado { get; set; }
        public DateTime FechaRegistro { get; set; }

        public ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();


    }
}
