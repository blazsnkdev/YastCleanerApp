using Microsoft.AspNetCore.Mvc;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
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
        public IActionResult Registrar()
        {
            return View(new InsertarClienteViewModel());
        }

        [HttpPost]
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
            return RedirectToAction("Index", "Home");//TODO: esto a futuro tengo que redirigir a una accion anterior con js
        }
        
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
    }
}
