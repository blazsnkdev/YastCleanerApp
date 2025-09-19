using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;

namespace YastCleaner.Business.Interfaces
{
    public interface IReporteService
    {
        Task<Result<int>> RegistrarReporte(ReporteDto reporteDto);
        Task<Result<ReporteDto>> DetalleReporte(int reporteId);
        Task<Result<TrabajadorDto>> DetalleRegistroReporte(int trabajadorId);
        Task<bool> ExisteReportePorTrabajadorHoy(int trabajadorId);
    }
}
