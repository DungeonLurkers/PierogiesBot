using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Discord.Modules
{
    [Group("settings")]
    public class GuildSettingsCommandModule : ModuleBase
    {
        private readonly ILogger<GuildSettingsCommandModule> _logger;
        private readonly IRepository<GuildSettings> _repository;

        public GuildSettingsCommandModule(ILogger<GuildSettingsCommandModule> logger, IRepository<GuildSettings> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Command("set_timezone")]
        public async Task SetTimeZone(TimeZoneInfo tzInfo)
        {
            var guildId = Context.Guild.Id;
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);

            if (settings == null)
            {
                await _repository.InsertAsync(new GuildSettings(guildId, tzInfo.ToSerializedString()));
            }
            else
            {
                await _repository.UpdateAsync(settings with {GuildTimeZone = tzInfo.ToSerializedString()});
            }

            await ReplyAsync($"Server timezone set to {tzInfo}");
        }
        
        [Command("get_timezone")]
        public async Task GetTimeZone()
        {
            var guildId = Context.Guild.Id;
            var settings = await _repository.GetByProperty(s => s.GuildId, guildId);

            if (settings == null) return;

            await ReplyAsync($"Server timezone is {TimeZoneInfo.FromSerializedString(settings.GuildTimeZone)}");
        }
    }
}