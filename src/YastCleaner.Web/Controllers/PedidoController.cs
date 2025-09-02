using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class PedidoController : Controller
    {
        //TODO: este controlador va manejar el general del recurso de pediso
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly ITrabajadorService _trabajadorService;
        private readonly IMetodoPagoService _metodoPagoService;
        private readonly IServicioService _servicioService;

        public PedidoController(
            IPedidoService pedidoService,
            IClienteService clienteService,
            ITrabajadorService trabajadorService,
            IMetodoPagoService metodoPagoService,
            IServicioService servicioService)
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _trabajadorService = trabajadorService;
            _metodoPagoService = metodoPagoService;
            _servicioService = servicioService;
        }

        [RoleAuthorize(Rol.Trabajador)]
        public IActionResult Temporal()
        {
            var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
            if (pedidosTemporalDto.Count == 0 || pedidosTemporalDto is null || !pedidosTemporalDto.Any())
                return RedirectToAction("Servicios","Servicio");
            var pedidoTemporalViewModel = pedidosTemporalDto.Select(p=> new PedidosTemporalViewModel()
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Cantidad = p.Cantidad,
                Precio = p.Precio
            });
            ViewBag.Importe = _pedidoService.ImporteTotalPedido();
            return View(pedidoTemporalViewModel);
        }
        [HttpPost]
        [RoleAuthorize(Rol.Trabajador)]
        public IActionResult EliminarServicio(int servicioId)
        {
            var result = _pedidoService.EliminarServicioDelPedido(servicioId);
            if (!result.Success)
                return View("Temporal");
            return RedirectToAction("Temporal");
        }
        [HttpPost]
        [RoleAuthorize(Rol.Trabajador)]
        public IActionResult ModificarCantidad(int servicioId, int cantidad)
        {
            var result = _pedidoService.ModificarCantidadServicioDelPedido(servicioId,cantidad);
            if (!result.Success)
                return View("Temporal");
            return RedirectToAction("Temporal");
        }

        [RoleAuthorize(Rol.Trabajador)]
        public async Task<IActionResult> RegistrarPedido()
        {
            await CargarCombos();

            var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
            if (pedidosTemporalDto is null || !pedidosTemporalDto.Any())
            {
                //await CargarCombos();//TODO : esto ah modificar, si en caso no hay lista no debe haber esto
                return RedirectToAction("Servicios", "Servicio");
            }
            var pedidoTemporalViewModel = pedidosTemporalDto.Select(p => new PedidosTemporalViewModel()
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Cantidad = p.Cantidad,
                Precio = p.Precio
            });
            return View(pedidoTemporalViewModel);
        }
        [HttpPost]
        [RoleAuthorize(Rol.Trabajador)]
        public async Task<IActionResult> RegistrarPedido(int clienteId,DateTime fecha,double montoAdelanto, string metodoPago)
        {
            //Aqui necesito construir un pedidoDto apartir de un viewModel
            var trabajadorId = SessionHelper.GetUsuarioId(HttpContext);
            if (trabajadorId is null)
                return RedirectToAction("UnauthorizedPage", "Auth");

            var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
            if(pedidosTemporalDto is null || !pedidosTemporalDto.Any())
            {
                return RedirectToAction("Servicios", "Servicio");
            }

            var pedidoDto = new PedidoDto()
            {
                ClienteId = clienteId,
                Fecha = fecha,
                UsuarioId = trabajadorId.Value,
                MontoAdelantado = montoAdelanto,
                MetodoPago = metodoPago,
                Detalles = pedidosTemporalDto.Select(p => new DetallePedidoDto()
                {
                    ServicioId = p.Id,
                    Cantidad = p.Cantidad,
                    Precio = p.Precio,
                    SubTotal = p.Cantidad * p.Precio
                }).ToList()
            };

            var result = await _pedidoService.RegistrarPedido(pedidoDto);
            if (!result.Success)
            {
                await CargarCombos();
                return View(pedidosTemporalDto);
            }
            return RedirectToAction("DetallePedido", new {pedidoId = result.Value});
        }

        [RoleAuthorize(Rol.Administrador,Rol.Trabajador)]
        public async Task<IActionResult> DetallePedido(int pedidoId)
        {
            var pedidoDto = await _pedidoService.VerDetallePedido(pedidoId);
            if (pedidoDto.Value is null)
                return RedirectToAction("NotFoundPage", "Auth");

            var pedido = pedidoDto.Value; 
            var clienteDto = await _clienteService.ObtenerCliente(pedido.ClienteId);
            var trabajadorDto = await _trabajadorService.ObtenerTrabajador(pedido.UsuarioId);
            
            if (pedidoDto.Value is null || clienteDto.Value is null || trabajadorDto is null)
                return View();

            var clienteViewModel = new ClienteViewModel(clienteDto.Value.ClienteId, clienteDto.Value.Nombre);

            var trabajadorViewModel = new TrabajadorViewModel()
            {
                TrabajadorId = trabajadorDto.TrabajadorId,
                Nombre = trabajadorDto.Nombre,
                Apellidos = trabajadorDto.ApellidoPaterno + " " + trabajadorDto.ApellidoMaterno,
                Dni = trabajadorDto.Dni,
                Direccion = trabajadorDto.Direccion,
                Email = trabajadorDto.Email
            };
            var detallePedidoViewModel = new DetallePedidoViewModel()
            {
                PedidoId = pedidoId,
                Cliente = clienteViewModel,
                Trabajador = trabajadorViewModel,
                Fecha = pedido.Fecha,
                MontoAdelantado = pedido.MontoAdelantado,
                MetodoPago = pedido.MetodoPago,
                MontoFaltante = pedido.MontoFaltante,
                CodigoPedido = pedido.CodigoPedido,
                Estado = pedido.Estado,
                Detalles = pedido.Detalles.Select(d => new DetallePedidoDetalleViewModel()
                {
                    PedidoDetalleId = d.DetallePedidoId,
                    PedidoId = pedidoId,
                    ServicioId = d.ServicioId,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    SubTotal = d.SubTotal,
                    NombreServicio = d.Servicio.Nombre
                }).ToList()
            };
            return View(detallePedidoViewModel);
        }

        private async Task CargarCombos()//TODO : aqui este metodo privado carga todos los combos de selectList para el registro de pedido
        {
            ViewBag.Clientes = new SelectList(await _clienteService.ListarClientes(), "ClienteId", "Nombre");
            ViewBag.Importe = _pedidoService.ImporteTotalPedido();
            ViewBag.MetodosPago = new SelectList(await _metodoPagoService.ListarMetodosPago());
        }
        
    }
}
