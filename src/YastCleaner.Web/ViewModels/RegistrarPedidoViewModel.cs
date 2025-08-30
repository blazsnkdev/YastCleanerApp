namespace YastCleaner.Web.ViewModels
{
    public class RegistrarPedidoViewModel
    {
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public double MontoAdelantado { get; set; }
        public string MetodoPago { get; set; }
    }
}
