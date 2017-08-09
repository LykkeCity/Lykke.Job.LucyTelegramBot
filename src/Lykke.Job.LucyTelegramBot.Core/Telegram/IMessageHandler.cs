using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Core.Services
{    
    public interface IMessageHandler
    {
        Task HandleAsync(string update);
    }
}