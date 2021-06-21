using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Services
{
    public class DiscordClientHostedService : IHostedService
    {
        private readonly ILogger<DiscordClientHostedService> _logger;
        private readonly DiscordSocketClient _client;

        public DiscordClientHostedService(ILoggerFactory loggerfactory, DiscordSocketClient client)
        {
            _client = client;

            _logger = loggerfactory.CreateLogger<DiscordClientHostedService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Discord services");
            await _client.StopAsync();
            await _client.LogoutAsync();
            _logger.LogInformation("Discord services stopped");
        }
    }
}