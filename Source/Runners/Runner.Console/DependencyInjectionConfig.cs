using Microsoft.Extensions.DependencyInjection;
using Module.Discord.Services;
using Module.Discord.Services.Definitions;
using Module.Discord.Services.Implementations;
using Persistence.Extensions;
using Runner.Console.Services;

namespace Runner.Console
{
    public static class DependencyInjectionConfig
    {
        public static void AddBotServices(this IServiceCollection services)
        {
            services.AddPersistence();

            services.AddSingleton<IDiscordBotService, DiscordBotServiceImpl>();

            services.AddHostedService<PopulateDataSourcesHostedService>();
            services.AddHostedService<PierogiesBotService>();

        }
    }
}