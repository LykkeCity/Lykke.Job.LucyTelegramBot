using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lykke.Job.LucyTelegramBot.Core;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public class KeyboardsFactory
    {
        private readonly LucyTelegramBotSettings _settings;

        public KeyboardsFactory(LucyTelegramBotSettings settings)
        {
            _settings = settings;
        }

        public ReplyKeyboardMarkup GetKeyboard(string command)
        {
            if (string.IsNullOrEmpty(command))
                return new ReplyKeyboardMarkup(new []{new KeyboardButton(BotCommands.StartWord)}, true);

            var botCommand = _settings.Commands.FirstOrDefault(
                item => item.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase)) ?? new LykkeBotCommand();

            List<string> commands;
            var parentCommand = _settings.Commands.FirstOrDefault(item => item.Commands.Contains(command));

            if (botCommand.Commands.Length == 0)
            {
                if (parentCommand == null)
                    return new ReplyKeyboardMarkup(new []{ new KeyboardButton(BotCommands.Back) });

                commands = parentCommand.Commands.ToList();
            }
            else
            {
                commands = botCommand.Commands.ToList();
            }

            if (parentCommand != null && (botCommand.Commands.Any() || _settings.Commands.Any(item => item.Commands.Contains(parentCommand.Name))))
                commands.Add(BotCommands.Back);

            var markup = new ReplyKeyboardMarkup();

            var rowsCount = commands.Any()
                ? commands.Count / 2 + (commands.Count % 2 == 0 ? 0 : 1)
                : 1;

            markup.Keyboard = new KeyboardButton[rowsCount][];
            markup.ResizeKeyboard = true;

            for (var i = 0; i < rowsCount; i++)
            {
                markup.Keyboard[i] = commands.Skip(i * 2).Take(2).Select(item => new KeyboardButton(item))
                    .ToArray();
            }

            return markup;
        }
    }
}
