using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class GetPaidCommand : IBotCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings.LucyTelegramBotSettings _settings;

        public GetPaidCommand(ITelegramBotClient botClient, IBlobStorage blobStorage, AppSettings.LucyTelegramBotSettings settings)
        {
            _botClient = botClient;
            _blobStorage = blobStorage;
            _settings = settings;
        }

        public IEnumerable<string> SupportedCommands
        {
            get { yield return BotCommands.GetPaid; }
        }

        public async Task Execute(Message message)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.GetPaid["Hello"], ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);

            var template = await _blobStorage.GetAsync("templates", "invoice.xlsx");
            var file = new FileToSend("invoice.xlsx", template);

            await _botClient.SendDocumentAsync(message.Chat.Id, file);
            await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.GetPaid["SendInfo"], ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
        }
    }
}