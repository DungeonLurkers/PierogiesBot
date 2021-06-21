using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orleans;
using PierogiesBot.GrainsInterfaces;
using PierogiesBot.GrainsInterfaces.Discord;

namespace PierogiesBot.Grains.Discord
{
    public class DiscordGuildGrain : Grain, IDiscordGuildGrain
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<DiscordGuildGrain> _logger;

        public DiscordGuildGrain(DiscordSocketClient client, ILogger<DiscordGuildGrain> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public Task<DiscordGuild> GetGuildByIdAsync(ulong id)
        {
            _logger.LogTrace("{0} - {1}", IdentityString, nameof(GetGuildByIdAsync));
            return Task.FromResult(Map(_client.Guilds.Single(x => x.Id == id)));
        }

        public Task<List<DiscordGuild>> GetGuildsAsync()
        {
            _logger.LogTrace("{0} - {1}", IdentityString, nameof(GetGuildsAsync));
            return Task.FromResult(_client.Guilds.Select(Map).ToList());
        }

        private static DiscordGuild Map(IGuild guild) => new() {Id = guild.Id, Name = guild.Name};
    }
}