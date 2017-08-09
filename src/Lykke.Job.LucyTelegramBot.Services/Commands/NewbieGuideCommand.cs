using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class NewbieGuideCommand : IBotCommand
    {
        private ITelegramBotClient _botClient;
        private AppSettings.LucyTelegramBotSettings _settings;

        public NewbieGuideCommand(ITelegramBotClient botClient, AppSettings.LucyTelegramBotSettings settings)
        {
            _botClient = botClient;
            _settings = settings;            
        }

        public IEnumerable<string> SupportedCommands
        {
            get { yield return BotCommands.NewbieGuide; }
        }

        public async Task Execute(Message message)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.NewbieGuide, ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
        }
    }
}