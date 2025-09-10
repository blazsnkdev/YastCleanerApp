namespace YastCleaner.Web.ViewModels
{
    public class DetalleReporteViewModel
    {
        public int TrabajadorId { get; set; }
        public TrabajadorViewModel Trabajador { get; set; }
        public double MontoTotal { get; set; }
        public DateTime FechaReporte { get; set; }
        //public List<DetallePedidoDetalleViewModel> PedidosDetalle { get; set; }
    }
}
