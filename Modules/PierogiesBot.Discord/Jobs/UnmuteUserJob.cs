using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Data.Models;
using PierogiesBot.Discord.Services;
using Quartz;

namespace PierogiesBot.Discord.Jobs
{
    public class UnmuteUserJob : IJob
    {
        private readonly ILogger<UnmuteUserJob> _logger;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordMuteUserService _muteUserService;

        public UnmuteUserJob(ILogger<UnmuteUserJob> logger, DiscordSocketClient client, IDiscordMuteUserService muteUserService)
        {
            _logger = logger;
            _client = client;
            _muteUserService = muteUserService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (context.MergedJobDataMap["Mute"] is GetMuteDto mute)
            {
                var guild = _client.GetGuild(mute.DiscordGuildId);

                var user = guild.GetUser(mute.DiscordUserId);

                _logger.LogTrace($"Unmuting user {user} in guild {guild}");
                await _muteUserService.UnmuteUser(user);
            }
        }
    }
}