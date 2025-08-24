using System.ComponentModel.DataAnnotations;

namespace YastCleaner.Web.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Correo Electronico")]
        [Required,EmailAddress]
        public string Email { get; set; }

        [Display(Name ="Contraseña")]
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
