using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;

namespace YastCleaner.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DashboardService(IUnitOfWork uoW, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _dateTimeProvider = dateTimeProvider;
        }

        public Result<DashboardDto> GetDashboardHoy()
        {
            var hoy = _dateTimeProvider.DateTimeActual();
            var dashbpardDto =  new DashboardDto
            {
                TotalPedidos = _UoW.PedidoRepository.ContarPedidos(hoy),
                TotalPedidosEntregados = _UoW.PedidoRepository.ContarPedidosEntregados(hoy),
                MontoTotal = _UoW.PedidoRepository.SumarMontoTotal(hoy)
            };
            return Result<DashboardDto>.Ok(dashbpardDto);
        }
    }
}
