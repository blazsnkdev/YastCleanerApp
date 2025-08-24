using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Repositorios
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly AppDbContext _appDbContext;
        public UsuarioRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
         //NOTE : obtiene el usuario partir del email de la bd
        public async Task<Usuario?> GetByEmail(string email)
        {
            return await _appDbContext.TblUsuario.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
