using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Repositorios
{
    public class ClienteRepository : Repository<Cliente>
    {
        public ClienteRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
