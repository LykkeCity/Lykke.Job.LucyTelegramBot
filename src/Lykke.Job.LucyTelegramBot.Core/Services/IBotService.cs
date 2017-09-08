using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Core.Services
{
    public interface IBotService
    {
        Task ProcessMessageAsync(Message message);
        Task SendTextMessageAsync(Message message, LykkeBotCommand command, string text);
        Task SendDocumentAsync(Message message, FileToSend file);
        Task ExecuteCommandAsync(LykkeBotCommand command, Message message);
        Task ReplyCommandAsync(LykkeBotCommand command, Message message);
    }
}
