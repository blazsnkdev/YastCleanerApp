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

        public async Task<Result> AnularPedido(int pedidoId, string comentario)
        {
            var pedido = await _UoW.PedidoRepository.GetPedidoById(pedidoId);
            if(pedido is null)
                return Result.Fail("El pedido no existe");
            if(pedido.Estado == EstadoPedido.Entregado)
                return Result.Fail("No se puede anular un pedido que ya fue entregado");
            var pedidoYaFueAnulado = await _UoW.PedidoAnuladoRepository.ExistePedidoAnulado(pedidoId);
            if(pedidoYaFueAnulado)
                return Result.Fail("El pedido ya fue anulado");
            var pedidoAnulado = new PedidoAnulado
            {
                PedidoId = pedidoId,
                Pedido = pedido,
                Comentario = comentario,
                FechaAnulacion = _dateTimeProvider.DateTimeActual(),
            };
            pedidoAnulado.Pedido.Estado = EstadoPedido.Anulado;
            await _UoW.PedidoAnuladoRepository.AddAsync(pedidoAnulado);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result<PedidoDto>> ConsultarPedidoPorCodigo(string codigoPedido)
        {
            if(string.IsNullOrEmpty(codigoPedido) || string.IsNullOrWhiteSpace(codigoPedido))
            {
                return Result<PedidoDto>.Fail("El codigo del pedido es obligatorio");
            }
            var pedido = await _UoW.PedidoRepository.GetPedidoByCodigo(codigoPedido);
            if(pedido is null)
            {
                return Result<PedidoDto>.Fail("El pedido no existe");
            }
            var pedidoDto = new PedidoDto()
            {
                PedidoId = pedido.PedidoId,
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
                    DetallePedidoId = d.DetallePedidoId,
                    PedidoId = d.PedidoId,
                    ServicioId = d.ServicioId,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    SubTotal = d.SubTotal,
                    Servicio = new ServicioDto
                    {
                        ServicioId = d.Servicio.ServicioId,
                        Nombre = d.Servicio.Nombre,
                        Precio = d.Servicio.Precio
                    }
                }).ToList()
            };
            return Result<PedidoDto>.Ok(pedidoDto);
        }

        public async Task<Result<PedidoDto>> DetalleEntregarPedido(int pedidoId)
        {
            var pedido = await _UoW.PedidoRepository.GetPedidoById(pedidoId);
            if (pedido is null)
                return Result<PedidoDto>.Fail("El pedido no existe");
            if (pedido.Estado != EstadoPedido.Entregado)
            {
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
                    Cliente = new ClienteDto
                    {
                        ClienteId = pedido.Cliente.ClienteId,
                        Nombre = pedido.Cliente.Nombre,
                        ApellidoPaterno = pedido.Cliente.ApellidoPaterno,
                        ApellidoMaterno = pedido.Cliente.ApellidoMaterno,
                        Email = pedido.Cliente.Email,
                        NumeroCelular = pedido.Cliente.NumeroCelular
                    },
                    Detalles = pedido.DetallePedidos.Select(d => new DetallePedidoDto()
                    {
                        DetallePedidoId = d.DetallePedidoId,
                        PedidoId = pedido.PedidoId,
                        ServicioId = d.ServicioId,
                        Cantidad = d.Cantidad,
                        Precio = d.PrecioUnitario,
                        SubTotal = d.SubTotal,
                        Servicio = new ServicioDto
                        {
                            ServicioId = d.Servicio.ServicioId,
                            Nombre = d.Servicio.Nombre,
                            Precio = d.Servicio.Precio
                        }
                    }).ToList()
                };
                return Result<PedidoDto>.Ok(pedidoDto);
            }
            return Result<PedidoDto>.Fail("El pedido ya fue entregado");
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
            var pedidos = await _UoW.PedidoRepository.GetAllAsync();
            if (pedidos == null || !pedidos.Any())
            {
                return "P00000001";
            }
            var ultimoCodigo = pedidos.Max(p => p.CodigoPedido);
            int numero = int.Parse(ultimoCodigo.Substring(1)) + 1;
            return "P" + numero.ToString("D8");
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

        public async Task<Result> RegistrarEntrega(EntregaDto pedidoEntregadoDto)
        {
            var pedido = await _UoW.PedidoRepository.GetPedidoById(pedidoEntregadoDto.PedidoId);
            if (pedido is null)
                return Result.Fail("El pedido no existe");

            pedido.Estado = EstadoPedido.Entregado;
            pedido.MontoFaltante = 0;

            var entrega = new PedidoEntregado()
            {
                PedidoId = pedido.PedidoId,
                Pedido = pedido,
                FechaEntrega = _dateTimeProvider.DateTimeActual(),
                Observaciones = pedidoEntregadoDto.Observaciones
            };

            await _UoW.PedidoEntregadoRepository.AddAsync(entrega);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<Result<AnularDto>> DetalleAnularPedido(int pedidoId)
        {
            var pedido = await _UoW.PedidoRepository.GetPedidoById(pedidoId);
            if(pedido is null)
                return Result<AnularDto>.Fail("El pedido no existe");
            if(pedido.Estado != EstadoPedido.Entregado && pedido.Estado != EstadoPedido.Anulado)
            {
                var anuladoDto = new AnularDto
                {
                    PedidoId = pedidoId,
                    CodigoPedido = pedido.CodigoPedido,
                    MontoTotal = pedido.MontoTotal,
                    FechaEntrega = pedido.Fecha,
                    NombreCliente = pedido.Cliente.Nombre,
                    NombreTrabajador = pedido.Usuario.Nombre
                };
                return Result<AnularDto>.Ok(anuladoDto);
            }
            else
            {
                return Result<AnularDto>.Fail("El pedido no se puede anular porque ya fue entregado o anulado.");
            }


        }
        public async Task<Result<int>> RegistrarPedido(PedidoDto pedidoDto)
        {
            try
            {
                var listaPedidoTemporal = _pedidoStorage.RecuperarCarrito();//Recuperamos la lista
                if (!listaPedidoTemporal.Any())
                {
                    return Result<int>.Fail("La lista de servicios seleccionado esta vacío");
                }
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
                    await _UoW.SaveChangesAsync();
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
                Cliente = new ClienteDto
                {
                    ClienteId = pedido.Cliente.ClienteId,
                    Nombre = pedido.Cliente.Nombre,
                    ApellidoPaterno = pedido.Cliente.ApellidoPaterno,
                    ApellidoMaterno = pedido.Cliente.ApellidoMaterno,
                    Email = pedido.Cliente.Email,
                    NumeroCelular = pedido.Cliente.NumeroCelular
                },
                Detalles = pedido.DetallePedidos.Select(d => new DetallePedidoDto()
                {
                    DetallePedidoId = d.DetallePedidoId,
                    PedidoId = pedido.PedidoId,
                    ServicioId = d.ServicioId,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    SubTotal = d.SubTotal,
                    Servicio = new ServicioDto
                    {
                        ServicioId = d.Servicio.ServicioId,
                        Nombre = d.Servicio.Nombre,
                        Precio = d.Servicio.Precio
                    }
                }).ToList()
            };
            return Result<PedidoDto>.Ok(pedidoDto);
        }
        
    }
}
