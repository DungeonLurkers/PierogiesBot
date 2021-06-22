using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PierogiesBot.Data.Models;
using TimeZoneConverter;

namespace PierogiesBot.Data.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IRepository<GuildSettings> _repository;
        private readonly DiscordSocketClient _discordSocketClient;

        public SettingsService(IRepository<GuildSettings> repository, DiscordSocketClient discordSocketClient)
        {
            _repository = repository;
            _discordSocketClient = discordSocketClient;
        }

        public async Task SetGuildTimeZone(ulong guildId, TimeZoneInfo tzInfo)
        {
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);

            if (settings == null)
                await _repository.InsertAsync(new GuildSettings(guildId, tzInfo.Id, 0));
            else
                await _repository.UpdateAsync(settings with {GuildTimeZone = tzInfo.Id});
        }

        public async Task<TimeZoneInfo?> GetGuildTimeZone(ulong guildId)
        {
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);
            if (settings is null) return null;
            var tzInfo = TZConvert.GetTimeZoneInfo(settings.GuildTimeZone);
            return tzInfo;
        }

        public async Task SetMuteRole(ulong guildId, IRole role)
        {
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);

            if (settings == null)
                await _repository.InsertAsync(new GuildSettings(guildId, TimeZoneInfo.Local.Id, role.Id));
            else
                await _repository.UpdateAsync(settings with {GuildMuteRoleId = role.Id});
        }

        public async Task<IRole?> GetMuteRole(ulong guildId)
        {
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);

            var muteRoleId = settings?.GuildMuteRoleId;

            switch (muteRoleId)
            {
                case 0ul:
                    return null;
                case {} roleId:
                {
                    var guild = _discordSocketClient.GetGuild(guildId);

                    return guild.GetRole(roleId);
                }
            }

            return null;
        }
    }
}