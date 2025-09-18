using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }
        [RoleAuthorize(Rol.Administrador,Rol.Trabajador)]
        public async Task<IActionResult> Activos(int pagina = 1, int tamanioPagina = 10)
        {
            var clientesActivosDto = await _clienteService.ObtenerClientesActivos();
            var viewModel = clientesActivosDto.Select(c => new ClienteViewModel()
            {
                ClienteId = c.ClienteId,
                Nombre =c.Nombre,
                ApellidoPaterno = c.ApellidoPaterno,
                ApellidoMaterno = c.ApellidoMaterno,
                NumeroCelular = c.NumeroCelular,
                Direccion = c.Direccion,
                Email= c.Email,
                Estado = c.Estado,
                FechaRegistro = c.FechaRegistro
            });
            var paginacion = PaginacionHelper.Paginacion(viewModel, pagina, tamanioPagina);
            return View(paginacion);
        }
        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        public IActionResult Registrar()
        {
            return View(new InsertarClienteViewModel());
        }
        [HttpPost]
        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(InsertarClienteViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
               return View(viewModel);
            }
            
            var clienteDto = new ClienteDto
            {
                Nombre = viewModel.Nombre,
                ApellidoPaterno = viewModel.ApellidoPaterno,
                ApellidoMaterno = viewModel.ApellidoMaterno,
                NumeroCelular = viewModel.NumeroCelular,
                Direccion = viewModel.Direccion,
                Email = viewModel.Email
            };
            var result = _clienteService.CrearCliente(clienteDto).Result;
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(viewModel);
            }
            return RedirectToAction("Activos", "Cliente");//TODO: esto a futuro tengo que redirigir a una accion anterior con js
        }
        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        public async Task<IActionResult> Detalle(int clienteId)
        {
            var result = await _clienteService.ObtenerDetalleCliente(clienteId);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            var viewModel = new ClienteViewModel
            {
                ClienteId = result.Value!.ClienteId,
                Nombre = result.Value.Nombre,
                ApellidoPaterno = result.Value.ApellidoPaterno,
                ApellidoMaterno = result.Value.ApellidoMaterno,
                NumeroCelular = result.Value.NumeroCelular,
                Direccion = result.Value.Direccion,
                Email = result.Value.Email,
                Estado = result.Value.Estado,
                FechaRegistro = result.Value.FechaRegistro,
                Pedidos = result.Value.Pedidos.Select(p => new PedidoViewModel
                {
                    PedidoId = p.PedidoId,
                    CodigoPedido = p.CodigoPedido,
                    Fecha = p.Fecha,
                    UsuarioId = p.UsuarioId,
                    MontoAdelantado = p.MontoAdelantado,
                    MontoFaltante = p.MontoFaltante,
                    MontoTotal = p.MontoTotal,
                    MetodoPago = p.MetodoPago,
                    Estado = p.Estado,
                    Trabajador = new TrabajadorViewModel
                    {
                        TrabajadorId = p.Trabajador.TrabajadorId,
                        Nombre = p.Trabajador.Nombre,
                        Apellidos = p.Trabajador.ApellidoPaterno+" "+p.Trabajador.ApellidoMaterno,
                    }
                }).ToList()
            };

            return View(viewModel);
        }
        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        public async Task<IActionResult> Actualizar(int clienteId)
        {
            var clienteDto = await _clienteService.ObtenerCliente(clienteId);
            if (!clienteDto.Success || clienteDto.Value is null)
            {
                return RedirectToAction("NotFoundPage","Auth");
            }
            var clienteViewModel = new InsertarClienteViewModel
            {
                Nombre = clienteDto.Value.Nombre,
                ApellidoPaterno = clienteDto.Value.ApellidoPaterno,
                ApellidoMaterno = clienteDto.Value.ApellidoMaterno,
                NumeroCelular = clienteDto.Value.NumeroCelular,
                Direccion = clienteDto.Value.Direccion,
                Email = clienteDto.Value.Email
            };
            return View(clienteViewModel);
        }
        [HttpPost]
        [RoleAuthorize(Rol.Administrador, Rol.Trabajador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(InsertarClienteViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var clienteDto = new ClienteDto
            {
                Nombre = viewModel.Nombre,
                ApellidoPaterno = viewModel.ApellidoPaterno,
                ApellidoMaterno = viewModel.ApellidoMaterno,
                NumeroCelular = viewModel.NumeroCelular,
                Direccion = viewModel.Direccion,
                Email = viewModel.Email
            };
            var result = await _clienteService.ActualizarCliente(viewModel.ClienteId, clienteDto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return View(viewModel);
            }
            return RedirectToAction("Index", "Home");//TODO: esto a futuro tengo que redirigir a una accion anterior con js
        }

        [HttpPost]
        [RoleAuthorize(Rol.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int clienteId)
        {
            try
            {
                var result = await _clienteService.DesactivarCliente(clienteId);
                if (!result.Success)
                {
                    TempData["Error"] = $"Error: {result.ErrorMessage}";//TODO: Notificaciones
                    return RedirectToAction("Activos", "Cliente");
                }
                ViewData["Mensaje"] = "Cliente desactivado correctamente";//TODO: esto lo tengo que manejar con js
                return RedirectToAction("Detalle", new { clienteId = clienteId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";//TODO: Notificaciones
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
            
        }
        [RoleAuthorize(Rol.Administrador,Rol.Trabajador)]
        public async Task<IActionResult> Pedidos(int clienteId, int pagina = 1,int tamanioPagina = 10)
        {
            try
            {
                var result = await _clienteService.ObtenerPedidosCliente(clienteId);
                if (!result.Success)
                {
                    TempData["Error"] = $"Error: {result.ErrorMessage}";  
                    return RedirectToAction(nameof(Detalle), new { clienteId});
                }
                var pedidosViewModel = result.Value!.Select(p => new PedidoViewModel
                {
                    PedidoId = p.PedidoId,
                    CodigoPedido = p.CodigoPedido,
                    Fecha = p.Fecha,
                    UsuarioId = p.UsuarioId,
                    MontoAdelantado = p.MontoAdelantado,
                    MontoFaltante = p.MontoFaltante,
                    MontoTotal = p.MontoTotal,
                    MetodoPago = p.MetodoPago,
                    Estado = p.Estado,
                    Trabajador = new TrabajadorViewModel
                    {
                        TrabajadorId = p.Trabajador.TrabajadorId,
                        Nombre = p.Trabajador.Nombre,
                        Apellidos = p.Trabajador.ApellidoPaterno + " " + p.Trabajador.ApellidoMaterno,
                    }
                }).ToList();
                var paginacion = PaginacionHelper.Paginacion(pedidosViewModel, pagina, tamanioPagina);
                return View(paginacion);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
    }
}
