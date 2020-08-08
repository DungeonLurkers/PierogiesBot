using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Host
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection servicesCollection)
        {
            servicesCollection.AddAutofac();
            
            servicesCollection.AddServices();

            ServiceProvider = servicesCollection.BuildServiceProvider();
        }

        public static void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();
        }
    }
}