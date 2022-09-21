using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.Mensajeria.Email.Interface;
using TiendaServicios.Mensajeria.Email.Modelo;

namespace TiendaServicios.Mensajeria.Email.Implement
{
    internal class SendGridEnviar : ISendGridEnviar
    {
        public async Task<(bool resultado, string errorMessage)> EnviarEmail(SendGridData data)
        {
            try {
                var sendGridCliente = new SendGridClient(data.SendGridAPIKey);
                var destinatario = new EmailAddress(data.EmailDestinatario, data.NombreDestinatario);
                var tituloEmail = data.Titulo;
                var sender = new EmailAddress("correo", "nombre");
                var contenidoMensaje = data.Contenido;

                var objMensaje = MailHelper.CreateSingleEmail(sender, destinatario, tituloEmail, contenidoMensaje, contenidoMensaje);

                await sendGridCliente.SendEmailAsync(objMensaje);

                return (true, null);
            } catch (Exception e) { 
                return (false, e.Message);
            }
        }
    }
}
