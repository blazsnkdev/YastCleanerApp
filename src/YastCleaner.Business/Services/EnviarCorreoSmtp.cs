using MailKit.Net.Smtp;
using MimeKit;
using YastCleaner.Business.Helpers;
using YastCleaner.Business.Interfaces;
using YastCleaner.Business.Utils;

namespace YastCleaner.Business.Services
{
    public class EnviarCorreoSmtp : IEnviarCorreoSmtp
    {
        private readonly PilaCorreos _pilaCorreos = new PilaCorreos();
        public void RegistroTrabajador(string email, string password)
        {
            var asunto = "Lavandería Yast Cleaner - Registro de Trabajador";
            var cuerpo = $"Su contraseña: {password}";

            EnviarOEncolar(new Correo
            {
                Destinatario = email,
                Asunto = asunto,
                Cuerpo = cuerpo
            });
        }

        public void RegistroPedido(string email)
        {
            var asunto = "Lavandería Yast Cleaner - Confirmación de Pedido";
            var cuerpo = "Su pedido ha sido registrado con éxito.";

            EnviarOEncolar(new Correo
            {
                Destinatario = email,
                Asunto = asunto,
                Cuerpo = cuerpo
            });
        }
        private void EnviarOEncolar(Correo correo)
        {
            if (!ConexionInternetHelper.HayConexionInternet())
            {
                _pilaCorreos.Agregar(correo);
                return;
            }

            try
            {
                EnviarCorreo(correo);
                ReintentarPendientes();
            }
            catch
            {
                _pilaCorreos.Agregar(correo);
            }
        }
        private void EnviarCorreo(Correo correo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Lavandería Yast Cleaner", "blasasto0914@gmail.com"));
            mensaje.To.Add(new MailboxAddress("Destino", correo.Destinatario));
            mensaje.Subject = correo.Asunto;
            mensaje.Body = new TextPart("plain") { Text = correo.Cuerpo };

            using (var cliente = new SmtpClient())
            {
                cliente.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                cliente.Authenticate("blasasto0914@gmail.com", "fetv ozub yhym gouj");
                cliente.Send(mensaje);
                cliente.Disconnect(true);
            }
        }
        private void ReintentarPendientes()
        {
            //Si hay conexion a internet, intentar enviar los correos pendientes
            while (_pilaCorreos.Pendientes() && ConexionInternetHelper.HayConexionInternet())
            {
                var correoPendiente = _pilaCorreos.Obtener();
                if (correoPendiente != null)//si hay correo pendiente
                {
                    try
                    {
                        EnviarCorreo(correoPendiente);
                    }
                    catch
                    {
                        _pilaCorreos.Agregar(correoPendiente);
                        break;
                    }
                }
            }
        }

    }
}
