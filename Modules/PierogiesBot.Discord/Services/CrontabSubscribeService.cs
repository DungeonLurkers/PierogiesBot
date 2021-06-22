using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using PierogiesBot.Data.Enums;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Jobs;
using Quartz;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Services
{
    public class CrontabSubscribeService : IChannelSubscribeService
    {
        private readonly IRepository<BotCrontabRule> _ruleRepository;
        private readonly IScheduler _scheduler;
        private readonly IRepository<GuildSettings> _settingsRepository;
        private readonly IRepository<BotMessageSubscription> _subscriptionRepository;
        private readonly ILogger<CrontabSubscribeService> _logger;

        public CrontabSubscribeService(
            IScheduler scheduler,
            IRepository<BotCrontabRule> ruleRepository,
            IRepository<GuildSettings> settingsRepository,
            IRepository<BotMessageSubscription> subscriptionRepository,
            ILogger<CrontabSubscribeService> logger)
        {
            _scheduler = scheduler;
            _ruleRepository = ruleRepository;
            _settingsRepository = settingsRepository;
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task LoadSubscriptionsAsync()
        {
            _logger.LogInformation("Loading Crontab subscriptions");
            var rules = await _ruleRepository.GetAll();
            var guilds = await _settingsRepository.GetAll();

            var botCrontabRules = rules.ToList();
            foreach (var (_, guildId, guildTimeZoneId, _) in guilds)
            foreach (var rule in botCrontabRules)
            {
                var tzInfo = TZConvert.GetTimeZoneInfo(guildTimeZoneId);

                _logger.LogInformation("Creating job for guild {{{0}}} in TimeZone '{1}', Crontab = {{{2}}}", 
                    guildId,
                    tzInfo.DisplayName,
                    rule.Crontab);

                var guildIdS = guildId.ToString();
                var job = JobBuilder.Create<SendCrontabMessageToChannelsJob>()
                    .WithIdentity(guildIdS, rule.Id)
                    .SetJobData(new JobDataMap
                    {
                        { "Rule", rule },
                        { "GuildId", guildId },
                    }).Build();

                var trigger = TriggerBuilder
                    .Create()
                    .WithIdentity(guildIdS, rule.Id)
                    .ForJob(job)
                    .WithCronSchedule(rule.Crontab, builder => builder.InTimeZone(tzInfo))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);

                var triggerNextFire = trigger.GetNextFireTimeUtc();
                _logger.LogDebug($"Trigger '{rule.Crontab}' next fire time is {triggerNextFire:F}");
            }
        }

        /// <inheritdoc/>
        public async Task SubscribeAsync(SocketGuildChannel channel)
        {
            var guild = channel.Guild!;
            var existing = await _subscriptionRepository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id)
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Crontab);
            var existingList = existing?.ToList() ?? new();

            if (!existingList.Any())
            {
                _logger.LogInformation(
                    $"Subscription not found in database. Inserting new document for channel {channel} in guild {guild}");
                await _subscriptionRepository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id,
                    SubscriptionType.Crontab));
            }
        }

        /// <inheritdoc/>
        public async Task UnsubscribeAsync(SocketGuildChannel channel)
        {
            var guild = channel.Guild!;
            _logger.LogInformation($"Unsubscribing channel {channel} in guild {guild}");
            var existingEnumerable = await _subscriptionRepository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id)
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Crontab);
            var existing = existingEnumerable?.FirstOrDefault();

            if (existing is not null) await _subscriptionRepository.DeleteAsync(existing.Id);
        }
    }
}