using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Telegram.Bot;
using Common.Log;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Lykke.SettingsReader;
using Lykke.Service.EmailSender;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram;
using Lykke.Job.LucyTelegramBot.Services;
using Lykke.Job.LucyTelegramBot.Services.Commands;

namespace Lykke.Job.LucyTelegramBot.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;
        private readonly IReloadingManager<AppSettings> _settingsManager;

        public JobModule(IReloadingManager<AppSettings> settingsManager, ILog log)
        {
            _settingsManager = settingsManager;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings.LucyTelegramBotJob)
                .SingleInstance();
            builder.RegisterInstance(_settings.GoogleAuthSettings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            var offsetRecordStorage = AzureTableStorage<OffsetRecord>.Create(
                _settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.DataConnString), "TgUpdatesOffset", _log);
            builder.RegisterInstance(new OffsetRepository(offsetRecordStorage)).As<IOffsetRepository>();
            var tgEmployeeStorage = AzureTableStorage<TgEmployee>.Create(
                _settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.DataConnString), "TgEmployees", _log);
            builder.RegisterInstance(new TgEmployeeRepository(tgEmployeeStorage)).As<ITgEmployeeRepository>();
            var handledMessageRecordStorage = AzureTableStorage<HandledMessageRecord>.Create(
                _settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.DataConnString), "TgHandledMessages", _log);
            builder.RegisterInstance(new HandledMessagesRepository(handledMessageRecordStorage)).As<IHandledMessagesRepository>();
            var tgUserStateEntityStorage = AzureTableStorage<TgUserStateEntity>.Create(
                _settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.DataConnString), "TgUserStates", _log);
            builder.RegisterInstance(new TgUserStateRepository(tgUserStateEntityStorage)).As<ITgUserStateRepository>();
            var blobStorage = AzureBlobStorage.Create(_settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.DataConnString));
            builder.RegisterInstance(blobStorage).As<IBlobStorage>();
            var queue = AzureQueueExt.Create(
                _settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.TriggerQueueConnectionString), "lucy-tg-updates");
            builder.RegisterInstance(queue).As<IQueueExt>();

            builder.RegisterType<MessageHandler>()
                .As<IMessageHandler>();
            builder.RegisterType<MessagePoller>()
                .As<IMessagePoller>();

            builder.RegisterType<BotService>().As<IBotService>().SingleInstance();
            builder.RegisterType<BotCommandHandlerFactory>().As<IBotCommandHandlerFactory>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<KeyboardsFactory>().AsSelf().SingleInstance();

            builder.RegisterType<SimpleTextResponseCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<BackCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<StartCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<InfoCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<PostBioCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<FeedbackCommand>().As<IBotCommandHandler>().SingleInstance();
            builder.RegisterType<TeamOverviewCommand>().As<IBotCommandHandler>().SingleInstance();

            builder.RegisterType<EmailFacadeService>().As<IEmailFacadeService>();

            // NOTE: You can implement your own poison queue notifier. See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            var telegramBot = new TelegramBotClient(_settings.LucyTelegramBotJob.Token);
            telegramBot.SetWebhookAsync(string.Empty).Wait();

            builder.RegisterInstance(telegramBot).As<ITelegramBotClient>();
            builder.RegisterEmailSenderService(_settings.LucyTelegramBotJob.EmailSenderServiceUrl, _log);

            builder.Populate(_services);
        }
    }
}
