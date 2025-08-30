using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.DTOs;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;
using YastCleaner.Data.UnitOfWork;

namespace YastCleaner.Business.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _UoW;

        public ClienteService(IUnitOfWork uoW)
        {
            _UoW = uoW;
        }

        public async Task<Result<ClienteDto>> ObtenerCliente(int clienteId)
        {
            var cliente = await _UoW.ClienteRepository.GetByIdAsync(clienteId);
            if (cliente is null)
                return Result<ClienteDto>.Fail("El cliente no existe");
            var clienteDto = new ClienteDto()
            {
                ClienteId = clienteId,
                Nombre = cliente.Nombre
            };
            return Result<ClienteDto>.Ok(clienteDto);
        }

        public async Task<List<ClienteDto>> ListarClientes()
        {
            var clientes = await _UoW.ClienteRepository.GetAllAsync();
            return clientes.Select(c => new ClienteDto()
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre
            }).ToList();
        }
    }
}
