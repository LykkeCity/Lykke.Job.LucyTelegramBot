using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class PostBioCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly ITgEmployeeRepository _employeeRepository;

        public PostBioCommand(
            IBotService botService,
            ITgEmployeeRepository employeeRepository)
        {
            _botService = botService;
            _employeeRepository = employeeRepository;
        }

        public string[] SupportedCommands => new[] { BotCommands.PostUserBio };

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            await _botService.SendTextMessageAsync(message, command, command.IntroText);
        }

        public async Task Reply(LykkeBotCommand command, Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                await _employeeRepository.UpdateBio(message.Chat.Id, message.Text);
                await _botService.SendTextMessageAsync(message, command, command.ReplyText);
            }
        }
    }
}
