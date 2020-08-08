using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Host.Services;
using PierogiesBot.Host.Services.Definitions;
using PierogiesBot.Host.Services.Implementations;

namespace PierogiesBot.Host
{
    public static class DependencyInjectionConfig
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<DiscordSocketClient>();
            services.AddSingleton<IDiscordBotService, DiscordBotServiceImpl>();
            
            services.AddHostedService<PierogiesBotService>();
        }
    }
}