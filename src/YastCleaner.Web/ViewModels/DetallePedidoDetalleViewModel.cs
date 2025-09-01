namespace YastCleaner.Web.ViewModels
{
    public class DetallePedidoDetalleViewModel
    {
        public int PedidoDetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ServicioId { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double SubTotal { get; set; }
        public ServicioViewModel Servicio { get; set; }
    }
}
