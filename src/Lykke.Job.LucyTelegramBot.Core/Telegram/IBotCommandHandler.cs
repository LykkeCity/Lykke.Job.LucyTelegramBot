using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface IBotCommandHandler
    {
        string[] SupportedCommands { get; }
        Task Execute(LykkeBotCommand command, Message message);
        Task<bool> Reply(LykkeBotCommand command, Message message);
    }
}