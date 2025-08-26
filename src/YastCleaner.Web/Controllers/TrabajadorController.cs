using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        

        public TrabajadorController(ITrabajadorService trabajadorService, IEnviarCorreoSmtp enviarCorreo)
        {
            _trabajadorService = trabajadorService;
            _enviarCorreo = enviarCorreo;
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
            return View(new InsertarTrabajadorViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(InsertarTrabajadorViewModel viewModel)
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

            _enviarCorreo.EnviarCorreo(trabajadorDto.Email, password,"Registrada");
            return RedirectToAction("Trabajadores");
        }

        [RoleAuthorize(Rol.Administrador)]
        public async Task<IActionResult> Actualizar(int id)
        {
            var trabajadorDto =await _trabajadorService.ObtenerTrabajador(id);
            if (trabajadorDto is null)
                return View("Trabajadores");
            var trabajadorViewModel = new InsertarTrabajadorViewModel()
            {
                TrabajadorId = trabajadorDto.TrabajadorId,
                Nombre = trabajadorDto.Nombre,
                ApellidoPaterno = trabajadorDto.ApellidoPaterno,
                ApellidoMaterno = trabajadorDto.ApellidoMaterno,
                Dni = trabajadorDto.Dni,
                Direccion = trabajadorDto.Direccion,
                Email = trabajadorDto.Email
            };
            return View(trabajadorViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Actualizar(InsertarTrabajadorViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "No se pudo actualizar el trabajador. Verifica los datos.");
                return View(viewModel);
            }
            var trabajadorDto = new TrabajadorDto()
            {
                TrabajadorId = viewModel.TrabajadorId,
                Nombre = viewModel.Nombre,
                ApellidoPaterno = viewModel.ApellidoPaterno,
                ApellidoMaterno = viewModel.ApellidoMaterno,
                Dni = viewModel.Dni,
                Direccion = viewModel.Direccion,
                Email = viewModel.Email
            };
            var result = await _trabajadorService.ActualizarTrabajador(trabajadorDto);
            if (!result)
                return View(viewModel);
            return RedirectToAction("Trabajadores");
        }

    } 
}
