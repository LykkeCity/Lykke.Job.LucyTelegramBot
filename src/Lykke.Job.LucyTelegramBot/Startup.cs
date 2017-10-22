using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Models;
using Lykke.Job.LucyTelegramBot.Modules;
using Lykke.JobTriggers.Extenstions;
using Lykke.JobTriggers.Triggers;
using Lykke.Logs;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.LucyTelegramBot
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; set; }
        public IConfigurationRoot Configuration { get; }

        private TriggerHost _triggerHost;
        private Task _triggerHostTask;

        private ILog _log;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "LucyTelegramBot API");
            });

            var builder = new ContainerBuilder();
            var settingsManager = Configuration.LoadSettings<AppSettings>("SettingsUrl");
            var appSettings = settingsManager.CurrentValue;
            _log = CreateLogWithSlack(services, settingsManager, appSettings);

            builder.RegisterModule(new JobModule(settingsManager, _log));

            if (string.IsNullOrWhiteSpace(appSettings.LucyTelegramBotJob.Db.TriggerQueueConnectionString))
            {
                builder.AddTriggers();
            }
            else
            {
                builder.AddTriggers(pool =>
                {
                    pool.AddDefaultConnection(appSettings.LucyTelegramBotJob.Db.TriggerQueueConnectionString);
                });
            }

            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLykkeMiddleware("LucyTelegramBot", ex => new ErrorResponse { ErrorMessage = "Technical problem" });

            app.UseMvc();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUi();

            appLifetime.ApplicationStarted.Register(Start);
            appLifetime.ApplicationStopping.Register(StopApplication);
            appLifetime.ApplicationStopped.Register(CleanUp);
        }

        private void Start()
        {
            _triggerHost = new TriggerHost(new AutofacServiceProvider(ApplicationContainer));
            _triggerHostTask = _triggerHost.Start();

            _log.WriteMonitorAsync("", "", "Started").Wait();
        }

        private void StopApplication()
        {
            _triggerHost?.Cancel();
            _triggerHostTask?.Wait();
        }

        private void CleanUp()
        {
            ApplicationContainer.Dispose();
        }

        private static ILog CreateLogWithSlack(
            IServiceCollection services,
            IReloadingManager<AppSettings> settingsManager,
            AppSettings settings)
        {
            var consoleLogger = new LogToConsole();
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(
                new AzureQueueIntegration.AzureQueueSettings
                {
                    ConnectionString = settings.SlackNotifications.AzureQueue.ConnectionString,
                    QueueName = settings.SlackNotifications.AzureQueue.QueueName
                },
                consoleLogger);

            var log = services.UseLogToAzureStorage(
                settingsManager.ConnectionString(i => i.LucyTelegramBotJob.Db.LogsConnString),
                slackService,
                "LucyTelegramBotLog",
                consoleLogger);

            var aggregateLogger = new AggregateLogger();
            aggregateLogger.AddLog(consoleLogger);
            aggregateLogger.AddLog(log);

            return aggregateLogger;
        }
    }
}