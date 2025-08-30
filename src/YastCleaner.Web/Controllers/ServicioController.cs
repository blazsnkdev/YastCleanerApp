using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YastCleaner.Business.Interfaces;
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
            var serviciosDto = await _servicioService.ListaServicios();
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
        public async Task<IActionResult> Seleccionar(int servicioId, int cantidad)
        {
            var result = await _servicioService.AgregarServicioAlPedido(servicioId,cantidad);
            if(!result.Success)
                return View(result);
            return RedirectToAction("Temporal","Pedido");//TODO: ojito aqui cambiar che el temp

        }
    }
}
