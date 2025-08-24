using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario> //TODO : hereda los metodos sin impl de repositorio generico
    {
        Task<Usuario?> GetByEmail(string email);//TODO : aqui solo el email pq el password hash se hace en el servicio
    }
}
