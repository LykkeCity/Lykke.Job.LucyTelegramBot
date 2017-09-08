using System.Threading.Tasks;

namespace Lykke.Job.LucyTelegramBot.Core.Services
{
    public interface IEmailFacadeService
    {
        Task SendFeedbackEmail(string email, string name, string message);
    }
}
