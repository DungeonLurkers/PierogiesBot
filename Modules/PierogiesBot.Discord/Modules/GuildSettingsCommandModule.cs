using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orleans;
using PierogiesBot.Data.Services;
using PierogiesBot.GrainsInterfaces.Data;

namespace PierogiesBot.Discord.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("settings")]
    public class GuildSettingsCommandModule : LoggingModuleBase
    {
        private readonly ISettingsService _settingsService;

        public GuildSettingsCommandModule(
            ILogger<GuildSettingsCommandModule> logger,
            ISettingsService settingsService)
            : base(logger)
        {
            _settingsService = settingsService;
        }

        [Command("set_timezone")]
        public async Task SetTimeZone(TimeZoneInfo tzInfo)
        {
            LogTrace($"Set TimeZone {tzInfo.DisplayName}");
            var guildId = Context.Guild.Id;

            await _settingsService.SetGuildTimeZone(guildId, tzInfo);

            await ReplyAsync($"Server timezone set to {tzInfo}");
        }

        [Command("get_timezone")]
        public async Task GetTimeZone()
        {
            LogTrace("Get TimeZone");
            var guildId = Context.Guild.Id;
            var tzInfo = await _settingsService.GetGuildTimeZone(guildId);

            if (tzInfo == null) return;

            await ReplyAsync($"Server timezone is {tzInfo}");
        }

        [Command("set_muterole")]
        public async Task SetMuteRole(SocketRole role)
        {
            LogTrace($"Set mute role to {role}");
            var guildId = Context.Guild.Id;

            await _settingsService.SetMuteRole(guildId, role);

            await ReplyAsync($"Server mute role set to {role}");
        }

        [Command("get_muterole")]
        public async Task GetMuteRole()
        {
            LogTrace("Get guild mute role");
            var guild = Context.Guild;
            var guildId = guild.Id;

            var settingsGuildMuteRole = await _settingsService.GetMuteRole(guildId);
            if (settingsGuildMuteRole is null)
            {
                await ReplyAsync("There is no mute role set");
                return;
            }

            await ReplyAsync($"Server mute role is {settingsGuildMuteRole}");
        }
    }
}