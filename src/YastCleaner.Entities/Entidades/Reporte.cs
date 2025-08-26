using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Entities.Entidades
{
    public class Reporte
    {
        [Key]
        public int ReporteId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public double MontoGenerado { get; set; }
        public DateTime FechaReporte { get; set; }
    }
}
