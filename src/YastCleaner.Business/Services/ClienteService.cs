using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ClienteService(IUnitOfWork uoW, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _dateTimeProvider = dateTimeProvider;
        }
        public async Task<Result<ClienteDto>> ObtenerCliente(int clienteId)
        {
            var cliente = await _UoW.ClienteRepository.GetByIdAsync(clienteId);
            if (cliente is null)
                return Result<ClienteDto>.Fail("El cliente no existe");
            var clienteDto = new ClienteDto()
            {
                ClienteId = clienteId,
                Nombre = cliente.Nombre,
                ApellidoPaterno = cliente.ApellidoPaterno,
                ApellidoMaterno = cliente.ApellidoMaterno,
                NumeroCelular = cliente.NumeroCelular,
                Email = cliente.Email,
                Direccion = cliente.Direccion
            };
            return Result<ClienteDto>.Ok(clienteDto);
        }
        public async Task<List<ClienteDto>> ListarClientes()
        {
            var clientes = await _UoW.ClienteRepository.GetAllAsync();
            return clientes.Select(c => new ClienteDto()
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre
            }).ToList();
        }
        public async Task<Result> CrearCliente(ClienteDto clienteDto)
        {
            //validaciones
            if (string.IsNullOrEmpty(clienteDto.Nombre)
                || string.IsNullOrEmpty(clienteDto.ApellidoMaterno)
                || string.IsNullOrEmpty(clienteDto.ApellidoPaterno)
                || string.IsNullOrEmpty(clienteDto.NumeroCelular)
                || string.IsNullOrEmpty(clienteDto.Direccion)
                || string.IsNullOrEmpty(clienteDto.Email)) 
            {
                return Result.Fail("Todos los campos son obligatorios");
            }
            // Crear el cliente
            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                ApellidoPaterno = clienteDto.ApellidoPaterno,
                ApellidoMaterno = clienteDto.ApellidoMaterno,
                NumeroCelular = clienteDto.NumeroCelular,
                Direccion = clienteDto.Direccion,
                Email = clienteDto.Email,
                Estado = EstadoCliente.Activo,
                FechaRegistro = _dateTimeProvider.DateTimeActual()
            };
            await _UoW.ClienteRepository.AddAsync(cliente);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<List<ClienteDto>> ObtenerClientesActivos()
        {
            var clientesAll = await _UoW.ClienteRepository.GetAllAsync();
            //TODO : 
            var clientesActivos = clientesAll.Where(c => c.Estado == EstadoCliente.Activo).ToList();
            return clientesActivos.Select(c => new ClienteDto()
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre,
                ApellidoPaterno = c.ApellidoPaterno,
                ApellidoMaterno = c.ApellidoMaterno,    
                NumeroCelular = c.NumeroCelular,
                Direccion = c.Direccion,
                Email = c.Email,
                Estado = c.Estado.ToString(),
                FechaRegistro = c.FechaRegistro
            }).ToList();
        }

        public async Task<Result<ClienteDto>> ObtenerDetalleCliente(int clienteId)
        {
            var cliente = await _UoW.ClienteRepository.GetClienteById(clienteId);
            if(cliente is null)
            {
                return Result<ClienteDto>.Fail("El cliente no existe");
            }
            var clienteDto = new ClienteDto()
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                ApellidoPaterno = cliente.ApellidoPaterno,
                ApellidoMaterno = cliente.ApellidoMaterno,
                NumeroCelular = cliente.NumeroCelular,
                Direccion = cliente.Direccion,
                Email = cliente.Email,
                Estado = cliente.Estado.ToString(),
                FechaRegistro = cliente.FechaRegistro,
                Pedidos = cliente.Pedidos.Select(p => new PedidoDto()
                {
                    PedidoId = p.PedidoId,
                    CodigoPedido = p.CodigoPedido,
                    Fecha = p.Fecha,
                    UsuarioId = p.UsuarioId,
                    MontoAdelantado = p.MontoAdelantado,
                    MontoFaltante = p.MontoFaltante,
                    MontoTotal = p.MontoTotal,
                    MetodoPago = p.MetodoPago.ToString(),
                    Estado = p.Estado.ToString(),
                    Trabajador = new TrabajadorDto()
                    {
                        TrabajadorId = p.UsuarioId,
                        Nombre = p.Usuario.Nombre,
                        ApellidoPaterno = p.Usuario.ApellidoPaterno,
                        ApellidoMaterno = p.Usuario.ApellidoMaterno
                    }
                }).ToList()
            };
            return Result<ClienteDto>.Ok(clienteDto);
        }

        public async Task<Result> ActualizarCliente(int clienteId,ClienteDto clienteDto)
        {
            if(clienteId <= 0)
            {
                return Result.Fail("El clienteId es obligatorio");
            }
            var cliente = await _UoW.ClienteRepository.GetByIdAsync(clienteId);
            if(cliente is null)
            {
                return Result.Fail("El cliente no existe");
            }
            try
            {
                cliente.Nombre = clienteDto.Nombre;
                cliente.ApellidoPaterno = clienteDto.ApellidoPaterno;
                cliente.ApellidoMaterno = clienteDto.ApellidoMaterno;
                cliente.NumeroCelular = clienteDto.NumeroCelular;
                cliente.Direccion = clienteDto.Direccion;
                cliente.Email = clienteDto.Email;
                _UoW.ClienteRepository.Update(cliente);
                await _UoW.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error al actualizar el cliente: {ex.Message}");
            }
        }

        public async Task<Result> DesactivarCliente(int clienteId)
        {
            var cliente = await _UoW.ClienteRepository.GetByIdAsync(clienteId);
            if (cliente is null) { 
                return Result.Fail("El cliente no existe");
            }
            try
            {
                cliente.Estado = EstadoCliente.Desactivo;
                _UoW.ClienteRepository.Update(cliente);
                await _UoW.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error al desactivar el cliente: {ex.Message}");
            }
        }

        public async Task<Result<List<PedidoDto>>> ObtenerPedidosCliente(int clienteId)
        {
            if(clienteId <= 0)
            {
                return Result<List<PedidoDto>>.Fail("El clienteId es obligatorio");
            }
            var pedidosCliente = await _UoW.PedidoRepository.GetPedidosByClienteId(clienteId);
            if(pedidosCliente is null)
            {
                return Result<List<PedidoDto>>.Fail("El cliente no tiene pedidos");
            }
            var pedidosClienteDto =  pedidosCliente.Select(pc => new PedidoDto
            {
                PedidoId = pc.PedidoId,
                CodigoPedido = pc.CodigoPedido,
                Fecha = pc.Fecha,
                UsuarioId = pc.UsuarioId,
                MontoAdelantado = pc.MontoAdelantado,
                MontoFaltante = pc.MontoFaltante,
                MontoTotal = pc.MontoTotal,
                MetodoPago = pc.MetodoPago.ToString(),
                Estado = pc.Estado.ToString(),
                Trabajador = new TrabajadorDto()
                {
                    TrabajadorId = pc.UsuarioId,
                    Nombre = pc.Usuario.Nombre,
                    ApellidoPaterno = pc.Usuario.ApellidoPaterno,
                    ApellidoMaterno = pc.Usuario.ApellidoMaterno,
                },
                Cliente = new ClienteDto()
                {
                    ClienteId = pc.ClienteId,
                    Nombre = pc.Cliente.Nombre,
                    ApellidoPaterno = pc.Cliente.ApellidoPaterno,
                    ApellidoMaterno = pc.Cliente.ApellidoMaterno,
                },
                Detalles = pc.DetallePedidos.Select(d => new DetallePedidoDto
                {
                    DetallePedidoId = d.DetallePedidoId,
                    PedidoId = d.PedidoId,
                    ServicioId = d.ServicioId,
                    Servicio = new ServicioDto
                    {
                        ServicioId = d.ServicioId,
                        Nombre = d.Servicio.Nombre,
                        Descripcion = d.Servicio.Descripcion,
                        Precio = d.Servicio.Precio
                    },
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario,
                    SubTotal = d.SubTotal
                }).ToList()

            }).ToList();
            return Result<List<PedidoDto>>.Ok(pedidosClienteDto);
        }
    }
}
