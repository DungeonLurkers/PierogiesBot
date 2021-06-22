using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.Commons.Dtos.Mute;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Jobs;
using PierogiesBot.Discord.Services;
using PierogiesBot.GrainsInterfaces.Discord;
using Quartz;

namespace PierogiesBot.Grains.Discord
{
    [StatelessWorker(1)]
    [Reentrant]
    public class DiscordMuteGrainService : GrainService, IDiscordMuteGrainService
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerFactory _loggerFactory;
        private ILogger<DiscordMuteGrainService> _logger;
        private DiscordSocketClient _client;
        private readonly IDiscordMuteUserService _muteUserService;
        private readonly ISettingsService _settingsService;
        private readonly IScheduler _scheduler;

        public DiscordMuteGrainService(IServiceProvider services, IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory, DiscordSocketClient client, IDiscordMuteUserService muteUserService, ISettingsService settingsService, IScheduler scheduler)
            : base(id, silo, loggerFactory)
        {
            _services = services;
            _loggerFactory = loggerFactory;
            _client = client;
            _muteUserService = muteUserService;
            _settingsService = settingsService;
            _scheduler = scheduler;
        }
        
        public override async Task Init(IServiceProvider serviceProvider)
        {
            _logger = _loggerFactory.CreateLogger<DiscordMuteGrainService>();
            
            await base.Init(serviceProvider);
        }

        public override async Task Start()
        {
            _logger.LogDebug("Loading discord unmute jobs");

            var mutes = await _muteUserService.GetAllMutes();
            foreach (var muteGroup in mutes.GroupBy(x => x.DiscordGuildId))
            {
                var guildId = muteGroup.Key;
                var guild = _client.GetGuild(guildId);

                _logger.LogDebug($"Checking mutes for guild {guild}");

                foreach (var mute in muteGroup)
                {
                    var user = guild.GetUser(mute.DiscordUserId);
                    var guildTimeZone = await _settingsService.GetGuildTimeZone(guild.Id);
                    if (guildTimeZone is null) continue;

                    _logger.LogTrace($"{user} has mute until {mute.Until:F} because of \"{mute.Reason}\"");
                    var now = DateTimeOffset.UtcNow;
                    var guildNow = TimeZoneInfo.ConvertTime(now, guildTimeZone);

                    // Unmute if mute is expired or close to expire
                    if (mute.Until > guildNow.Subtract(TimeSpan.FromSeconds(25)))
                    {
                        await CreateUnmuteJob(mute);
                        continue;
                    }
                    _logger.LogDebug($"{user} mute has expired ({guildNow:F} is greater than {mute.Until:F})");
                    await _muteUserService.UnmuteUser(user);
                }
            }
            
            _logger.LogDebug("Discord unmute jobs loaded");
        }

        private async Task CreateUnmuteJob(GetMuteDto mute)
        {
            var guildId = mute.DiscordGuildId.ToString();
            var userId = mute.DiscordUserId.ToString();
            
            var job = JobBuilder.Create<UnmuteUserJob>()
                .WithIdentity(userId, guildId)
                .SetJobData(new JobDataMap
                {
                    {"Mute", mute}
                }).Build();


            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(userId, guildId)
                .ForJob(job)
                .StartAt(mute.Until)
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
            
            _logger.LogDebug($"Created unmute job with trigger at {trigger.GetNextFireTimeUtc()} ");
        }
    }
}