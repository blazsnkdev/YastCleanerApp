using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;

namespace YastCleaner.Web.Controllers
{
    public class AdminController : Controller
    {
        [RoleAuthorize(Rol.Administrador)]
        public IActionResult Index()
        {
            return View();
        }

        [RoleAuthorize(Rol.Administrador)]
        public IActionResult Trabajadores()
        {
            return View();
        }
    }
}
