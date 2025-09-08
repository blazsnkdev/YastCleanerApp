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
            return View(new RegistrarClienteViewModel());
        }

        [HttpPost]
        public IActionResult Registrar(RegistrarClienteViewModel viewModel)
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
            return RedirectToAction("Index", "Home");
        }
    }
}
