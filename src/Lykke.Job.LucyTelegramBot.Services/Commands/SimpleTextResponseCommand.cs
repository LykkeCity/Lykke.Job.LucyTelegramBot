using System;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class SimpleTextResponseCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;

        public SimpleTextResponseCommand(IBotService botService)
        {
            _botService = botService;
        }

        public string[] SupportedCommands => Array.Empty<string>();

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            await _botService.SendTextMessageAsync(message, command, command.IntroText);
        }

        public Task<bool> Reply(LykkeBotCommand command, Message message)
        {
            return Task.FromResult(true);
        }
    }
}
