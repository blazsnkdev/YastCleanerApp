using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;
using YastCleaner.Entities.Entidades;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Data.Repositorios
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly AppDbContext _appDbContext;
        public UsuarioRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        //NOTE : obtiene la lista de todos los usuarios con rol trabajador
        public async Task<IEnumerable<Usuario>> GetAllByRolTrabajador()
        {
            return await _appDbContext.TblUsuario.Where(u=>u.Rol == Rol.Trabajador).ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAllByRolTrabajadorPedidosHoy(DateTime date)
        {
            return await _appDbContext.TblUsuario
                .Where(u => u.Rol == Rol.Trabajador && u.Pedidos.Any(p => p.FechaRegistro.Date == date.Date))
                .Include(u => u.Pedidos.Where(p => p.FechaRegistro.Date == date.Date))
                .ThenInclude(p => p.Cliente)
                .ToListAsync();
        }

        //NOTE : obtiene el usuario partir del email de la bd
        public async Task<Usuario?> GetByEmail(string email)
        {
            return await _appDbContext.TblUsuario.FirstOrDefaultAsync(u => u.Email == email);
            
        }

        public async Task<Usuario?> GetByIdUsuario(int usuarioId)
        {
            return await _appDbContext.TblUsuario.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
        }

        public async Task<bool> UsuarioDniExiste(string dni)
        {
            var existe = await _appDbContext.TblUsuario.FirstOrDefaultAsync(u => u.Dni == dni);
            if (existe == null)
                return false;
            return true;
        }
    }
}
