using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Service.EmailSender;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public class EmailFacadeService : IEmailFacadeService
    {
        private readonly IEmailSender _emailSender;

        public EmailFacadeService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendFeedbackEmail(string email, string name, string message)
        {
            await _emailSender.SendAsync(new EmailMessage
            {
                HtmlBody = $"Feedback frequest from {name}:<br>{message}",
                Subject = "Feedback"
            }, new EmailAddressee {DisplayName = name, EmailAddress = email});
        }
    }
}
