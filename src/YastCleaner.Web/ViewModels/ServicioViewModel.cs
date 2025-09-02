using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class ServicioViewModel
    {
        [Display(Name ="Id")]
        public int ServicioId { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }= string.Empty;
        [Display(Name = "Precio K/U")]
        public double Precio { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }= string.Empty;
        [Display(Name = "Estado")]
        public string Estado { get; set; }= string.Empty;
        [Display(Name = "Fecha Registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
