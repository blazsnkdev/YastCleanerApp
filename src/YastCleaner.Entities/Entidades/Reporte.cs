namespace YastCleaner.Entities.Entidades
{
    public class Reporte
    {
        public int ReporteId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public double MontoGenerado { get; set; }
        public DateTime FechaReporte { get; set; }
    }
}
