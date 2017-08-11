using System.Linq;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Newtonsoft.Json;
using Telegram.Bot;

namespace Lykke.Job.LucyTelegramBot.Services
{    
    public class MessagePoller : IMessagePoller
    {
        private readonly IOffsetRepository _offsetRepository;
        private readonly IQueueExt _queueExt;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IHandledMessagesRepository _handledMessagesRepository;

        public MessagePoller(IOffsetRepository offsetRepository, IQueueExt queueExt, ITelegramBotClient telegramBotClient, IHandledMessagesRepository handledMessagesRepository)
        {
            _offsetRepository = offsetRepository;
            _queueExt = queueExt;
            _telegramBotClient = telegramBotClient;
            _handledMessagesRepository = handledMessagesRepository;
        }

        public async Task PullAsync()
        {
            var offset = await _offsetRepository.GetOffset();
            var updates = await _telegramBotClient.GetUpdatesAsync(offset + 1);
            
            foreach (var update in updates.OrderBy(o => o.Id))
            {                
                var message = update.Message;

                if (message == null)
                    continue;

                if (await _handledMessagesRepository.TryHandleMessage(update.Message.MessageId))
                {
                    await _queueExt.PutRawMessageAsync(JsonConvert.SerializeObject(message));
                }

                if (update.Id > offset)
                    await _offsetRepository.SetOffset(update.Id);
            }            
        }
    }
}