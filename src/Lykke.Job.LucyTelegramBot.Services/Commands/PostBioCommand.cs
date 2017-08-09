using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class PostBioCommand : IBotCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ITgEmployeeRepository _employeeRepository;
        private readonly AppSettings.LucyTelegramBotSettings _settings;

        public PostBioCommand(ITelegramBotClient botClient, ITgEmployeeRepository employeeRepository, AppSettings.LucyTelegramBotSettings settings)
        {
            _botClient = botClient;
            _employeeRepository = employeeRepository;
            _settings = settings;
        }

        public IEnumerable<string> SupportedCommands
        {
            get { yield return BotCommands.PostUserInfo; }
        }
        public async Task Execute(Message message)
        {
            var bio = string.Join(string.Empty, message.Text.Skip(BotCommands.PostUserInfo.Length + 1));

            if (!string.IsNullOrWhiteSpace(bio))
            {
                await _employeeRepository.UpdateBio(message.Chat.Id, bio);

                await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.BioAccepted, ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
            }
        }
    }
}