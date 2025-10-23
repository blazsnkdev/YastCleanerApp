using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Models;

namespace YastCleaner.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger,IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        public IActionResult Index()
        {
            return View();
        }
        //[HttpGet("Pedidos-Hoy")]
        [HttpGet]
        public IActionResult PedidosDashboard()
        {
            var data = _dashboardService.GetDashboardHoy();
            return Json(data);
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
