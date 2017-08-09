using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Core.Services;
using Lykke.Job.LucyTelegramBot.Core.Telegram;
using Lykke.Job.LucyTelegramBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using System.Runtime;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Queue;
using Lykke.Job.LucyTelegramBot.AzureRepositories.Telegram;
using Lykke.Job.LucyTelegramBot.Services.Commands;

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
                new AzureTableStorage<OffsetRecord>(_settings.LucyTelegramBotJob.Db.DataConnString, "TgUpdatesOffset", _log))).As<IOffsetRepository>();
            builder.RegisterInstance(new TgEmployeeRepository(
                new AzureTableStorage<TgEmployee>(_settings.LucyTelegramBotJob.Db.DataConnString, "TgEmployees", _log))).As<ITgEmployeeRepository>();
            builder.RegisterInstance(new AzureBlobStorage(_settings.LucyTelegramBotJob.Db.DataConnString)).As<IBlobStorage>();

            builder.RegisterInstance(new AzureQueueExt(_settings.LucyTelegramBotJob.TriggerQueueConnectionString, "lucy-tg-updates")).As<IQueueExt>();
            
            builder.RegisterType<MessageHandler>()
                .As<IMessageHandler>();
            builder.RegisterType<MessagePoller>()
                .As<IMessagePoller>();

            builder.RegisterType<BotCommandFactory>().As<IBotCommandFactory>();
            builder.RegisterType<StartCommand>().As<IBotCommand>();
            builder.RegisterType<PostBioCommand>().As<IBotCommand>();
            builder.RegisterType<NewbieGuideCommand>().As<IBotCommand>();
            builder.RegisterType<InfoCommand>().As<IBotCommand>();
            builder.RegisterType<GetPaidCommand>().As<IBotCommand>();
            builder.RegisterType<ContractorsCommand>().As<IBotCommand>();

            // NOTE: You can implement your own poison queue notifier. See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            builder.RegisterType<TelegramBotClient>()
                .As<ITelegramBotClient>()
                .WithParameter("token", _settings.LucyTelegramBotJob.Token)
                .SingleInstance();

            builder.Populate(_services);
        }
    }
}