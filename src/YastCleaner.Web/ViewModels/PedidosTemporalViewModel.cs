using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class PedidosTemporalViewModel
    {
        public int Id { get; set; }
        [Display(Name ="Servicio")]
        public string Nombre { get; set; }
        [Display(Name = "Cantidad K/U")]
        public int Cantidad { get; set; }
        [Display(Name ="Precio")]
        public double Precio { get; set; }
        public double Importe => Cantidad * Precio;
    }
}
