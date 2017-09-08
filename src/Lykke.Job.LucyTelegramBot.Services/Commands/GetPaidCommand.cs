using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class GetPaidCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly IBlobStorage _blobStorage;

        public GetPaidCommand(
            IBotService botService,
            IBlobStorage blobStorage)
        {
            _botService = botService;
            _blobStorage = blobStorage;
        }

        public string[] SupportedCommands => new[] { BotCommands.GetPaid };

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            await _botService.SendTextMessageAsync(message, command, command.IntroText);

            try
            {
                var template = await _blobStorage.GetAsync("templates", "invoice.xlsx");
                var file = new FileToSend("invoice template.xlsx", template);

                await _botService.SendDocumentAsync(message, file);
            }
            catch
            {
            }

            await _botService.SendTextMessageAsync(message, command, command.ReplyText);
        }

        public Task Reply(LykkeBotCommand command, Message message)
        {
            return Task.FromResult(0);
        }
    }
}
