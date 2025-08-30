using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IPedidoStorage _pedidoStorage;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PedidoService(IUnitOfWork uow, IPedidoStorage pedidoStorage,IDateTimeProvider dateTimeProvider)
        {
            _UoW = uow;
            _pedidoStorage = pedidoStorage;
            _dateTimeProvider = dateTimeProvider;
        }
        public Result EliminarServicioDelPedido(int servicioId)
        {
            try
            {
                var pedidosTemporal = _pedidoStorage.RecuperarCarrito();
                pedidosTemporal.RemoveAll(s => s.Id == servicioId);
                _pedidoStorage.GrabarCarrito();
                return Result.Ok();
            }
            catch (Exception ex) {
                return Result.Fail(ex.ToString());
            }           
        }

        public async Task<string> GenerarCodigoPedido()
        {
            var ultimoPedido = await _UoW.PedidoRepository.GetAllAsync();
            var ultimoList = ultimoPedido.ToList().Max(p=>p.CodigoPedido);
            int numero = int.Parse(ultimoList.Substring(1)) + 1;
            return "P" + numero.ToString("0000");
        }

        public double ImporteTotalPedido()
        {
            var pedidosTemporal = _pedidoStorage.RecuperarCarrito();
            return pedidosTemporal.Sum(p => p.Importe);
        }

        public Result ModificarCantidadServicioDelPedido(int servicioId, int cantidad)
        {
            var pedidosTemporal = _pedidoStorage.RecuperarCarrito();
            var servicioSeleccionado = pedidosTemporal.FirstOrDefault(s => s.Id == servicioId);
            if (servicioSeleccionado is null)
                return Result.Fail("El servicio no existe");
            servicioSeleccionado.Cantidad = cantidad;
            _pedidoStorage.GrabarCarrito();
            return Result.Ok();
        }

        public List<PedidoTemporalDto> ObtenerPedidosTemporal()
        {
            var pedidosTemporal = _pedidoStorage.RecuperarCarrito();
            return pedidosTemporal.Select(p => new PedidoTemporalDto()
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Cantidad = p.Cantidad,
                Precio = p.Precio
            }).ToList();
        }

        public async Task<Result<int>> RegistrarPedido(PedidoDto pedidoDto) //TODO: esto debe ser un dto
        {
            try
            {
                var listaPedidoTemporal = _pedidoStorage.RecuperarCarrito();
                if (!listaPedidoTemporal.Any())
                    return Result<int>.Fail("La lista esta vacia");

                double montoPagar = listaPedidoTemporal.Sum(x => x.Importe);
                string codigoPedido = await GenerarCodigoPedido();
                double faltante = montoPagar - pedidoDto.MontoAdelantado;
                var nuevoPedido = new Pedido()
                {
                    CodigoPedido = codigoPedido,
                    Fecha = pedidoDto.Fecha,
                    ClienteId = pedidoDto.ClienteId,
                    UsuarioId = pedidoDto.UsuarioId,
                    MontoTotal = montoPagar,
                    MontoAdelantado = pedidoDto.MontoAdelantado,
                    MontoFaltante = faltante,//esto seria 
                    MetodoPago = (MetodoPago)Enum.Parse(typeof(MetodoPago),pedidoDto.MetodoPago),//TODO : conversion
                    Estado = faltante == 0 ? EstadoPedido.Pagado : EstadoPedido.Pendiente, 
                    FechaRegistro = _dateTimeProvider.DateTimeActual()
                };
                await _UoW.PedidoRepository.AddAsync(nuevoPedido);
                await _UoW.SaveChangesAsync();
                foreach (var item in listaPedidoTemporal)
                {
                    var pedidoDetalle = new DetallePedido()
                    {
                        PedidoId =nuevoPedido.PedidoId,
                        CodigoPedido = codigoPedido,
                        ServicioId = item.Id,
                        Cantidad =item.Cantidad,
                        PrecioUnitario = item.Precio,
                        SubTotal = item.Importe
                    };
                    await _UoW.PedidoDetalleRepository.AddAsync(pedidoDetalle);
                }
                _pedidoStorage.LimpiarCarrito();
                return Result<int>.Ok(nuevoPedido.PedidoId);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.ToString());
            }
            
        }

        public async Task<Result<PedidoDto>> VerDetallePedido(int pedidoId)
        {
            var pedido = await _UoW.PedidoRepository.GetPedidoById(pedidoId);
            if (pedido is null)
                return Result<PedidoDto>.Fail("El pedido no existe");
            var pedidoDto = new PedidoDto()
            {
                PedidoId = pedidoId,
                CodigoPedido = pedido.CodigoPedido,
                Fecha = pedido.Fecha,
                ClienteId = pedido.ClienteId,
                UsuarioId = pedido.UsuarioId,
                MontoAdelantado = pedido.MontoAdelantado,
                MontoFaltante = pedido.MontoFaltante,
                MontoTotal = pedido.MontoTotal,
                MetodoPago = pedido.MetodoPago.ToString(),
                Estado = pedido.Estado.ToString(),
                Detalles = pedido.DetallePedidos.Select(d => new DetallePedidoDto()
                {
                    DetallePedidoId =d.DetallePedidoId,
                    PedidoId =pedido.PedidoId,
                    ServicioId = d.ServicioId,
                    Cantidad =d.Cantidad,
                    Precio = d.PrecioUnitario,
                    SubTotal =d.SubTotal
                }).ToList()
            };
            return Result<PedidoDto>.Ok(pedidoDto);
        }
    }
}
