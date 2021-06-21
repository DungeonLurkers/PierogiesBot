using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace PierogiesBot.GrainsInterfaces.Discord
{
    public interface IDiscordGuildGrain : IGrainWithStringKey
    {
        Task<DiscordGuild> GetGuildByIdAsync(ulong id);
        Task<List<DiscordGuild>> GetGuildsAsync();
    }
}