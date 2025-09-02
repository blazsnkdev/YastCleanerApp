namespace YastCleaner.Business.DTOs
{
    public class DetallePedidoDto
    {
        public int DetallePedidoId { get; set; }
        public int PedidoId { get; set; }
        public int ServicioId { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
        public ServicioDto Servicio { get; set; }
    }
}
