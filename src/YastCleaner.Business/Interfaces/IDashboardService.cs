using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;

namespace YastCleaner.Business.Interfaces
{
    public interface IDashboardService
    {
        Result<DashboardDto> GetDashboardHoy();
    }
}
