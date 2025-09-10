namespace YastCleaner.Web.ViewModels
{
    public class PedidoViewModel
    {
        public int PedidoId { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public int ClienteId { get; set; }
        public double MontoAdelantado { get; set; }
        public double MontoFaltante { get; set; }
        public double MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }

        // Relación con Trabajador
        public TrabajadorViewModel Trabajador { get; set; }
        public ClienteViewModel Cliente { get; set; }
    }
}
