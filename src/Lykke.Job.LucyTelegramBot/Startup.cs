using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Job.LucyTelegramBot.Core;
using Lykke.Job.LucyTelegramBot.Models;
using Lykke.Job.LucyTelegramBot.Modules;
using Lykke.JobTriggers.Extenstions;
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
            var appSettings = Environment.IsDevelopment()
                ? Configuration.Get<AppSettings>()
                : HttpSettingsLoader.Load<AppSettings>(Configuration.GetValue<string>("SettingsUrl"));

            var log = CreateLogWithSlack(services, appSettings);

            builder.RegisterModule(new JobModule(appSettings, log));

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

            appLifetime.ApplicationStopped.Register(() =>
            {
                ApplicationContainer.Dispose();
            });
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, AppSettings settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var log = services.UseLogToAzureStorage(settings.LucyTelegramBotJob.Db.LogsConnString, slackService,
                "LucyTelegramBotLog", new LogToConsole());

            return log;
        }
    }
}