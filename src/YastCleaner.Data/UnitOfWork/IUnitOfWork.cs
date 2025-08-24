using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Data.UnitOfWork
{
    public interface IUnitOfWork :IDisposable
    {
        //TODO : aqui falta agregar los repositorios

        //
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackAsync();
    }
}
