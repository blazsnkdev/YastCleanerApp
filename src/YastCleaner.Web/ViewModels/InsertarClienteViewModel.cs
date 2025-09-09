using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class InsertarClienteViewModel
    {
        public int ClienteId { get; set; }
        [Display(Name ="Nombre Completo"),
            Required(ErrorMessage ="Este campo es obligatorio"),
            StringLength(30, ErrorMessage = "No puede superar los 30 caracteres"),
            RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$",ErrorMessage = "El campo solo debe contener letras y espacios.")]
        public string Nombre { get; set; }
        [Display(Name ="Apellido Paterno"),
            Required(ErrorMessage = "Este campo es obligatorio"),
            StringLength(30, ErrorMessage = "No puede superar los 30 caracteres"),
            RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", ErrorMessage = "El campo solo debe contener letras y espacios.")]
        public string ApellidoPaterno { get; set; }
        [Display(Name = "Apellido Materno"),
            Required(ErrorMessage = "Este campo es obligatorio"),
            StringLength(30, ErrorMessage = "No puede superar los 30 caracteres"),
            RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$", ErrorMessage = "El campo solo debe contener letras y espacios.")]
        public string ApellidoMaterno { get; set; }
        [Display(Name ="Número de Celular"),
            Required(ErrorMessage = "Este campo es obligatorio"),
            RegularExpression(@"^\d{9}$", ErrorMessage = "El campo debe tener exactamente 9 dígitos.")]
        public string NumeroCelular { get; set; }
        [Display(Name ="Dirección"),
            Required(ErrorMessage = "Este campo es obligatorio"),
            StringLength(100, ErrorMessage = "No puede superar los 100 caracteres")]
        public string Direccion { get; set; }
        [Display(Name = "Correo Electrónico"),
            Required(ErrorMessage = "Este campo es obligatorio"),
            EmailAddress(ErrorMessage = "Debe ser un correo electrónico válido"),
            StringLength(50, ErrorMessage = "No puede superar los 50 caracteres")]
        public string Email { get; set; }
    }
}
