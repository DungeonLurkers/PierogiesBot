using System;
using System.Threading.Tasks;
using Discord;

namespace PierogiesBot.Data.Services
{
    public interface ISettingsService
    {
        Task SetGuildTimeZone(ulong guildId, TimeZoneInfo tzInfo);

        Task<TimeZoneInfo?> GetGuildTimeZone(ulong guildId);

        Task SetMuteRole(ulong guildId, IRole role);

        Task<IRole?> GetMuteRole(ulong guildId);
    }
}