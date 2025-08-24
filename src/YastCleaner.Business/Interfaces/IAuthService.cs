using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Business.Interfaces
{
    public interface IAuthService
    {
        Task<SesionDto?> LoginAsync(string email, string password);
        string HashPassword(string password);
        // NOTE : esto lo manejo de forma private en la impl pq desde otra clase no se puede hacer la validacion sino solo en el authService
        //bool ValidarPassword(string passwordIngresada, string passwordHasheadaBD);
    }
}
