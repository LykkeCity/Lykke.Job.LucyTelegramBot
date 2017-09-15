using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Domain;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class TeamOverviewCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly ITgEmployeeRepository _employeeRepository;
        private readonly LucyTelegramBotSettings _settings;

        public TeamOverviewCommand(
            IBotService botService,
            ITgEmployeeRepository employeeRepository, 
            LucyTelegramBotSettings settings)
        {
            _botService = botService;
            _employeeRepository = employeeRepository;
            _settings = settings;
        }

        public string[] SupportedCommands => new[] { BotCommands.TeamOverview };

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            var employees = await _employeeRepository.GetEmployeesWithBio();
            var sb = new StringBuilder();

            foreach (var employee in employees.OrderBy(item => item.FirstName))
                sb.AppendLine($"{employee.FullName()} (@{employee.UserName})");

            await _botService.SendTextMessageAsync(message, command, command.IntroText);
            await _botService.SendTextMessageAsync(message, command, sb.ToString());
        }

        public async Task<bool> Reply(LykkeBotCommand command, Message message)
        {
            var userinfo = await _employeeRepository.Find(message.Text);

            string text = userinfo != null
                ? $"*{userinfo.FullName()}:*\r\n{userinfo.Bio ?? _settings.Messages.NoUserInfo}"
                : _settings.Messages.UserNotFound;

            await _botService.SendTextMessageAsync(message, command, text);
            return true;
        }
    }
}
