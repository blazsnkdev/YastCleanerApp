using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Data.Interfaces;
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

        public PedidoController(IPedidoService pedidoService, IClienteService clienteService, ITrabajadorService trabajadorService)
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _trabajadorService = trabajadorService;
        }
        public IActionResult Temporal()
        {
            var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
            if (pedidosTemporalDto.Count == 0)
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

        public async Task<IActionResult> RegistrarPedido()
        {
            ViewBag.Clientes = new SelectList(await _clienteService.ListarClientes(), "ClienteId","Nombre");//TODO :esto es una lista de clienteDto
            ViewBag.Importe = _pedidoService.ImporteTotalPedido();
            return View(Temporal());
        }

        public async Task<IActionResult> RegistrarPedido(RegistrarPedidoViewModel viewModel)
        {
            //Aqui necesito construir un pedidoDto apartir de un viewModel
            var trabajadorId = SessionHelper.GetUsuarioId(HttpContext);
            if (trabajadorId is null)
                return RedirectToAction("UnauthorizedPage", "Auth");
            var pedidoDto = new PedidoDto()
            {
                ClienteId = viewModel.ClienteId,
                Fecha = viewModel.Fecha,
                UsuarioId = trabajadorId.Value,
                MontoAdelantado = viewModel.MontoAdelantado,
                MetodoPago = viewModel.MetodoPago,
            };
            var result = await _pedidoService.RegistrarPedido(pedidoDto);
            if (!result.Success)
                return View(viewModel);
            return RedirectToAction("DetallePedido", new {pedidoId = result.Value});
        }
        public async Task<IActionResult> DetallePedido(int pedidoId)
        {
            var pedidoDto = await _pedidoService.VerDetallePedido(pedidoId);
            if (pedidoDto.Value is null)
                return View("NotFoundPage", "Auth");

            var pedido = pedidoDto.Value; 
            var clienteDto = await _clienteService.ObtenerCliente(pedido.ClienteId);
            var trabajadorDto = await _trabajadorService.ObtenerTrabajador(pedido.UsuarioId);

            if (pedidoDto.Value is null || clienteDto.Value is null || trabajadorDto is null)
                return View();


            var clienteViewModel = new ClienteViewModel(clienteDto.Value.ClienteId, clienteDto.Value.Nombre);
            var trabajadorViewModel = new TrabajadorViewModel(trabajadorDto.TrabajadorId, trabajadorDto.Nombre, trabajadorDto.ApellidoPaterno, trabajadorDto.Dni, trabajadorDto.Direccion, trabajadorDto.Email);
            var pedidoViewModel = new DetallePedidoViewModel()
            {
                PedidoId = pedidoId,
                Cliente =clienteViewModel,
                Trabajador = trabajadorViewModel,
                Fecha = pedido.Fecha,
                MontoAdelantado = pedido.MontoAdelantado,
                MetodoPago = pedido.MetodoPago,
                MontoFaltante =pedido.MontoFaltante,
                CodigoPedido = pedido.CodigoPedido,
                Estado = pedido.Estado,
                Detalles = pedido.Detalles.Select(d => new DetallePedidoDetalleViewModel()
                {
                    PedidoDetalleId = d.DetallePedidoId,
                    PedidoId = pedidoId,
                    ServicioId = d.ServicioId,
                    Cantidad =d.Cantidad,
                    Precio = d.Precio,
                    SubTotal = d.SubTotal
                }).ToList()
            };
            return View(pedidoViewModel);
        }
    }
}
