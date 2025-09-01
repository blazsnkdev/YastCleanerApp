using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Enums;

namespace YastCleaner.Business.Interfaces
{
    public interface IMetodoPagoService
    {
        Task<List<MetodoPago>> ListarMetodosPago();
    }
}
