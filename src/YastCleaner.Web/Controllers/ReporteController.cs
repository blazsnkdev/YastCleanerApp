using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ITrabajadorService _trabajadorService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReporteController(ITrabajadorService trabajadorService, IDateTimeProvider dateTimeProvider)
        {
            _trabajadorService = trabajadorService;
            _dateTimeProvider = dateTimeProvider;
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> CerrarCaja(int pagina = 1, int tamanioPagina = 10)//este es el total
        {
            try
            {
                var trabajadoresDto = await _trabajadorService.TrabajadoresConPedidosHoy();
                if (trabajadoresDto.Value is null)
                {
                    return View(new List<TrabajadorViewModel>());
                }
                var viewModel = trabajadoresDto.Value.Select(t => new TrabajadorViewModel()
                {
                    TrabajadorId = t.TrabajadorId,
                    Nombre = t.Nombre,
                    Apellidos = t.ApellidoPaterno + " " + t.ApellidoPaterno,
                    Dni = t.Dni,
                    Direccion = t.Direccion,
                    Email = t.Email,
                    FechaRegistro = t.FechaRegistro
                });
                var paginacion = PaginacionHelper.Paginacion(viewModel, pagina, tamanioPagina);
                ViewBag.HoraActual = _dateTimeProvider.DateTimeActual().Date;
                return View(paginacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
            
        }
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Pedidos(int trabajadorId)
        {
            try
            {
                var result = await _trabajadorService.PedidosPorTrabajadorHoy(trabajadorId);
                if (!result.Success)
                {

                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction("UnauthorizedPage", "Auth");
                }
                if(result.Value is null)
                {
                    return View(new List<PedidoViewModel>());
                }
                var viewModel = result.Value.Select(p => new PedidoViewModel()
                {
                    PedidoId = p.PedidoId,
                    CodigoPedido = p.CodigoPedido,
                    Fecha = p.Fecha,
                    ClienteId = p.ClienteId,
                    MontoAdelantado = p.MontoAdelantado,
                    MontoFaltante = p.MontoFaltante,
                    MontoTotal = p.MontoTotal,
                    MetodoPago = p.MetodoPago,
                    Estado = p.Estado,
                    Cliente = new ClienteViewModel()
                    {
                        ClienteId = p.Cliente.ClienteId,
                        Nombre = p.Cliente.Nombre,
                        ApellidoPaterno = p.Cliente.ApellidoPaterno,
                        ApellidoMaterno = p.Cliente.ApellidoMaterno,
                        NumeroCelular = p.Cliente.NumeroCelular,
                        Direccion = p.Cliente.Direccion,
                        Email = p.Cliente.Email,
                        FechaRegistro = p.Cliente.FechaRegistro
                    },
                });
                return View(viewModel);

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
    }
}
