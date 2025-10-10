using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;

namespace YastCleaner.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public AdminController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [RoleAuthorize(Rol.Administrador)]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Pedidos-Hoy")]
        public IActionResult PedidosDashboard()
        {
            var data = _dashboardService.GetDashboardHoy();
            return Json(data);
        }

        [RoleAuthorize(Rol.Administrador)]
        public IActionResult Trabajadores()
        {
            return View();
        }
    }
}
