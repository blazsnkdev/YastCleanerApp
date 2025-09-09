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
    public interface IClienteService
    {
        //TODO : esto voy a tener que cambiar despues para que solo tenga el id 
        Task<List<ClienteDto>> ListarClientes();
        Task<Result<ClienteDto>> ObtenerCliente(int clienteId);
        Task<Result> CrearCliente(ClienteDto clienteDto);
        Task<List<ClienteDto>> ObtenerClientesActivos();
        Task<Result<ClienteDto>> ObtenerDetalleCliente(int clienteId);
        Task<Result> ActualizarCliente(int clienteId,ClienteDto clienteDto);
        Task<Result> DesactivarCliente(int clienteId);
    }
}
