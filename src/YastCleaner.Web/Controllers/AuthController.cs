using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel) {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var sesionDto = await _authService.LoginAsync(viewModel.Email, viewModel.Password);
            if(sesionDto is null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas");//TODO :esto a futuro cambiar a un notification o algo asi
                return View(viewModel);
            }
            SessionHelper.SetUsuario(HttpContext, sesionDto);
            if (sesionDto.Rol == Rol.Administrador)//TODO : si agrego otro rol, esto tengo que modificar a un if else
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext); 
            return RedirectToAction("Login", "Auth");
        }


        [Route("acceso-denegado")]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }


        [Route("no-autorizado")]
        public IActionResult UnauthorizedPage()
        {
            return View("UnauthorizedPage");
        }


        [Route("no-encontrado")]
        public IActionResult NotFoundPage()
        {
            return View("NotFoundPage");
        }

    }
}
