using System.Collections.Generic;
using System.Linq;
using Lykke.Job.LucyTelegramBot.Core.Telegram;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public interface IBotCommandFactory
    {
        IBotCommand GetCommand(string botCommand);
    }

    public class BotCommandFactory : IBotCommandFactory
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public BotCommandFactory(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        public IBotCommand GetCommand(string botCommand)
        {
            return _commands.FirstOrDefault(command => command.SupportedCommands.Any(botCommand.StartsWith));
        }
    }
}