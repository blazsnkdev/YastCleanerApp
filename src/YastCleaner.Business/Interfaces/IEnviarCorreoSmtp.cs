using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YastCleaner.Business.Interfaces
{
    public interface IEnviarCorreoSmtp
    {
        void RegistroTrabajador(string email, string password);
        void RegistroPedido(string email);
    }
}
