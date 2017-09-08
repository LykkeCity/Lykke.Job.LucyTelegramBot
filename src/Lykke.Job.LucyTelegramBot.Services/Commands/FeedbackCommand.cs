using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class FeedbackCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly IEmailFacadeService _emailFacadeService;
        private readonly ITgEmployeeRepository _employeeRepository;
        private readonly KeyboardsFactory _keyboardsFactory;
        private readonly LucyTelegramBotSettings _settings;

        public FeedbackCommand(
            IBotService botService,
            IEmailFacadeService emailFacadeService,
            ITgEmployeeRepository employeeRepository, 
            KeyboardsFactory keyboardsFactory,
            LucyTelegramBotSettings settings)
        {
            _botService = botService;
            _emailFacadeService = emailFacadeService;
            _employeeRepository = employeeRepository;
            _keyboardsFactory = keyboardsFactory;
            _settings = settings;
        }

        public string[] SupportedCommands => new[] {BotCommands.Feedback};

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            await _botService.SendTextMessageAsync(message, command, command.IntroText);
        }

        public async Task Reply(LykkeBotCommand command, Message message)
        {
            try
            {
                await _emailFacadeService.SendFeedbackEmail(_settings.FeedbackEmail,
                    $"{message.From.LastName} {message.From.FirstName}", message.Text);
            }
            catch
            {
                await _botService.SendTextMessageAsync(message, command, _settings.Messages.ErrorSendingFeedback);
                return;
            }

            await _botService.SendTextMessageAsync(message, command, command.ReplyText);
        }
    }
}
