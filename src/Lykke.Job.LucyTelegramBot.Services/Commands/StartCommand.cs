using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lykke.Job.LucyTelegramBot.Services.Commands
{
    public class StartCommand : IBotCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppSettings.LucyTelegramBotSettings _settings;
        private readonly ITgEmployeeRepository _employeeRepository;

        public StartCommand(ITelegramBotClient botClient, AppSettings.LucyTelegramBotSettings settings, ITgEmployeeRepository employeeRepository)
        {
            _botClient = botClient;
            _settings = settings;
            _employeeRepository = employeeRepository;
        }

        public IEnumerable<string> SupportedCommands
        {
            get { yield return BotCommands.Start; }
        }

        public async Task Execute(Message message)
        {
            var chatId = message.Chat.Id;
            var id = string.Join(string.Empty, message.Text.Skip(BotCommands.Start.Length + 1));

            if (!string.IsNullOrWhiteSpace(id))
                await _employeeRepository.UpdateEmployeeInfo(id, chatId, message.From.Username, message.From.FirstName, message.From.LastName);

            var emp = !string.IsNullOrWhiteSpace(id)
                ? await _employeeRepository.Get(id)
                : await _employeeRepository.Get(chatId);
                        
            if (emp != null)
            {
                await _botClient.SendTextMessageAsync(chatId, _settings.Messages.Start, ParseMode.Default, false, false, 0, KeyBoards.MainKeyboard);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, _settings.Messages.Auth);
            }            
        }
    }
}