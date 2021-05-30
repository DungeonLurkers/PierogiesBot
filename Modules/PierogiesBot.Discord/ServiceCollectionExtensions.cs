
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Discord.JobFactory;
using PierogiesBot.Discord.Jobs;
using PierogiesBot.Discord.MessageHandlers;
using PierogiesBot.Discord.Services;
using PierogiesBot.Discord.Settings;
using Quartz;
using Quartz.Simpl;

namespace PierogiesBot.Discord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddTransient<CommandService>();
            services.AddTransient<ChannelSubscribeService>();
            services.AddTransient<CrontabSubscribeService>();
            services.AddHostedService<DiscordClientHostedService>();

            services.AddTransient<IMessageHandlerChain, MessageHandlerChain>();
            services.AddTransient<IUserSocketMessageHandler, BotResponseMessageHandler>();
            services.AddTransient<IUserSocketMessageHandler, BotReactionsMessageHandler>();

            services.AddTransient<SendCrontabMessageToChannelsJob>();

            services.AddQuartz(configurator =>
            {
                configurator.SchedulerId = "CoreScheduler";
                
                configurator.UseMicrosoftDependencyInjectionJobFactory();
                configurator.UseSimpleTypeLoader();
                configurator.UseInMemoryStore();
                configurator.UseTimeZoneConverter();

                // configurator.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();

            });
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = false);

            services.AddSingleton<IScheduler>(serviceProvider =>
            {
                var config = SchedulerBuilder.Create();
                
                config.UseTimeZoneConverter();
                var scheduler = config.Build().GetScheduler().GetAwaiter().GetResult();

                scheduler.JobFactory = new DependencyInjectionJobFactory(serviceProvider);
                
                return scheduler;
            });
            
            return services;
        }
    }
}