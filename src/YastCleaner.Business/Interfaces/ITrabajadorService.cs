using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Utils;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Business.Interfaces
{
    public interface ITrabajadorService
    {
        Task<TrabajadorDto?> ObtenerTrabajador(int trabajdorId);
        Task<List<TrabajadorDto>> ListaTrabajadores();
        Task<Result> RegistrarTrabajador(TrabajadorDto dto);
        Task<Result> ActualizarTrabajador(TrabajadorDto dto);
        string GenerarPassword(string nombre, string apellidoPaterno, string apellidoMaterno);
        Task<Result> EliminarTrabajador(int trabajadorId);
        Task<Result<List<PedidoDto>>> PedidosPorTrabajador(int trabajadorId);
        Task<Result<List<TrabajadorDto>>> TrabajadoresConPedidosHoy();
        Task<Result<List<PedidoDto>>> PedidosPorTrabajadorHoy(int trabajadorId);
    }
}
