namespace YastCleaner.Web.ViewModels
{
    public class DetallePedidoViewModel
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public double MontoAdelantado { get; set; }
        public double MontoFaltante { get; set; }
        public double MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public ClienteViewModel Cliente { get; set; }
        public TrabajadorViewModel Trabajador { get; set; }

        public List<DetallePedidoDetalleViewModel> Detalles { get; set; }
    }
}
