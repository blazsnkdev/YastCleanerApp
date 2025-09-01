using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;

namespace YastCleaner.Business.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IPedidoStorage _pedidoStorage;
        public ServicioService(IUnitOfWork uoW, IPedidoStorage pedidoStorage)
        {
            _UoW = uoW;
            _pedidoStorage = pedidoStorage;
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


        public async Task<List<ServicioDto>> ListaServicios()//TODO : esta lista es para el trabajador pueda ver los que estan disnponinbles
        {
            var servicios = await _UoW.ServicioRepository.GetAllAsync();
            return servicios.Select(s => new ServicioDto()
            {
                ServicioId = s.ServicioId,
                Nombre = s.Nombre,
                Precio = s.Precio,
                Descripcion = s.Descripcion,
                Estado = s.Estado.ToString(),
            }).ToList();
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
            
    }
}
