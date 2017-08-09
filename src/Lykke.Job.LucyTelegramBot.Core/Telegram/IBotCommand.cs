using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Core.Telegram
{
    public interface IBotCommand
    {
        IEnumerable<string> SupportedCommands { get; }

        Task Execute(Message message);
    }
}