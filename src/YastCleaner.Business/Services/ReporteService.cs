using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Business.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IDateTimeProvider _dateTimeProvider;
        public ReporteService(IUnitOfWork uoW, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<TrabajadorDto>> DetalleRegistroReporte(int trabajadorId)
        {
            if(trabajadorId <= 0)
            {
                return Result<TrabajadorDto>.Fail("El ID del trabajador es inválido");
            }
            var trabajador =await _UoW.UsuarioRepository.GetByRolTrabajadorPedidosById(trabajadorId, _dateTimeProvider.DateTimeActual());
            if (trabajador is null)
            {
                return Result<TrabajadorDto>.Fail("El trabajador no existe");
            }
            var pedidosTrabajador = await _UoW.PedidoRepository.GetAllPedidosByTrabajadorHoy(trabajadorId, _dateTimeProvider.DateTimeActual());
            if (!pedidosTrabajador.Any())
            {
                return Result<TrabajadorDto>.Ok(new TrabajadorDto
                {
                    TrabajadorId = trabajador.UsuarioId,
                    Nombre = trabajador.Nombre,
                    ApellidoPaterno = trabajador.ApellidoPaterno,
                    ApellidoMaterno = trabajador.ApellidoMaterno,
                    Direccion = trabajador.Direccion,
                    Email = trabajador.Email,
                    Pedidos = new List<PedidoDto>()
                });
            }
            if(pedidosTrabajador is null || !pedidosTrabajador.Any())
            {
                return Result<TrabajadorDto>.Ok(new TrabajadorDto
                {
                    TrabajadorId = trabajador.UsuarioId,
                    Nombre = trabajador.Nombre,
                    ApellidoPaterno = trabajador.ApellidoPaterno,
                    ApellidoMaterno = trabajador.ApellidoMaterno,
                    Direccion = trabajador.Direccion,
                    Email = trabajador.Email,
                    Pedidos = new List<PedidoDto>()
                });
            }

            var pedidosDto = pedidosTrabajador.Select(p=>new PedidoDto
            {
                PedidoId = p.PedidoId,
                CodigoPedido = p.CodigoPedido,
                Fecha = p.Fecha,
                ClienteId = p.ClienteId,
                MontoAdelantado = p.MontoAdelantado,
                MontoFaltante = p.MontoFaltante,
                MontoTotal = p.MontoTotal,
                MetodoPago = p.MetodoPago.ToString(),
                Estado = p.Estado.ToString(),
            }).ToList();
            var trabajadorDto = new TrabajadorDto
            {
                TrabajadorId = trabajador.UsuarioId,
                Nombre = trabajador.Nombre,
                ApellidoPaterno = trabajador.ApellidoPaterno,
                ApellidoMaterno = trabajador.ApellidoMaterno,
                Direccion = trabajador.Direccion,
                Email = trabajador.Email,
                Pedidos = pedidosDto
            };
            return Result<TrabajadorDto>.Ok(trabajadorDto);
        }

        public async Task<Result<ReporteDto>> DetalleReporte(int reporteId)
        {
            var reporte = await _UoW.ReporteRepository.GetByIdAsync(reporteId);
            if(reporte is null)
            {
                return Result<ReporteDto>.Fail("El reporte no existe");
            }
            var reporteDto = new ReporteDto
            {
                ReporteId = reporte.ReporteId,
                TrabajadorId = reporte.UsuarioId,
                MontoGenerado = reporte.MontoGenerado,
                FechaRegistro = reporte.FechaReporte
            };
            return Result<ReporteDto>.Ok(reporteDto);
        }

        public async Task<Result<int>> RegistrarReporte(ReporteDto reporteDto)
        {
            try
            {
                var trabajador = await _UoW.UsuarioRepository.GetByIdAsync(reporteDto.TrabajadorId);
                if(trabajador is null)
                {
                    return Result<int>.Fail("El trabajador no existe");
                }
                var reporte = new Reporte
                {
                    UsuarioId = reporteDto.TrabajadorId,
                    MontoGenerado = reporteDto.MontoGenerado,
                    FechaReporte = _dateTimeProvider.DateTimeActual()
                };
                await _UoW.ReporteRepository.AddAsync(reporte);
                await _UoW.SaveChangesAsync();
                return Result<int>.Ok(reporte.ReporteId);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error al registrar el reporte: {ex.Message}");
            }
        }

    }
}
