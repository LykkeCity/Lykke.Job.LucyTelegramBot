using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services
{    
    public class MessageHandler : IMessageHandler
    {
        private readonly IBotCommandFactory _botCommandFactory;

        public MessageHandler(IBotCommandFactory botCommandFactory)
        {
            _botCommandFactory = botCommandFactory;
        }

        public async Task HandleAsync(string update)
        {
            var message = JsonConvert.DeserializeObject<Message>(update);

            var command = _botCommandFactory.GetCommand(message.Text);

            if (command != null)
                await command.Execute(message);
        }
    }
}