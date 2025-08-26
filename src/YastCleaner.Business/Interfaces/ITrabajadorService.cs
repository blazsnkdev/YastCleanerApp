using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Business.Interfaces
{
    public interface ITrabajadorService
    {
        Task<TrabajadorDto?> ObtenerTrabajador(int trabajdorId);
        Task<List<TrabajadorDto>> ListaTrabajadores();
        Task<bool> RegistrarTrabajador(TrabajadorDto dto);
        Task<bool> ActualizarTrabajador(TrabajadorDto dto);
        string GenerarPassword(string nombre, string apellidoPaterno, string apellidoMaterno);
    }
}
