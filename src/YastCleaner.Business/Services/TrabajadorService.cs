using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IAuthService _authService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TrabajadorService(IUnitOfWork uoW, IAuthService authService, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _authService = authService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<TrabajadorDto>> ListaTrabajadores()
        {
            var trabajadores = await _UoW.UsuarioRepository.GetAllByRolTrabajador();
            return trabajadores.Select(t => new TrabajadorDto
            {
                TrabajadorId = t.UsuarioId,
                Nombre = t.Nombre,
                ApellidoPaterno = t.ApellidoPaterno,
                ApellidoMaterno = t.ApellidoMaterno,
                Dni = t.Dni,
                Direccion = t.Direccion,
                Email = t.Email,
                Password = t.Password,
                FechaRegistro = t.FechaRegistro
            }).ToList();
        }

        public async Task<bool> RegistrarTrabajador(TrabajadorDto dto)
        {
            var existe = await _UoW.UsuarioRepository.UsuarioDniExiste(dto.Dni);
            if (existe)//si si esxiste devolver true pq el dni ya esta ocupado 
                return false;
            string encriptado = _authService.HashPassword(dto.Password);//esto ya llega del viewModel
            var trabajador = new Usuario()
            {
                UsuarioId = dto.TrabajadorId,
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Dni = dto.Dni,
                Direccion = dto.Direccion,
                Email = dto.Email,
                Password = encriptado,
                Rol = Rol.Trabajador,
                FechaRegistro = _dateTimeProvider.DateTimeActual()//asignar el trabajador
            };
            await _UoW.UsuarioRepository.AddAsync(trabajador);
            await _UoW.SaveChangesAsync();
            return true;//TODO : flaco, aqui posiblemente a futuro se debera realizar un object Result
        }
        public string GenerarPassword(string nombre, string apellidoPaterno, string apellidoMaterno)
        {
            var fechaHoraActual = _dateTimeProvider.DateTimeActual().ToString("yyyyMMddHHmmss");
            string parteNombre = !string.IsNullOrEmpty(nombre) ? nombre.Substring(0, 1) : "";
            string parteApePat = !string.IsNullOrEmpty(apellidoPaterno) ? apellidoPaterno.Substring(0, 1) : "";
            string parteApeMat = !string.IsNullOrEmpty(apellidoMaterno) ? apellidoMaterno.Substring(0, 1) : "";
            Random rnd = new Random();
            string numerosAleatorios = rnd.Next(100, 999).ToString();
            return $"{parteNombre}{parteApePat}{parteApeMat}{numerosAleatorios}{fechaHoraActual}";
        }

        public async Task<bool> ActualizarTrabajador(TrabajadorDto dto)
        {
            var seleccionado = await _UoW.UsuarioRepository.GetById(dto.TrabajadorId);
            if (seleccionado is null)
                return false;
            try
            {
                seleccionado.Nombre = dto.Nombre;
                seleccionado.ApellidoPaterno = dto.ApellidoPaterno;
                seleccionado.ApellidoMaterno = dto.ApellidoMaterno;
                seleccionado.Direccion = dto.Direccion;
                seleccionado.Email = dto.Email;
                _UoW.UsuarioRepository.UpdateAsync(seleccionado);
                await _UoW.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<TrabajadorDto?> ObtenerTrabajador(int trabajdorId)
        {
            var trabajador = await _UoW.UsuarioRepository.GetById(trabajdorId);
            if (trabajador is null)
                return null;
            return new TrabajadorDto()
            {
                TrabajadorId = trabajdorId,
                Nombre = trabajador.Nombre,
                ApellidoPaterno = trabajador.ApellidoPaterno,
                ApellidoMaterno = trabajador.ApellidoMaterno,
                Dni = trabajador.Dni,
                Direccion = trabajador.Direccion,
                Email = trabajador.Email
            };
        }
    }
}
