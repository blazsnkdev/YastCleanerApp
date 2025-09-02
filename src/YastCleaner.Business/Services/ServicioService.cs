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
    public class ServicioService : IServicioService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IPedidoStorage _pedidoStorage;
        private readonly IDateTimeProvider _dateTimeProvider;
        public ServicioService(IUnitOfWork uoW, IPedidoStorage pedidoStorage, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _pedidoStorage = pedidoStorage;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> AgregarServicioAlPedido(int servicioId, int cantidad)
        {
            var servicioSeleccionado = await _UoW.ServicioRepository.GetByIdAsync(servicioId);
            if (servicioSeleccionado is null)
                return Result.Fail("El servicio seleccionado no existe");
            var objPedido = new PedidoTemporalDto()
            {
                Id = servicioId,
                Nombre = servicioSeleccionado.Nombre,
                Cantidad = cantidad,
                Precio = servicioSeleccionado.Precio,
            };
            var listaPedidosTemporal = _pedidoStorage.RecuperarCarrito();
            var servicioSeleccionadoTemporal = listaPedidosTemporal.FirstOrDefault(p => p.Id == servicioId);
            if (servicioSeleccionadoTemporal is null)
            {
                listaPedidosTemporal.Add(objPedido);
            }
            else
            {
                servicioSeleccionadoTemporal.Cantidad += cantidad;
            }
            _pedidoStorage.GrabarCarrito();
            return Result.Ok();
        }

        public async Task<Result> DesactivarServicio(int servicioId)
        {
            var existeServicio = await _UoW.ServicioRepository.GetByIdAsync(servicioId);
            if(existeServicio is null)
                return Result.Fail("El registro no existe");
            existeServicio.Estado = EstadoServicio.Inactivo;
            _UoW.ServicioRepository.Update(existeServicio);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> EditarServicio(int servicioId,ServicioDto servicioDto)
        {
            var existeServicio = await _UoW.ServicioRepository.GetByIdAsync(servicioId);
            if (existeServicio is null)
            {
                return Result.Fail("El registro no existe");
            }
            if (string.IsNullOrWhiteSpace(servicioDto.Nombre)
                || string.IsNullOrEmpty(servicioDto.Descripcion)
                || servicioDto.Precio < 0)
            {
                return Result.Fail("Las propiedades vienen vacias");
            }
            existeServicio.Nombre = servicioDto.Nombre;
            existeServicio.Descripcion = servicioDto.Descripcion;
            existeServicio.Precio = servicioDto.Precio;
            if (Enum.TryParse<EstadoServicio>(servicioDto.Estado, out var estadoServicio))//TODO: el viewmodel debe enviar el estado de servicio como string
            {
                existeServicio.Estado = estadoServicio;
            }
            else
            {
                return Result.Fail("El estado del servicio no es válido");
            }
            _UoW.ServicioRepository.Update(existeServicio);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }

        public Task<List<EstadoServicio>> ListarEstadoServicios() => Task.FromResult(Enum.GetValues<EstadoServicio>().ToList());

        public async Task<List<ServicioDto>> ListaServiciosDisponibles()//TODO : esta lista es para el trabajador pueda ver los que estan disnponinbles
        {
            var servicios = await _UoW.ServicioRepository.GetAllAsync();
            return servicios
                .Where(s => s.Estado == EstadoServicio.Disponible)
                .Select(s => new ServicioDto()
            {
                ServicioId = s.ServicioId,
                Nombre = s.Nombre,
                Precio = s.Precio,
                Descripcion = s.Descripcion,
                Estado = s.Estado.ToString(),
            }).ToList();
        }

        public Task<List<ServicioDto>> ListaServiciosInactivos()
        {
            throw new NotImplementedException();
        }

        public async Task<ServicioDto?> ObtenerServicio(int servicioId)
        {
            var servicio = await _UoW.ServicioRepository.GetByIdAsync(servicioId);
            if(servicio is null)
                return null;//TODO : cambiar el objeto result para que devuelva data tipo clase, evitar los nulls
            var servicioDto = new ServicioDto()
            {
                ServicioId = servicio.ServicioId,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                Estado = servicio.Estado.ToString(),
                FechaRegistro = servicio.FechaRegistro,
            };
            return servicioDto;
        }

        public async Task<Result> RegistrarServicio(ServicioDto servicioDto)
        {
            if (string.IsNullOrWhiteSpace(servicioDto.Nombre)
                || string.IsNullOrEmpty(servicioDto.Descripcion)
                || servicioDto.Precio < 0)
            {
                return Result.Fail("Error en las propiedades nombre, descripción, precio");
            }
            var existeServicio = await _UoW.ServicioRepository.GetByNameAsync(servicioDto.Nombre);
            if(existeServicio is not null)
            {
                return Result.Fail("El servicio ya existe");
            }
            var nuevoServicio = new Servicio
            {
                Nombre = servicioDto.Nombre,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio,
                Estado = EstadoServicio.Disponible,
                FechaRegistro = _dateTimeProvider.DateTimeActual(),
            };
            await _UoW.ServicioRepository.AddAsync(nuevoServicio);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
