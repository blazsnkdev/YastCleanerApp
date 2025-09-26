using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;
        private readonly ITrabajadorService _trabajadorService;
        private readonly IMetodoPagoService _metodoPagoService;
        private readonly IServicioService _servicioService;
        private readonly IEnviarCorreoSmtp _enviarCorreoSmtp;

        public PedidoController(
            IPedidoService pedidoService,
            IClienteService clienteService,
            ITrabajadorService trabajadorService,
            IMetodoPagoService metodoPagoService,
            IServicioService servicioService,
            IEnviarCorreoSmtp enviarCorreoSmtp)
        {
            _pedidoService = pedidoService;
            _clienteService = clienteService;
            _trabajadorService = trabajadorService;
            _metodoPagoService = metodoPagoService;
            _servicioService = servicioService;
            _enviarCorreoSmtp = enviarCorreoSmtp;
        }

        [RoleAuthorize(Rol.Trabajador)]
        public IActionResult Temporal()
        {
            var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
            if (pedidosTemporalDto == null || !pedidosTemporalDto.Any())
            {
                TempData["Error"] = "El carrito esta vacío";
                return RedirectToAction("Servicios", "Servicio");
            }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPedido(int clienteId,DateTime fecha,double montoAdelanto, string metodoPago)
        {
            try
            {
                var trabajadorId = SessionHelper.GetUsuarioId(HttpContext);
                if (trabajadorId is null)
                {
                    return RedirectToAction("UnauthorizedPage", "Auth");
                }
                var pedidosTemporalDto = _pedidoService.ObtenerPedidosTemporal();
                if (pedidosTemporalDto is null || !pedidosTemporalDto.Any())
                {
                    TempData["Error"] = "No hay servicios en el carrito";
                    return RedirectToAction("Servicios", "Servicio");
                }
                double totalPagar = pedidosTemporalDto.Sum(p => p.Cantidad * p.Precio);
                if(montoAdelanto> totalPagar)
                {
                    TempData["Error"] = "El monto adelantado no puede ser mayor";
                    return RedirectToAction("RegistrarPedido");
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
                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction("RegistrarPedido");
                }
                var email = await _clienteService.RecuperarEmailCliente(clienteId);
                if(email.Value is not null)
                {
                    _enviarCorreoSmtp.RegistroPedido(email.Value);
                }
                return RedirectToAction("DetallePedido", new { pedidoId = result.Value });
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "No tiene acceso";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("RegistrarPedido");
            }
        }
        //Ruido de Mate....
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

            var clienteViewModel = new ClienteViewModel
            {
                ClienteId = pedidoDto.Value.Cliente.ClienteId,
                Nombre = pedidoDto.Value.Cliente.Nombre,
                ApellidoPaterno = pedidoDto.Value.Cliente.ApellidoPaterno,
                ApellidoMaterno = pedidoDto.Value.Cliente.ApellidoMaterno,
                NumeroCelular = pedidoDto.Value.Cliente.NumeroCelular,
                Email = pedidoDto.Value.Cliente.Email,
                Direccion = pedidoDto.Value.Cliente.Direccion,
                Estado = pedidoDto.Value.Cliente.Estado,
                FechaRegistro = pedidoDto.Value.Fecha
            };

            var trabajadorViewModel = new TrabajadorViewModel()
            {
                TrabajadorId = trabajadorDto.TrabajadorId,
                Nombre = trabajadorDto.Nombre,
                Apellidos = trabajadorDto.ApellidoPaterno + " " + trabajadorDto.ApellidoMaterno,
                Dni = trabajadorDto.Dni,
                Direccion = trabajadorDto.Direccion,
                Email = trabajadorDto.Email,
                FechaRegistro = trabajadorDto.FechaRegistro
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
            ViewBag.Importe = _pedidoService.ImporteTotalPedido();
            ViewBag.MetodosPago = new SelectList(await _metodoPagoService.ListarMetodosPago());
        }
        
        public async Task<IActionResult> Consultar(string codigoPedido)
        {
            try
            {
                var result = await _pedidoService.ConsultarPedidoPorCodigo(codigoPedido);
                if (result.Success && result.Value is not null)
                {
                    return RedirectToAction(nameof(DetallePedido), new { pedidoId = result.Value.PedidoId });   
                }
                    return RedirectToAction("NotFoundPage", "Auth");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
        public IActionResult Buscar()
        {
            return View();
        }

        [RoleAuthorize(Rol.Trabajador)]
        public async Task<IActionResult> Entregar(int pedidoId)
        {
            try
            {
                var pedido = await _pedidoService.DetalleEntregarPedido(pedidoId);
                if (!pedido.Success)
                {
                    TempData["Mensaje"] = $"{pedido.ErrorMessage}";
                    return RedirectToAction("DetallePedido", "Pedido", new { pedidoId = pedidoId });
                }
                if (pedido.Value is null)
                    return RedirectToAction("NotFoundPage", "Auth");
                var detalleEntregaViewModel = new DetalleEntregarPedidoViewModel
                {
                    PedidoId = pedidoId,
                    CodigoPedido = pedido.Value.CodigoPedido,
                    Fecha = pedido.Value.Fecha,
                    NombreCliente = pedido.Value.Cliente.Nombre,
                    MontoAdelantado = pedido.Value.MontoAdelantado,
                    MontoFaltante = pedido.Value.MontoFaltante,
                    MontoTotal = pedido.Value.MontoTotal,
                    MetodoPago = pedido.Value.MetodoPago,
                    Estado = pedido.Value.Estado,
                    Observaciones = string.Empty
                };
                ViewBag.MetodosPago = new SelectList(await _metodoPagoService.ListarMetodosPago());
                return View(detalleEntregaViewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Rol.Trabajador)]
        public async Task<IActionResult> EntregarPedido(int pedidoId, string metodoPago, string observaciones)
        {
            try
            {
                var pedido = await _pedidoService.VerDetallePedido(pedidoId);
                if (!pedido.Success)
                    return RedirectToAction("BadRequest", "Auth");
                if (pedido.Value is null)
                    return RedirectToAction("NotFoundPage", "Auth");

                var pedidoEntregadoDto = new EntregaDto()
                {
                    PedidoId = pedido.Value.PedidoId,
                    Pedido = pedido.Value,
                    Observaciones = observaciones,
                };
                var result = await _pedidoService.RegistrarEntrega(pedidoEntregadoDto);
                if (!result.Success)
                    return RedirectToAction("BadRequest", "Auth");
                TempData["Mensaje"] = $"El pedido {pedidoEntregadoDto.Pedido.CodigoPedido} se ha entregado correctamente.";
                return View("Confirmacion", "Pedido");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Anular(int pedidoId)
        {
            try
            {
                var detallePedidoAnulado = await _pedidoService.DetalleAnularPedido(pedidoId);
                if (!detallePedidoAnulado.Success)
                {
                    TempData["Mensaje"] = $"{detallePedidoAnulado.ErrorMessage}";
                    return RedirectToAction("DetallePedido", "Pedido", new { pedidoId = pedidoId });
                }
                if (detallePedidoAnulado.Value is null)
                    return RedirectToAction("NotFoundPage", "Auth");
                var anularViewModel = new DetalleAnularPedido
                {
                    PedidoId = detallePedidoAnulado.Value.PedidoId,
                    CodigoPedido = detallePedidoAnulado.Value.CodigoPedido,
                    NombreCliente = detallePedidoAnulado.Value.NombreCliente,
                    NombreTrabajador = detallePedidoAnulado.Value.NombreTrabajador,
                    MontoTotal = detallePedidoAnulado.Value.MontoTotal,
                    FechaEntrega = detallePedidoAnulado.Value.FechaEntrega
                };
                return View(anularViewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> AnularPedido(int pedidoId, string comentario)
        {
            try
            {
                var result = await _pedidoService.AnularPedido(pedidoId, comentario);
                if (!result.Success)
                {
                    TempData["Mensaje"] = $"{result.ErrorMessage}";
                    return RedirectToAction("DetallePedido", "Pedido", new { pedidoId = pedidoId });
                }
                TempData["Mensaje"] = "El pedido se ha anulado correctamente.";
                return RedirectToAction("DetallePedido", "Pedido", new { pedidoId = pedidoId });
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [ValidateAntiForgeryToken]
        [RoleAuthorize(Rol.Trabajador)]
        public IActionResult Confirmacion()
        {
            return View();
        }
        public IActionResult Limpiar()
        {
            var result = _pedidoService.LimpiarCarrito();
            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View("Temporal");
            }
            return RedirectToAction("Servicios","Servicio");
        }
    }
}
