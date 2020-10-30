using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Module.Discord.Extensions;
using NLog.Extensions.Logging;

namespace Runner.Console
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection servicesCollection)
        {
            servicesCollection.AddAutofac();

            servicesCollection.AddDiscordClient();
            servicesCollection.AddBotServices();

            ServiceProvider = servicesCollection.BuildServiceProvider();
        }

        public static void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog();
        }
    }
}