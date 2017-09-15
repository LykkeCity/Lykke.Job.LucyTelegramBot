using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public class BotService : IBotService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IBotCommandHandlerFactory _botCommandHandlerFactory;
        private readonly KeyboardsFactory _keyboardsFactory;
        private readonly ITgUserStateRepository _userStateRepository;
        private readonly LucyTelegramBotSettings _settings;

        public BotService(
            ITelegramBotClient botClient,
            IBotCommandHandlerFactory botCommandHandlerFactory,
            KeyboardsFactory keyboardsFactory,
            ITgUserStateRepository userStateRepository,
            LucyTelegramBotSettings settings)
        {
            _botClient = botClient;
            _botCommandHandlerFactory = botCommandHandlerFactory;
            _keyboardsFactory = keyboardsFactory;
            _userStateRepository = userStateRepository;
            _settings = settings;
        }

        public async Task ProcessMessageAsync(Message message)
        {
            var command = _settings.Commands.FirstOrDefault(item => message.Text.StartsWith(item.Name, StringComparison.InvariantCultureIgnoreCase));

            if (command != null)
            {
                await ExecuteCommandAsync(command, message);
                return;
            }

            var state = await _userStateRepository.GetStateAsync(message.Chat.Id);

            if (state != null)
            {
                command = _settings.Commands.FirstOrDefault(item => item.Name.Equals(state.Command, StringComparison.InvariantCultureIgnoreCase));

                if (command != null && (command.HasReply || !string.IsNullOrEmpty(command.ReplyText)))
                {
                    await ReplyCommandAsync(command, message);
                    return;
                }
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, _settings.Messages.CommandNotFound, ParseMode.Markdown);
        }

        public async Task SendTextMessageAsync(Message message, LykkeBotCommand command, string text)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, text, ParseMode.Markdown, false, false, 0, _keyboardsFactory.GetKeyboard(command?.Name));
        }

        public async Task SendDocumentAsync(Message message, FileToSend file)
        {
            await _botClient.SendDocumentAsync(message.Chat.Id, file);
        }

        public async Task ExecuteCommandAsync(LykkeBotCommand command, Message message)
        {
            var handler = _botCommandHandlerFactory.GetCommand(command.Name);
            await handler.Execute(command, message);
            var parentCommand = _settings.Commands.FirstOrDefault(item => item.Commands.Contains(command.Name));
            await _userStateRepository.SetStateAsync(message.Chat.Id, parentCommand?.Name ?? command.Name, command.Name);
        }

        public async Task ReplyCommandAsync(LykkeBotCommand command, Message message)
        {
            var handler = _botCommandHandlerFactory.GetCommand(command.Name);
            bool processed = await handler.Reply(command, message);
            var parentCommand = _settings.Commands.FirstOrDefault(item => item.Commands.Contains(command.Name));
            await _userStateRepository.SetStateAsync(message.Chat.Id, parentCommand?.Name, processed ? null : command.Name);

        }
    }
}
