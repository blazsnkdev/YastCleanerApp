using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;

namespace YastCleaner.Business.Interfaces
{
    public interface IPedidoService
    {
        Task<string> GenerarCodigoPedido();
        Task<Result<int>> RegistrarPedido(PedidoDto pedidoDto);//cambie esto XD
        double ImporteTotalPedido();
        Result EliminarServicioDelPedido(int servicioId);
        Result ModificarCantidadServicioDelPedido(int servicioId,int cantidad);
        List<PedidoTemporalDto> ObtenerPedidosTemporal();
        Task<Result<PedidoDto>> VerDetallePedido(int pedidoId);
    }
}
