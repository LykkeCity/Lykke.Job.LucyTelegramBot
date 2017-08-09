using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class InfoCommand : IBotCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ITgEmployeeRepository _employeeRepository;
        private readonly AppSettings.LucyTelegramBotSettings _settings;

        public InfoCommand(ITelegramBotClient botClient, ITgEmployeeRepository employeeRepository, AppSettings.LucyTelegramBotSettings settings)
        {
            _botClient = botClient;
            _employeeRepository = employeeRepository;
            _settings = settings;
        }

        public IEnumerable<string> SupportedCommands
        {
            get { yield return BotCommands.GetUserInfo; }
        }
        public async Task Execute(Message message)
        {
            var username = string.Join(string.Empty, message.Text.Skip(BotCommands.GetUserInfo.Length + 1));
            var userinfo = await _employeeRepository.Find(username);

            if (userinfo != null)
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, $"{userinfo.FirstName} {userinfo.LastName}: {userinfo.Bio}", ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
            }
            else
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.UserNotFound, ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
            }
        }
    }
}