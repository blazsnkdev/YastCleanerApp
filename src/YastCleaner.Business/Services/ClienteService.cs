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
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _UoW;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ClienteService(IUnitOfWork uoW, IDateTimeProvider dateTimeProvider)
        {
            _UoW = uoW;
            _dateTimeProvider = dateTimeProvider;
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
        public async Task<Result> CrearCliente(ClienteDto clienteDto)
        {
            //validaciones
            if (string.IsNullOrEmpty(clienteDto.Nombre)
                || string.IsNullOrEmpty(clienteDto.ApellidoMaterno)
                || string.IsNullOrEmpty(clienteDto.ApellidoPaterno)
                || string.IsNullOrEmpty(clienteDto.NumeroCelular)
                || string.IsNullOrEmpty(clienteDto.Direccion)
                || string.IsNullOrEmpty(clienteDto.Email)) 
            {
                return Result.Fail("Todos los campos son obligatorios");
            }
            // Crear el cliente
            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                ApellidoPaterno = clienteDto.ApellidoPaterno,
                ApellidoMaterno = clienteDto.ApellidoMaterno,
                NumeroCelular = clienteDto.NumeroCelular,
                Direccion = clienteDto.Direccion,
                Email = clienteDto.Email,
                Estado = EstadoCliente.Activo,
                FechaRegistro = _dateTimeProvider.DateTimeActual()
            };
            await _UoW.ClienteRepository.AddAsync(cliente);
            await _UoW.SaveChangesAsync();
            return Result.Ok();
        }
        public async Task<List<ClienteDto>> ObtenerClientesActivos()
        {
            var clientesAll = await _UoW.ClienteRepository.GetAllAsync();
            //TODO : 
            var clientesActivos = clientesAll.Where(c => c.Estado == EstadoCliente.Activo).ToList();
            return clientesActivos.Select(c => new ClienteDto()
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre,
                ApellidoPaterno = c.ApellidoPaterno,
                ApellidoMaterno = c.ApellidoMaterno,    
                NumeroCelular = c.NumeroCelular,
                Direccion = c.Direccion,
                Estado = c.Estado.ToString(),
                FechaRegistro = c.FechaRegistro
            }).ToList();
        }
    }
}
