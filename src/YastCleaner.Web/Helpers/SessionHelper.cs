using YastCleaner.Business.DTOs;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Web.Helpers
{
    public static class SessionHelper
    {
        private const string KeyUsuarioId = "UsuarioId";
        private const string KeyNombre = "Nombre";
        private const string KeyRol = "Rol";

        public static void SetUsuario(HttpContext context, SesionDto sesionDto)
        {
            context.Session.SetInt32(KeyUsuarioId, sesionDto.UsuarioId);
            context.Session.SetString(KeyNombre, sesionDto.Nombre);
            context.Session.SetInt32(KeyRol, (int)sesionDto.Rol);
        }
        public static SesionDto? GetUsuario(HttpContext context)
        {
            var usuarioId = context.Session.GetInt32("UsuarioId");
            var nombre = context.Session.GetString("Nombre");
            var rolInt = context.Session.GetInt32("Rol");

            if (!usuarioId.HasValue || string.IsNullOrEmpty(nombre) || !rolInt.HasValue)
                return null;

            return new SesionDto
            {
                UsuarioId = usuarioId.Value,
                Nombre = nombre,
                Rol = (Rol)rolInt.Value
            };
        }


        public static int? GetUsuarioId(HttpContext context)
            => context.Session.GetInt32(KeyUsuarioId);

        public static string? GetNombre(HttpContext context)
            => context.Session.GetString(KeyNombre);

        public static Rol? GetRol(HttpContext context)
        {
            var rolInt = context.Session.GetInt32(KeyRol);
            return rolInt.HasValue ? (Rol)rolInt.Value : null;
        }

        public static void Clear(HttpContext context)
        {
            context.Session.Clear();
        }
    }
}
