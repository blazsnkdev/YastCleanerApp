using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;

namespace YastCleaner.Business.Interfaces
{
    public interface IPedidoStorage
    {
        List<PedidoTemporalDto> RecuperarCarrito();
        void GrabarCarrito();
        void LimpiarCarrito();
    }
}
