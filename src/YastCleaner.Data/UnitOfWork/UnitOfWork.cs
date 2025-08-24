using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Data;
using YastCleaner.Data.Interfaces;

namespace YastCleaner.Data.UnitOfWork
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        private IDbContextTransaction? _transaction;
        public IUsuarioRepository UsuarioRepository { get; }

        public UnitOfWork(
            AppDbContext appDbContext,
            IUsuarioRepository usuarioRepository
            )
        {
            _appDbContext = appDbContext;
            UsuarioRepository = usuarioRepository;
        }



        public async Task BeginTransactionAsync()
        {
            _transaction = await _appDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
                await _appDbContext.SaveChangesAsync();
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _appDbContext.Dispose();
        }

        public async Task RollBackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
