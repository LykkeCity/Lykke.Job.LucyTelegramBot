using System.Threading.Tasks;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{    
    public interface IMessagePoller
    {
        Task PullAsync();
    }
}