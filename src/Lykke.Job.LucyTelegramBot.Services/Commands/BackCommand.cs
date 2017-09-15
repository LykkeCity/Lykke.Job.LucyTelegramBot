using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class BackCommand : IBotCommandHandler
    {
        private readonly IBotService _botService;
        private readonly LucyTelegramBotSettings _settings;
        private readonly ITgUserStateRepository _userStateRepository;

        public BackCommand(
            IBotService botService,
            LucyTelegramBotSettings settings,
            ITgUserStateRepository userStateRepository)
        {
            _botService = botService;
            _settings = settings;
            _userStateRepository = userStateRepository;
        }

        public string[] SupportedCommands => new[] {BotCommands.Back};

        public async Task Execute(LykkeBotCommand command, Message message)
        {
            var state = await _userStateRepository.GetStateAsync(message.Chat.Id);

            if (state != null)
            {
                var parentCommand = _settings.Commands.FirstOrDefault(item => item.Commands.Contains(state.ParentCommand))
                    ?? _settings.Commands.FirstOrDefault(item => item.Name.Equals(state.ParentCommand, StringComparison.InvariantCultureIgnoreCase));

                if (parentCommand != null)
                {
                    await _botService.ExecuteCommandAsync(parentCommand, message);
                }
            }
        }

        public Task<bool> Reply(LykkeBotCommand command, Message message)
        {
            return Task.FromResult(true);
        }
    }
}
