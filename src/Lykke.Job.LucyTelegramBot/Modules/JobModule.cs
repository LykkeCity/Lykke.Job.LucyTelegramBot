using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.Job.LucyTelegramBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Queue;
using Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Services.Commands;
using Lykke.Service.EmailSender;

namespace Lykke.Job.LucyTelegramBot.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(AppSettings settings, ILog log)
        {
            _settings = settings;
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

            builder.RegisterInstance(new OffsetRepository(
                AzureTableStorage<OffsetRecord>.Create(() => _settings.LucyTelegramBotJob.Db.DataConnString, "TgUpdatesOffset", _log))).As<IOffsetRepository>();
            builder.RegisterInstance(new TgEmployeeRepository(
                AzureTableStorage<TgEmployee>.Create(() => _settings.LucyTelegramBotJob.Db.DataConnString, "TgEmployees", _log))).As<ITgEmployeeRepository>();
            builder.RegisterInstance(new HandledMessagesRepository(
                AzureTableStorage<HandledMessageRecord>.Create(() => _settings.LucyTelegramBotJob.Db.DataConnString, "TgHandledMessages", _log))).As<IHandledMessagesRepository>();
            builder.RegisterInstance(new TgUserStateRepository(
                AzureTableStorage<TgUserStateEntity>.Create(() => _settings.LucyTelegramBotJob.Db.DataConnString, "TgUserStates", _log))).As<ITgUserStateRepository>();
            builder.RegisterInstance(new AzureBlobStorage(_settings.LucyTelegramBotJob.Db.DataConnString)).As<IBlobStorage>();
            
            builder.RegisterInstance(new AzureQueueExt(_settings.LucyTelegramBotJob.Db.TriggerQueueConnectionString, "lucy-tg-updates")).As<IQueueExt>();
            
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
