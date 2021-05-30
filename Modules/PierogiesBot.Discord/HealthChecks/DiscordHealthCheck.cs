using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PierogiesBot.Discord.HealthChecks
{
    public class DiscordHealthCheck : IHealthCheck
    {
        private readonly DiscordSocketClient _client;

        public DiscordHealthCheck(DiscordSocketClient client)
        {
            _client = client;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new()) =>
            _client.ConnectionState switch
            {
                ConnectionState.Disconnected => Task.FromResult(
                    HealthCheckResult.Unhealthy("Discord client is disconnected")),
                ConnectionState.Connecting => Task.FromResult(
                    HealthCheckResult.Degraded("Discord client is connecting")),
                ConnectionState.Connected => Task.FromResult(HealthCheckResult.Healthy("Discord client is connected")),
                ConnectionState.Disconnecting => Task.FromResult(
                    HealthCheckResult.Degraded("Discord client is disconnecting")),
                _ => throw new ArgumentOutOfRangeException(nameof(_client.ConnectionState))
            };
    }
}