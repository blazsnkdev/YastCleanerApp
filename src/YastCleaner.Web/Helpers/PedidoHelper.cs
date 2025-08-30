using Newtonsoft.Json;
using YastCleaner.Business.DTOs;

namespace YastCleaner.Web.Helpers
{
    public class PedidoHelper
    {
        public static List<PedidoTemporalDto> pedidoTemporal = new List<PedidoTemporalDto>();

        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static List<PedidoTemporalDto> RecuperarCarrito() => pedidoTemporal;
        public static void GrabarCarrito()
        {
            var json = JsonConvert.SerializeObject(pedidoTemporal);
            _httpContextAccessor.HttpContext.Session.SetString("pedidoTemporal", json);
        }
        public static void LimpiarCarrito()
        {
            pedidoTemporal.Clear();
            _httpContextAccessor.HttpContext.Session.Remove("pedidoTemporal");
        }
    }
}
