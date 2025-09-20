using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class EditarServicioViewModel
    {
        public int ServicioId { get; set; }

        [Display(Name = "Nombre"),
            Required(ErrorMessage = "El campo nombre es requerido"),
            StringLength(20, ErrorMessage = "El campo tiene límite de 20 carateres")]
        public string Nombre { get; set; } = string.Empty;
        [Display(Name = "Precio K/U"),
            Required(ErrorMessage = "El campo del precio es requerido"),
            Range(0.1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public double Precio { get; set; }
        [Display(Name = "Descripción"),
            Required(ErrorMessage = "El campo descripción es requerido"),
            StringLength(50, ErrorMessage = "El campo tiene límite de 20 carateres")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
