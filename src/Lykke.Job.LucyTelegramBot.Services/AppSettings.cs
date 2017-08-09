using System.Collections.Generic;

namespace Lykke.Job.LucyTelegramBot.Services
{
    public class AppSettings
    {
        public LucyTelegramBotSettings LucyTelegramBotJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }

        public GoogleAuthSettings GoogleAuthSettings { get; set; }

        public class LucyTelegramBotSettings
        {
            public DbSettings Db { get; set; }
            public string TriggerQueueConnectionString { get; set; }
            public string BotName { get; set; }
            public string Token { get; set; }            
            public Messages Messages { get; set; }

        }

        public class Messages
        {
            public string Start { get; set; }
            public Dictionary<string, string> GetPaid { get; set; }
            public string Contractors { get; set; }
            public string NewbieGuide { get; set; }
            public string Auth { get; set; }
            public string BioAccepted { get; set; }
            public string UserNotFound { get; set; }
        }

        public class DbSettings
        {
            public string DataConnString { get; set; }
            public string LogsConnString { get; set; }            
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }       
    }

    public class GoogleAuthSettings
    {
        public string ApiClientId { get; set; }

        public string AvailableEmailsRegex { get; set; }

        public string DefaultAdminEmail { get; set; }
    }
}