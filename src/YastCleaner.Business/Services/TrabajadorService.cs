using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IAuthService _authService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TrabajadorService(IUnitOfWork uoW, IAuthService authService, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _authService = authService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<TrabajadorDto>> ListaTrabajadores()
        {
            var trabajadores = await _UoW.UsuarioRepository.GetAllByRolTrabajador();
            return trabajadores.Select(t => new TrabajadorDto
            {
                TrabajadorId = t.UsuarioId,
                Nombre = t.Nombre,
                ApellidoPaterno = t.ApellidoPaterno,
                ApellidoMaterno = t.ApellidoMaterno,
                Dni = t.Dni,
                Direccion = t.Direccion,
                Email = t.Email,
                Password = t.Password,
                FechaRegistro = t.FechaRegistro
            }).ToList();
        }

        public async Task<Result> RegistrarTrabajador(TrabajadorDto dto)
        {
            var existe = await _UoW.UsuarioRepository.UsuarioDniExiste(dto.Dni);
            if (existe)
                return Result.Fail("El dni Ya existe");
            string encriptado = _authService.HashPassword(dto.Password);//esto ya llega del viewModel
            var trabajador = new Usuario()
            {
                UsuarioId = dto.TrabajadorId,
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Dni = dto.Dni,
                Direccion = dto.Direccion,
                Email = dto.Email,
                Password = encriptado,
                Rol = Rol.Trabajador,
                FechaRegistro = _dateTimeProvider.DateTimeActual()//asignar el trabajador
            };
            await _UoW.UsuarioRepository.AddAsync(trabajador);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }
        public string GenerarPassword(string apellidoPaterno)
        {
            var fechaHoraActual = _dateTimeProvider.DateTimeActual().ToString("yyyyMMddHHmmss");
            string parteApePat = !string.IsNullOrEmpty(apellidoPaterno) ? apellidoPaterno.Substring(0, 1) : "";
            Random rnd = new Random();
            string numerosAleatorios = rnd.Next(100, 999).ToString();
            return $"{parteApePat}{numerosAleatorios}{fechaHoraActual}";
        }

        public async Task<Result> ActualizarTrabajador(TrabajadorDto dto)
        {
            if (dto.TrabajadorId <= 0)
                return Result.Fail("El id es invalido");
            var seleccionado = await _UoW.UsuarioRepository.GetByIdAsync(dto.TrabajadorId);
            if (seleccionado is null)
                return Result.Fail("El trabajador no existe");
            try
            {
                seleccionado.Nombre = dto.Nombre;
                seleccionado.ApellidoPaterno = dto.ApellidoPaterno;
                seleccionado.ApellidoMaterno = dto.ApellidoMaterno;
                seleccionado.Dni = dto.Dni;
                seleccionado.Direccion = dto.Direccion;
                seleccionado.Email = dto.Email;
                _UoW.UsuarioRepository.Update(seleccionado);
                await _UoW.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex) 
            {
                return Result.Fail($"{ex}");
            }
        }

        public async Task<TrabajadorDto?> ObtenerTrabajador(int trabajadorId)
        {
            var trabajador = await _UoW.UsuarioRepository.GetByIdUsuario(trabajadorId);
            if (trabajador is null)
                return null;
            return new TrabajadorDto()
            {
                TrabajadorId = trabajador.UsuarioId,
                Nombre = trabajador.Nombre,
                ApellidoPaterno = trabajador.ApellidoPaterno,
                ApellidoMaterno = trabajador.ApellidoMaterno,
                Dni = trabajador.Dni,
                Direccion = trabajador.Direccion,
                Email = trabajador.Email,
                FechaRegistro = trabajador.FechaRegistro
            };
        }

        public async Task<Result> EliminarTrabajador(int trabajadorId)
        {
            if(trabajadorId <=0)
                return Result.Fail("id Invalido");
            var trabajador = await _UoW.UsuarioRepository.GetByIdAsync(trabajadorId);
            if (trabajador is null)
                return Result.Fail("el usuario es null");
            try
            {
                _UoW.UsuarioRepository.Delete(trabajador);
                await _UoW.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return Result.Fail("Error al actualizar");
            }
            
        }

        public async Task<Result<List<PedidoDto>>> PedidosPorTrabajador(int trabajadorId)
        {
            var pedidos = await _UoW.PedidoRepository.GetAllAsync();
            var pedidosPorTrabajador = pedidos.Where(p => p.UsuarioId == trabajadorId).ToList();
            var pedidosPorTrabajadorDto = pedidosPorTrabajador.Select(p => new PedidoDto
            {
                PedidoId = p.PedidoId,
                CodigoPedido = p.CodigoPedido,
                Fecha = p.Fecha,
                ClienteId = p.ClienteId,
                UsuarioId = p.UsuarioId,
                MontoAdelantado = p.MontoAdelantado,
                MontoFaltante = p.MontoFaltante,
                MontoTotal = p.MontoTotal,
                MetodoPago = p.MetodoPago.ToString(),
                Estado = p.Estado.ToString(),
            }).ToList();
            return Result<List<PedidoDto>>.Ok(pedidosPorTrabajadorDto);
        }

        public async Task<Result<List<TrabajadorDto>>> TrabajadoresConPedidosHoy()
        {
            var pedidosHoy = await _UoW.UsuarioRepository.GetAllByRolTrabajadorPedidosHoy(_dateTimeProvider.DateTimeActual().Date);
            if (!pedidosHoy.Any())
            {
                return Result<List<TrabajadorDto>>.Fail("No hay trabajadores con pedidos hoy");
            }
            var trabajadoresPedidoHoyDto = pedidosHoy.Select(t => new TrabajadorDto
            {
                TrabajadorId = t.UsuarioId,
                Nombre = t.Nombre,
                ApellidoPaterno = t.ApellidoPaterno,
                ApellidoMaterno = t.ApellidoMaterno,
                Dni = t.Dni,
                Direccion = t.Direccion,
                Email = t.Email,
                FechaRegistro = t.FechaRegistro
            }).ToList();
            return Result<List<TrabajadorDto>>.Ok(trabajadoresPedidoHoyDto);
        }

        public async Task<Result<List<PedidoDto>>> PedidosPorTrabajadorHoy(int trabajadorId)
        {
            var trabajador = await _UoW.UsuarioRepository.GetByIdAsync(trabajadorId);
            if(trabajador is null)
            {
                return Result<List<PedidoDto>>.Fail("El trabajador no existe");
            }
            var pedidosHoy = await _UoW.PedidoRepository.GetAllPedidosByTrabajadorHoy(trabajadorId,_dateTimeProvider.DateTimeActual().Date);
            if (!pedidosHoy.Any())
            {
                return Result<List<PedidoDto>>.Ok(new List<PedidoDto>());//TODO: solucion temporal
            }
            var pedidosHoyDto = pedidosHoy.Select(p => new PedidoDto
            {
                PedidoId = p.PedidoId,
                CodigoPedido = p.CodigoPedido,
                Fecha = p.Fecha,
                ClienteId = p.ClienteId,
                MontoAdelantado = p.MontoAdelantado,
                MontoFaltante = p.MontoFaltante,
                MontoTotal = p.MontoTotal,
                MetodoPago = p.MetodoPago.ToString(),
                Estado = p.Estado.ToString(),
                Cliente = new ClienteDto()
                {
                    ClienteId = p.Cliente.ClienteId,
                    Nombre = p.Cliente.Nombre,
                    ApellidoPaterno = p.Cliente.ApellidoPaterno,
                    ApellidoMaterno = p.Cliente.ApellidoMaterno
                },
                Trabajador = new TrabajadorDto()
                {
                    TrabajadorId = p.UsuarioId,
                    Nombre = trabajador.Nombre,
                    ApellidoPaterno = trabajador.ApellidoPaterno,
                    ApellidoMaterno = trabajador.ApellidoMaterno
                }
            }).ToList();

            return Result<List<PedidoDto>>.Ok(pedidosHoyDto);
        }


    }
}
