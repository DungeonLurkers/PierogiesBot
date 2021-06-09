using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orleans;
using PierogiesBot.GrainsInterfaces;

namespace PierogiesBot.Grains
{
    public class DiscordGuildGrain : Grain, IDiscordGuildGrain
    {
        private readonly DiscordSocketClient _client;
        public DiscordGuildGrain(DiscordSocketClient client)
        {
            _client = client;
        }
        
        public Task<DiscordGuild> GetGuildByIdAsync(ulong id) =>
            Task.Run(() =>
            {
                var guild = _client.Guilds.Single(x => x.Id == id);
                return Map(guild);
            });

        public Task<List<DiscordGuild>> GetGuildsAsync() => Task.FromResult(_client.Guilds.Select(Map).ToList());

        private static DiscordGuild Map(IGuild guild) => new() {Id = guild.Id, Name = guild.Name};
    }
}