using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace EventEase_01.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        public EmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587) 
            {
                Credentials = new NetworkCredential("eventease80@gmail.com", "pueyelycviovprrm"),
                EnableSsl = true
            };
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MailMessage("eventease80@gmail.com", toEmail, subject, body);
            message.IsBodyHtml = true;
            await _smtpClient.SendMailAsync(message);
        }
    }
}

