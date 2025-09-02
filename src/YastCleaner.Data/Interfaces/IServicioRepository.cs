using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Interfaces
{
    public interface IServicioRepository :IRepository<Servicio>
    {
        Task<Servicio?> GetByNameAsync(string nombre);
    }
}
