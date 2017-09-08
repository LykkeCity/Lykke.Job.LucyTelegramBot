using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.Services
{    
    public class MessageHandler : IMessageHandler
    {
        private readonly LucyTelegramBotSettings _settings;
        private readonly IBotService _botService;
        private readonly ITgEmployeeRepository _employeeRepository;

        public MessageHandler(
            LucyTelegramBotSettings settings,
            IBotService botService, ITgEmployeeRepository employeeRepository)
        {
            _settings = settings;
            _botService = botService;
            _employeeRepository = employeeRepository;
        }

        public async Task HandleAsync(string update)
        {
            var message = JsonConvert.DeserializeObject<Message>(update);

            string id = string.Empty;

            if (message.Text.StartsWith(BotCommands.Start))
            {
                id = string.Join(string.Empty, message.Text.Skip(BotCommands.Start.Length + 1));
            }

            var emp = !string.IsNullOrWhiteSpace(id)
                ? await _employeeRepository.Get(id)
                : await _employeeRepository.Get(message.Chat.Id);

            if (emp == null)
                await _botService.SendTextMessageAsync(message, null, _settings.Messages.Auth);
            else
                await _botService.ProcessMessageAsync(message);
        }
    }
}
