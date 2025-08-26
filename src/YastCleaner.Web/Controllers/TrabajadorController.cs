using Humanizer;
using Microsoft.AspNetCore.Mvc;
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
    public class TrabajadorController : Controller
    {
        private readonly ITrabajadorService _trabajadorService;
        private readonly IEnviarCorreoSmtp _enviarCorreo;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TrabajadorController(ITrabajadorService trabajadorService, IEnviarCorreoSmtp enviarCorreo, IDateTimeProvider dateTimeProvider)
        {
            _trabajadorService = trabajadorService;
            _enviarCorreo = enviarCorreo;
            _dateTimeProvider = dateTimeProvider;
        }
        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Trabajadores(int pagina = 1, int tamanioPagina =10)//este es el total
        {
            var trabajadoresDto = await _trabajadorService.ListaTrabajadores();
            //TODO : utilizar esto para la paginacion
            var viewModel = trabajadoresDto.Select(t => new TrabajadorViewModel
            {
                TrabajadorId = t.TrabajadorId,
                Nombre = t.Nombre,
                Apellidos = t.ApellidoPaterno+" "+t.ApellidoPaterno,
                Dni = t.Dni,
                Direccion = t.Direccion,
                Email = t.Email,
                FechaRegistro = t.FechaRegistro
            });
            var paginacion = PaginacionHelper.Paginacion(viewModel, pagina, tamanioPagina);
            return View(paginacion);
        }
        [RoleAuthorize(Rol.Administrador)]
        public IActionResult Registrar()
        {
            return View(new RegistrarTrabajadorViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(RegistrarTrabajadorViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            string password = _trabajadorService.GenerarPassword(viewModel.Nombre, viewModel.ApellidoPaterno, viewModel.ApellidoMaterno);
            var trabajadorDto = new TrabajadorDto()
            {
                Nombre = viewModel.Nombre,
                ApellidoPaterno = viewModel.ApellidoPaterno,
                ApellidoMaterno = viewModel.ApellidoMaterno,
                Dni = viewModel.Dni,
                Direccion = viewModel.Direccion,
                Email = viewModel.Email,
                Password = password
            };
            var result = await _trabajadorService.RegistrarTrabajador(trabajadorDto);
            if (!result)
                return View(viewModel);

            _enviarCorreo.EnviarCorreo(trabajadorDto.Email, password);
            return RedirectToAction("Trabajadores");
        }
    }
}
