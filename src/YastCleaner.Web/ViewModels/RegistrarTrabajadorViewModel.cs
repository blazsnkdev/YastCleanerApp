using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class RegistrarTrabajadorViewModel
    {
        [Display(Name ="Nombre"),
            Required(ErrorMessage ="Requerido"),
            StringLength(30,ErrorMessage ="No puede superar los 30 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Apellido Paterno"),
            Required(ErrorMessage ="Apellido Paterno Requerido"),
            StringLength(30,ErrorMessage ="No puede superar los 30 caracteres")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Display(Name = "Apellido Materno"),
            Required(ErrorMessage ="Apellido Materno Requerido"),
            StringLength(30, ErrorMessage = "No puede superar los 30 caracteres")]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [Display(Name = "Dni"),
            Required(ErrorMessage ="El dni es requerido"),
            StringLength(8, MinimumLength = 7, ErrorMessage = "El Dni debe tener entre 7 y 8 dígitos")]
        public string Dni { get; set; } = string.Empty;

        [Display(Name = "Dirección"),
            Required(ErrorMessage ="La dirección es requida"),
            StringLength(30,ErrorMessage ="No puede superar los 30 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Correo Electronico"),
            Required(ErrorMessage ="El correo electronico es requerido"),
            EmailAddress(ErrorMessage ="Correo invalido")]
        public string Email { get; set; } =string.Empty;
    }
}
