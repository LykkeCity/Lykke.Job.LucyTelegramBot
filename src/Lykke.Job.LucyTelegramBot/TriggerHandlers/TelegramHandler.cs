using System.Threading.Tasks;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.JobTriggers.Triggers.Attributes;
using Telegram.Bot.Types;

namespace Lykke.Job.LucyTelegramBot.TriggerHandlers
{    
    public class TelegramHandler
    {
        private readonly IMessagePoller _messagePoller;
        private readonly IMessageHandler _messageHandler;
        
        public TelegramHandler(IMessagePoller messagePoller, IMessageHandler messageHandler)
        {
            _messagePoller = messagePoller;
            _messageHandler = messageHandler;            
        }

        [TimerTrigger("00:00:01")]
        public async Task GetUpdates()
        {
            await _messagePoller.PullAsync();                
        }

        [QueueTrigger("lucy-tg-updates")]
        public async Task QueueTriggeredHandler(string msg)
        {            
            await _messageHandler.HandleAsync(msg);            
        }
    }
}