using MailKit.Net.Smtp;
using MimeKit;
using YastCleaner.Business.Interfaces;

namespace YastCleaner.Business.Services
{
    public class EnviarCorreoSmtp : IEnviarCorreoSmtp
    {
        public void EnviarCorreo(string email, string password)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Lavandería Yast Cleaner", "blasasto0914@gmail.com"));//aqui el correo debe cambiar
            mensaje.To.Add(new MailboxAddress("Destino", email));
            mensaje.Subject = "Lavandería Yast Cleaner";

            mensaje.Body = new TextPart("plain")
            {
                Text = $"Su contraseña única de trabajador es: {password}"
            };

            using (var cliente = new SmtpClient())
            {
                cliente.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                cliente.Authenticate("blasasto0914@gmail.com", "fetv ozub yhym gouj");

                cliente.Send(mensaje);
                cliente.Disconnect(true);
            }
        }
    }
}
