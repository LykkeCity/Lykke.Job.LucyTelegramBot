using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class StartCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly ITgEmployeeRepository _employeeRepository;

        public StartCommand(
            IBotService botService,
            KeyboardsFactory keyboardsFactory,
            ITgEmployeeRepository employeeRepository)
        {
            _botService = botService;
            _employeeRepository = employeeRepository;
        }

        public string[] SupportedCommands => new[] { BotCommands.Start, BotCommands.StartWord };

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            var chatId = message.Chat.Id;
            var id = string.Join(string.Empty, message.Text.Skip(BotCommands.Start.Length + 1));

            if (!string.IsNullOrWhiteSpace(id))
                await _employeeRepository.UpdateEmployeeInfo(id, chatId, message.From.Username, message.From.FirstName, message.From.LastName);

            await _botService.SendTextMessageAsync(message, command, command.IntroText);
        }

        public Task Reply(LykkeBotCommand command, Message message)
        {
            return Task.FromResult(0);
        }
    }
}
