using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;

namespace YastCleaner.Web.Helpers
{
    public class PedidoSessionStorage : IPedidoStorage
    {
        public void GrabarCarrito() =>PedidoHelper.GrabarCarrito();
        public void LimpiarCarrito() => PedidoHelper.LimpiarCarrito();
        public List<PedidoTemporalDto> RecuperarCarrito() => PedidoHelper.RecuperarCarrito();
    }
}
