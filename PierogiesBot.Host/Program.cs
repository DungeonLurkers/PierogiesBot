using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PierogiesBot.Host
{
    public static class Program
    {
        public static IHostBuilder HostBuilder { get; } = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseConsoleLifetime()
            .ConfigureServices(Startup.ConfigureServices)
            .ConfigureLogging(Startup.ConfigureLogging);
        public static async Task Main(string[] args)
        {
            using var host = HostBuilder.Build();
            try
            {
                await host.RunAsync();
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("Exit");
            }
        }
    }
}