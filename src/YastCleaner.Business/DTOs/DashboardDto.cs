namespace YastCleaner.Business.DTOs
{
    public class DashboardDto
    {
        public int TotalPedidos { get; set; }
        public int TotalPedidosEntregados { get; set; }
        public double MontoTotal { get; set; }
        public List<PedidoDto> PedidosRecientes { get; set; } = new List<PedidoDto>();
    }
}
