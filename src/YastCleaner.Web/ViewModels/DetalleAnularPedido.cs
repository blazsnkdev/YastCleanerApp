namespace YastCleaner.Web.ViewModels
{
    public class DetalleAnularPedido
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public double MontoTotal { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string NombreCliente { get; set; }
        public string NombreTrabajador { get; set; }
    }
}
