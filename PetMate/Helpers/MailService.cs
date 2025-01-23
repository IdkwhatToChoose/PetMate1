using System.Configuration;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net;

namespace PetMate.Helpers
{
    public class MailService
    {
        private readonly IConfiguration _config;
        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string subject, string client_name, string client_email, string client_message)
        {
            string appass = _config["appass"];

            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("petmate821@gmail.com", appass);
                using (var message = new MailMessage(
                    from: new MailAddress(client_email,client_name),
                    to: new MailAddress("petmate821@gmail.com", "Stanislav")
                    ))
                {

                    message.Subject = subject;
                    message.Body = client_message;

                    client.Send(message);
                }
            }
        }
    }
}
