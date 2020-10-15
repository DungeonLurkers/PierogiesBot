using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Host;
using PierogiesBot.Modules.Core.Extensions;

namespace PierogiesBot.Runners.Console
{
    public static class Program
    {
        private static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .UseConsoleLifetime()
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