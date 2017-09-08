using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Job.LucyTelegramBot.Core.Telegram;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public interface IBotCommandHandlerFactory
    {
        IBotCommandHandler GetCommand(string botCommand);
    }

    public class BotCommandHandlerFactory : IBotCommandHandlerFactory
    {
        public IEnumerable<IBotCommandHandler> Handlers { get; set; }

        public IBotCommandHandler GetCommand(string botCommand)
        {
            var handler = Handlers.FirstOrDefault(command => command.SupportedCommands.Any(c => botCommand.Equals(c, StringComparison.InvariantCultureIgnoreCase)));

            return handler ?? Handlers.FirstOrDefault(item => item.SupportedCommands.Length == 0);
        }
    }
}