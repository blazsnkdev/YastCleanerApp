using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Business.Interfaces;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        public Task<List<MetodoPago>> ListarMetodosPago() => Task.FromResult(Enum.GetValues<MetodoPago>().ToList());

    }
}
