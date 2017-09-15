using System.Threading.Tasks;
using Common.RemoteUi;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class InfoCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly ITgEmployeeRepository _employeeRepository;
        private readonly LucyTelegramBotSettings _settings;

        public InfoCommand(
            IBotService botService,
            ITgEmployeeRepository employeeRepository, 
            LucyTelegramBotSettings settings)
        {
            _botService = botService;
            _employeeRepository = employeeRepository;
            _settings = settings;
        }

        public string[] SupportedCommands => new[] { BotCommands.GetUserInfo, BotCommands.ReadBio };

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            await _botService.SendTextMessageAsync(message, command, command.IntroText);
        }

        public async Task<bool> Reply(LykkeBotCommand command, Message message)
        {
            var userinfo = await _employeeRepository.Find(message.Text);

            string text = userinfo != null
                ? $"*{userinfo.FirstName} {userinfo.LastName}:*\r\n{userinfo.Bio ?? _settings.Messages.NoUserInfo}"
                : _settings.Messages.UserNotFound;

            await _botService.SendTextMessageAsync(message, command, text);
            //don't clear the state
            return false;
        }
    }
}
