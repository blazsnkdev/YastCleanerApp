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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        private IDbContextTransaction? _transaction;
        //Repositorios
        public IUsuarioRepository UsuarioRepository { get; }
        public IServicioRepository ServicioRepository { get; }
        public IPedidoRepository PedidoRepository { get; }
        public IPedidoDetalleRepository PedidoDetalleRepository { get; }
        public IClienteRepository ClienteRepository { get; }
        public IReporteRepository ReporteRepository { get; }
        public IPedidoEntregadoRepository PedidoEntregadoRepository { get; }
        public IPedidoAnuladoRepository PedidoAnuladoRepository { get; }

        public UnitOfWork(
            AppDbContext appDbContext,
            IUsuarioRepository usuarioRepository,
            IServicioRepository servicioRepository,
            IPedidoRepository pedidoRepository,
            IPedidoDetalleRepository pedidoDetalleRepository,
            IClienteRepository clienteRepository,
            IReporteRepository reporteRepository,
            IPedidoEntregadoRepository pedidoEntregadoRepository,
            IPedidoAnuladoRepository pedidoAnuladoRepository
        )
        {
            _appDbContext = appDbContext;
            UsuarioRepository = usuarioRepository;
            ServicioRepository = servicioRepository;
            PedidoRepository = pedidoRepository;
            PedidoDetalleRepository = pedidoDetalleRepository;
            ClienteRepository = clienteRepository;
            ReporteRepository = reporteRepository;
            PedidoEntregadoRepository = pedidoEntregadoRepository;
            PedidoAnuladoRepository = pedidoAnuladoRepository;
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
