using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Services;
using YastCleaner.Entities.Enums;
using YastCleaner.Web.Filters;
using YastCleaner.Web.Helpers;
using YastCleaner.Web.ViewModels;

namespace YastCleaner.Web.Controllers
{
    public class ServicioController : Controller
    {
        private readonly IServicioService _servicioService;

        public ServicioController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [RoleAuthorize(Rol.Administrador,Rol.Trabajador)]
        public async Task<IActionResult> Servicios(int indicePagina =1,int tamanioPagina =10)
        {
            var serviciosDto = await _servicioService.ListaServiciosDisponibles();
            var viewModel = serviciosDto.Select(p => new ServicioViewModel()
            {
                ServicioId = p.ServicioId,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Descripcion = p.Descripcion,
                Estado = p.Estado//TODO : esto ya viene como estring
            });
            var paginacion = PaginacionHelper.Paginacion(viewModel, indicePagina, tamanioPagina);
            return View(paginacion);
        }
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Modulo(int indicePagina = 1, int tamanioPagina = 10, string estado = "Disponible")
        {
            try
            {
                List<ServicioViewModel> viewModel;

                if (estado == "Inactivo")
                {
                    var serviciosDtoInactivos = await _servicioService.ListaServiciosInactivos();
                    viewModel = serviciosDtoInactivos.Select(s => new ServicioViewModel
                    {
                        ServicioId = s.ServicioId,
                        Nombre = s.Nombre,
                        Precio = s.Precio,
                        Descripcion = s.Descripcion,
                        Estado = s.Estado,
                        FechaRegistro = s.FechaRegistro
                    }).ToList();
                }
                else
                {
                    var serviciosDtoDisponibles = await _servicioService.ListaServiciosDisponibles();
                    viewModel = serviciosDtoDisponibles.Select(s => new ServicioViewModel
                    {
                        ServicioId = s.ServicioId,
                        Nombre = s.Nombre,
                        Precio = s.Precio,
                        Descripcion = s.Descripcion,
                        Estado = s.Estado,
                        FechaRegistro = s.FechaRegistro
                    }).ToList();
                }

                var paginacion = PaginacionHelper.Paginacion(viewModel, indicePagina, tamanioPagina);
                return View(paginacion);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "Sin autorización para el módulo de Servicios";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        [RoleAuthorize(Rol.Trabajador)]
        public async Task<IActionResult> Seleccionar(int servicioId)
        {
            var servicioDto = await _servicioService.ObtenerServicio(servicioId);
            if (servicioDto is null)
                return View("Servicios");
            var viewModel = new ServicioViewModel()
            {
                ServicioId = servicioDto.ServicioId,
                Nombre = servicioDto.Nombre,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio,
                Estado = servicioDto.Estado
            };
            return View(viewModel);
        }
        [HttpPost]
        [RoleAuthorize(Rol.Trabajador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Seleccionar(int servicioId, int cantidad)
        {
            var result = await _servicioService.AgregarServicioAlPedido(servicioId,cantidad);
            if(!result.Success)
                return View(result);
            return RedirectToAction("Temporal","Pedido");//TODO: ojito aqui cambiar che el temp

        }

        [RoleAuthorize(Rol.Administrador)]
        public IActionResult RegistrarServicio()
        {
            return View(new ServicioViewModel());
        }
        [HttpPost]
        [RoleAuthorize(Rol.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarServicio([FromBody]ServicioViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var servicioDto = new ServicioDto()
            {
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Precio = viewModel.Precio
            };
            var result = await _servicioService.RegistrarServicio(servicioDto);
            if (!result.Success)
            {
                return View(viewModel);
            }
            return RedirectToAction("Servicios", "Servicio");
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> EditarServicio(int servicioId)
        {
            var servicioDto = await _servicioService.ObtenerServicio(servicioId);
            if (servicioDto is null)
                return RedirectToAction("Servicios","Servicio");
            var viewModel = new ServicioViewModel()
            {
                ServicioId = servicioDto.ServicioId,
                Nombre = servicioDto.Nombre,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio,
                Estado = servicioDto.Estado
            };
            await CargarEstadoServicio();
            return View(viewModel);
        }
        [HttpPost]
        [RoleAuthorize(Rol.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarServicio(int servicioId,[FromBody]ServicioViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await CargarEstadoServicio();
                return View(viewModel);
            }
            var servicioDto = new ServicioDto()
            {
                ServicioId = viewModel.ServicioId,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Precio = viewModel.Precio,
                Estado = viewModel.Estado
            };
            var result = await _servicioService.EditarServicio(servicioId, servicioDto);
            if (!result.Success)
            {
                await CargarEstadoServicio();
                return View(viewModel);
            }
            return RedirectToAction("Servicios", "Servicio");
        }

        [HttpPost]
        [RoleAuthorize(Rol.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManipularEstado(int servicioId)
        {
            try
            {
                var result = await _servicioService.ManipularEstadoServicio(servicioId);
                if (!result.Success)
                {
                    ViewBag.Error = "No se pudo desactivar el servicio.";
                }
                return RedirectToAction("Modulo", "Servicio");
            }
            catch (UnauthorizedAccessException) 
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }
        }

        private async Task CargarEstadoServicio()
        {
            ViewBag.Estados = new SelectList(await _servicioService.ListarEstadoServicios());
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Detalle(int servicioId)
        {
            try
            {
                var detalleServicioDto = await _servicioService.DetalleServicio(servicioId);
                if (!detalleServicioDto.Success)
                {
                    ViewBag.Error = detalleServicioDto.ErrorMessage;
                    return View("Modulo");
                }
                if(detalleServicioDto.Value is null)
                {
                    ViewBag.Error = detalleServicioDto.ErrorMessage;
                    return View("Modulo");
                }
                var detalleServicioViewModel = new DetalleServicioViewModel
                {
                    ServicioId = servicioId,
                    Nombre = detalleServicioDto.Value.Nombre,
                    Descripcion = detalleServicioDto.Value.Descripcion,
                    Precio = detalleServicioDto.Value.Precio,
                    Estado = detalleServicioDto.Value.Estado,
                    CantidadDetalles = detalleServicioDto.Value.CantidadUtilizados,
                    MontoGenerado = detalleServicioDto.Value.MontoTotalGenerado
                };
                return View(detalleServicioViewModel);
            }catch (UnauthorizedAccessException)
            {
                return RedirectToAction("UnauthorizedPage", "Auth");
            }

        }
        [HttpPost]
        [RoleAuthorize(Rol.Administrador)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int servicioId)
        {
            try
            {
                var result = await _servicioService.EliminarServicio(servicioId);
                if (!result.Success)
                {
                    ViewBag.Error = result.ErrorMessage;
                    return View("Detalle", new { servicioId = servicioId});
                }
                return RedirectToAction("Modulo", "Servicio");
            }catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "Sin autorización para eliminar el servicio";
                return RedirectToAction("UnauthorizedPage", "Auth");
            }

        }
        
    }
}
