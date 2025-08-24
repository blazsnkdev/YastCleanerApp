using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Helpers;

namespace YastCleaner.Web.Filters
{
    public class RoleAuthorizeAttribute : TypeFilterAttribute
    {
        public RoleAuthorizeAttribute(params Rol[] roles) : base(typeof(RoleAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }

        private class RoleAuthorizeFilter : IAuthorizationFilter
        {
            private readonly Rol[] _roles;
            public RoleAuthorizeFilter(Rol[] roles) => _roles = roles;

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var sesion = SessionHelper.GetUsuario(context.HttpContext);

                // Caso 1: Usuario NO autenticado → Redirigir al Login
                if (sesion == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                    return;
                }


                if (!_roles.Contains(sesion.Rol))
                {
                    context.Result = new RedirectResult("/acceso-denegado");
                    return;
                }

            }
        }
    }

}
