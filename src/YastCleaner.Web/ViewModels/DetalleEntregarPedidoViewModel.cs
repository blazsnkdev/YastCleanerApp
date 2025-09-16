namespace YastCleaner.Web.ViewModels
{
    public class DetalleEntregarPedidoViewModel
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCliente { get; set; }
        public double MontoAdelantado { get; set; }
        public double MontoFaltante { get; set; }
        public double MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
}