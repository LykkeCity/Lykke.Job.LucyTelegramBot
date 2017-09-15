using System;
using System.Collections.Generic;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.LucyTelegramBot.Core
{
    public class AppSettings
    {
        public LucyTelegramBotSettings LucyTelegramBotJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }

        public GoogleAuthSettings GoogleAuthSettings { get; set; }
    }

    public class LucyTelegramBotSettings
    {
        public DbSettings Db { get; set; }
        public string EmailSenderServiceUrl { get; set; }
        public string BotName { get; set; }
        public string Token { get; set; }
        public string FeedbackEmail { get; set; }
        public Messages Messages { get; set; }
        public LykkeBotCommand[] Commands { get; set; } = Array.Empty<LykkeBotCommand>();
    }

    public class Messages
    {
        public string Auth { get; set; }
        public string UserNotFound { get; set; }
        public string CommandNotFound { get; set; }
        public string NoUserInfo { get; set; }
        public string ErrorSendingFeedback { get; set; }
    }

    public class LykkeBotCommand
    {
        public string Name { get; set; }
        [Optional]
        public string IntroText { get; set; }
        [Optional]
        public string ReplyText { get; set; }
        [Optional]
        public string[] Commands { get; set; } = Array.Empty<string>();
        [Optional]
        public bool HasReply { get; set; }
    }

    public class DbSettings
    {
        public string DataConnString { get; set; }
        public string LogsConnString { get; set; }
        public string TriggerQueueConnectionString { get; set; }
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


    public class GoogleAuthSettings
    {
        public string ApiClientId { get; set; }

        public string AvailableEmailsRegex { get; set; }

        public string DefaultAdminEmail { get; set; }
    }
}