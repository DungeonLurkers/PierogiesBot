using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Modules.Discord.Services;
using PierogiesBot.Modules.Discord.Services.Definitions;
using PierogiesBot.Modules.Discord.Services.Implementations;

namespace PierogiesBot.Runners.Console
{
    public static class DependencyInjectionConfig
    {
        public static void AddBotServices(this IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<IDiscordBotService, DiscordBotServiceImpl>();
            
            services.AddHostedService<PierogiesBotService>();
        }
    }
}