using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Models;

namespace YastCleaner.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        public IActionResult Index()
        {
            return View();
        }

        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]//TODO : ejemplo para un action con 2 roles
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("error/{statusCode}")]//TODO : la verdad no se que hace pero me re sirve xd
        public IActionResult Error(int statusCode)
        {
            return statusCode switch
            {
                401 => View("Unauthorized"),
                403 => View("AccessDenied"),
                404 => View("NotFound"),
                _ => View("Error")
            };
        }
    }
}
