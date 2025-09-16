using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Data.Interfaces;
using YastCleaner.Data.Repositorios;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.UnitOfWork
{
    public interface IUnitOfWork :IDisposable
    {
        //TODO : aqui falta agregar los repositorios
        public IUsuarioRepository UsuarioRepository { get; }
        public IServicioRepository ServicioRepository { get; }
        public IPedidoRepository PedidoRepository { get; }
        public IPedidoDetalleRepository PedidoDetalleRepository { get; }
        public IClienteRepository ClienteRepository { get; }
        public IReporteRepository ReporteRepository { get; }
        public IPedidoEntregadoRepository PedidoEntregadoRepository { get; }
        //
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackAsync();
    }
}
