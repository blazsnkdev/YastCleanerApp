using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Data.UnitOfWork;

namespace YastCleaner.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _UoW;

        public AuthService(IUnitOfWork uoW)
        {
            _UoW = uoW;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<SesionDto?> LoginAsync(string email, string password)
        {
            var usuario = await _UoW.UsuarioRepository.GetByEmail(email);
            var passwordValido= ValidarPassword(password, usuario?.Password ?? "");
            if (usuario == null || !passwordValido)
                return null;

            return new SesionDto
            {
                UsuarioId = usuario!.UsuarioId,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol
            };
        }

        private bool ValidarPassword(string passwordIngresada, string passwordHasheadaBD)
        {
            return BCrypt.Net.BCrypt.Verify(passwordIngresada, passwordHasheadaBD);
        }
    }
}
