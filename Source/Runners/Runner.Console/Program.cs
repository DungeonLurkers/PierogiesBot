using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Module.Core.Extensions;

namespace Runner.Console
{
    public static class Program
    {
        private static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddEnvironmentVariables("PIEROGIESBOT");
                builder.AddJsonFile("appsettings.json");
            })
            .ConfigureServices(Startup.ConfigureServices)
            .ConfigureLogging(Startup.ConfigureLogging);

        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            try
            {
                await host.RunAsync();

                ObservableExtensions.Logger = Startup.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(nameof(ObservableExtensions));
            }
            catch (TaskCanceledException)
            {
                System.Console.WriteLine("Exit");
            }
        }
    }
}