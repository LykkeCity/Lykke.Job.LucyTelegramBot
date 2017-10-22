using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Common.Log;
using AzureStorage.Queue;
using Lykke.Job.LucyTelegramBot.Core.Telegram;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public class MessagePoller : IMessagePoller
    {
        private readonly IOffsetRepository _offsetRepository;
        private readonly IQueueExt _queueExt;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IHandledMessagesRepository _handledMessagesRepository;
        private readonly ILog _log;
        private readonly TimeSpan _warningDelay = TimeSpan.FromMinutes(1);
        private DateTime _lastWarning = DateTime.MinValue;

        public MessagePoller(
            IOffsetRepository offsetRepository,
            IQueueExt queueExt,
            ITelegramBotClient telegramBotClient,
            IHandledMessagesRepository handledMessagesRepository,
            ILog log)
        {
            _offsetRepository = offsetRepository;
            _queueExt = queueExt;
            _telegramBotClient = telegramBotClient;
            _handledMessagesRepository = handledMessagesRepository;
            _log = log;
        }

        public async Task PullAsync()
        {
            try
            {
                var offset = await _offsetRepository.GetOffset();
                var updates = await _telegramBotClient.GetUpdatesAsync(offset + 1);
                foreach (var update in updates.OrderBy(o => o.Id))
                {
                    var message = update.Message;

                    if (message == null)
                        continue;

                    if (await _handledMessagesRepository.TryHandleMessage(update.Message.MessageId))
                        await _queueExt.PutRawMessageAsync(JsonConvert.SerializeObject(message));

                    if (update.Id > offset)
                        await _offsetRepository.SetOffset(update.Id);
                }
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.UtcNow;
                if (now.Subtract(_lastWarning) > _warningDelay)
                {
                    _lastWarning = now;
                    await _log.WriteWarningAsync(nameof(MessagePoller), nameof(PullAsync), ex.GetBaseException().Message);
                }
            }
        }
    }
}