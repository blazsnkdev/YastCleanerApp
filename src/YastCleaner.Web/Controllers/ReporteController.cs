using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.Utils;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ITrabajadorService _trabajadorService;
        private readonly IReporteService _reporteService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReporteController(
            ITrabajadorService trabajadorService,
            IDateTimeProvider dateTimeProvider,
            IReporteService reporteService)
        {
            _trabajadorService = trabajadorService;
            _dateTimeProvider = dateTimeProvider;
            _reporteService = reporteService;
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> CerrarCaja(int pagina = 1, int tamanioPagina = 10)
        {
            try
            {
                var trabajadoresDto = await _trabajadorService.TrabajadoresConPedidosHoy();
                var fecha = _dateTimeProvider.DateTimeActual();

                if (!trabajadoresDto.Success || trabajadoresDto.Value is null || !trabajadoresDto.Value.Any())
                {
                    ViewBag.HoraActual = fecha.ToString("dd/MM/yyyy");
                    return View(new PaginaResult<CierreCajaTrabajadorViewModel>
                    {
                        Items = new List<CierreCajaTrabajadorViewModel>(),
                        TotalRegistros = 0,
                        PaginaIndice = pagina,
                        TamanioPagina = tamanioPagina
                    });
                }

                // crear una lista de tasks
                var tareas = trabajadoresDto.Value.Select(async t => new CierreCajaTrabajadorViewModel
                {
                    TrabajadorId = t.TrabajadorId,
                    Nombre = t.Nombre,
                    Apellidos = $"{t.ApellidoPaterno} {t.ApellidoMaterno}",
                    Dni = t.Dni,
                    Direccion = t.Direccion,
                    Email = t.Email,
                    FechaRegistro = t.FechaRegistro,
                    Registrado = await _reporteService.ExisteReportePorTrabajadorHoy(t.TrabajadorId)
                });

                // esperar a que terminen todas las tasks
                var viewModelList = await Task.WhenAll(tareas);

                // ahora sí paginar
                var paginacion = PaginacionHelper.Paginacion(viewModelList, pagina, tamanioPagina);

                ViewBag.HoraActual = fecha.ToString("dd/MM/yyyy HH:mm");
                return View(paginacion);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "No tiene autorización para dar el cierre de caja";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error inesperado: {ex.Message}";
                return RedirectToAction("Error", "Home");
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
                    return RedirectToAction("NotFoundPage", "Auth");
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
                    Trabajador = new TrabajadorViewModel()
                    {
                        TrabajadorId = p.UsuarioId,
                        Nombre = p.Trabajador.Nombre,
                    },
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
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
        
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Registrar(int trabajadorId)
        {
            try
            {
                var trabajadorDto = await _reporteService.DetalleRegistroReporte(trabajadorId);
                if(trabajadorDto.Value is null)
                {
                    TempData["Error"] = "El trabajador no existe";
                    return RedirectToAction(nameof(CerrarCaja));
                }
                var trabajadorViewModel = new RegistrarReporteViewModel()
                {
                    TrabajadorId = trabajadorDto.Value.TrabajadorId,
                    MontoTotal = trabajadorDto.Value.Pedidos.Sum(p => p.MontoAdelantado)
                };
                ViewBag.Pedidos = trabajadorDto.Value.Pedidos.Count();
                return View(trabajadorViewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Registrar(RegistrarReporteViewModel viewModel)
        {
            try
            {
                var result = await _reporteService.RegistrarReporte(new ReporteDto
                {
                    TrabajadorId = viewModel.TrabajadorId,
                    MontoGenerado = viewModel.MontoTotal
                });
                if (!result.Success)
                {
                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Detalle), new { reporteId = result.Value});
                }
                    TempData["Exito"] = "Reporte registrado exitosamente";
                    return RedirectToAction(nameof(CerrarCaja));
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
        
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Detalle(int reporteId)
        {
            try
            {
                var result = await _reporteService.DetalleReporte(reporteId);
                if (!result.Success || result.Value is null)
                {
                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(CerrarCaja));
                }
                var reporteDetalleViewModel = new DetalleReporteViewModel()
                {
                    TrabajadorId = result.Value.TrabajadorId,
                    Trabajador = new TrabajadorViewModel
                    {
                        TrabajadorId = result.Value.Trabajador.TrabajadorId,
                        Nombre = result.Value.Trabajador.Nombre,
                        Apellidos = result.Value.Trabajador.ApellidoPaterno + " " + result.Value.Trabajador.ApellidoMaterno,
                    },
                    MontoTotal = result.Value.MontoGenerado,
                    FechaReporte = result.Value.FechaRegistro
                };
                return View(reporteDetalleViewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }
    }
}
