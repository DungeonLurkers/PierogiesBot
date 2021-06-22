using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Services;

namespace PierogiesBot.Discord.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class MuteUserCommandModule : LoggingModuleBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IDiscordMuteUserService _discordMuteUserService;

        public MuteUserCommandModule(ILogger<MuteUserCommandModule> logger, ISettingsService settingsService, IDiscordMuteUserService discordMuteUserService)
            : base(logger)
        {
            _settingsService = settingsService;
            _discordMuteUserService = discordMuteUserService;
        }

        [Command("mute")]
        public async Task MuteUser(SocketGuildUser user, string muteSpanString, string reason)
        {
            try
            {
                var guildId = Context.Guild.Id;
                var guildTimeZone = await _settingsService.GetGuildTimeZone(guildId) ?? TimeZoneInfo.Local;

                var nowUtc = DateTimeOffset.UtcNow;
                var now = TimeZoneInfo.ConvertTime(nowUtc, guildTimeZone);
                var muteSpan = XmlConvert.ToTimeSpan(muteSpanString.ToUpper(CultureInfo.InvariantCulture));
                var until = now + muteSpan;
                var dto = new CreateMuteDto(user.Id, guildId, until, reason, user.Roles.Select(x => x.Id).ToList());

                await _discordMuteUserService.MuteUser(user, dto);
                await ReplyAsync($"{user} is muted until {until:F}");
            }
            catch (FormatException e)
            {
                var message = "Wrong format of mute time span! Time span must be complaint with ISO 8601";
                await ReplyAsync(message);
                LogError(e, message);
            }
        }

        [Command("unmute")]
        public async Task UnmuteUser(SocketGuildUser user)
        {
            await _discordMuteUserService.UnmuteUser(user);

            await ReplyAsync($"User {user} is no longer muted");
        }
    }
}