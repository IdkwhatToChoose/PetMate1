using System.Configuration;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net;
using PetMate.ViewModels;
using static OpenAI.GPT3.ObjectModels.Models;
using PetMate.Model;

namespace PetMate.Helpers
{
    public class MailService
    {
        private readonly IConfiguration _config;
        public MailService(IConfiguration config)
        {
            _config = config;
        }
        private readonly string _appass = Environment.GetEnvironmentVariable("apppassG");
        public void SendContactEmail(string subject, string client_name, string client_email, string client_message)
        {

            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("petmate821@gmail.com", _appass);
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

        public async Task SendSuccessfullDonationNotification(string recieverEmail,string recieverName,string donationAmount,string petName)
        {
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("petmate821@gmail.com", _appass);
                using (var message = new MailMessage(
                    from: new MailAddress("petmate821@gmail.com", "Stanislav"),
                    to: new MailAddress(recieverEmail, recieverName)
                ))
                {

                    message.Subject = "Успешно дарение 🎉 !";
                    message.Body = $"Успешно получихме Вашето дарение на стойност {donationAmount} лв. в подкрепа на кучето {petName}. Вашата щедрост ще допринесе за неговото здраве, храна и по-добри условия, докато чака своето ново, любящо семейство. От сърце Ви благодарим, че сте част от промяната!";

                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task<int> Send2faCode(string recieverEmail)
        {
            int code = new Random().Next(100000, 999999);
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("petmate821@gmail.com", _appass);
                using (var message = new MailMessage(
                    from: new MailAddress("petmate821@gmail.com", "Stanislav"),
                    to: new MailAddress(recieverEmail)
                ))
                {
                    message.IsBodyHtml = true;
                    message.Subject = "Потвърдете вашият имейл";
                    message.Body = $"Потвърдете че това сте вие.\n Въведете кодът: <strong> {code} </strong>. Той ще изтече след 15 минути.";

                    await client.SendMailAsync(message);
                    return code;
                }
            }
        }

        // C# example using SMTP check
        //public async Task<bool> Verify(string email)
        //{
        //    try
        //    {
        //        using var client = new System.Net.Mail.SmtpClient("gmail-smtp-in.l.google.com", 25);
        //        client.
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
