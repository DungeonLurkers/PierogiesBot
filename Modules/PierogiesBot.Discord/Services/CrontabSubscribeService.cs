﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Jobs;
using Quartz;
using Quartz.Impl;
using IScheduler = Quartz.IScheduler;

namespace PierogiesBot.Discord.Services
{
    public class CrontabSubscribeService
    {
        private readonly IScheduler _scheduler;
        private readonly IRepository<BotCrontabRule> _ruleRepository;
        private readonly IRepository<GuildSettings> _settingsRepository;
        private readonly IRepository<BotMessageSubscription> _subscriptionRepository;
        public CrontabSubscribeService(IScheduler scheduler, IRepository<BotCrontabRule> ruleRepository, IRepository<GuildSettings> settingsRepository, IRepository<BotMessageSubscription> subscriptionRepository)
        {
            _scheduler = scheduler;
            _ruleRepository = ruleRepository;
            _settingsRepository = settingsRepository;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task LoadSubscriptions()
        {
            var rules = await _ruleRepository.GetAll();
            var guilds = await _settingsRepository.GetAll();

            foreach (var (id, guildId, guildTimeZone) in guilds)
                foreach (var rule in rules)
                {
                    var tzInfo = TimeZoneInfo.FromSerializedString(guildTimeZone);
                    var job = JobBuilder.Create<SendCrontabMessageToChannelsJob>()
                        .WithIdentity(id, rule.Crontab)
                        .SetJobData(new JobDataMap
                        {
                            {"Rule", rule},
                            {"GuildId", guildId}
                        }).Build();
                    
                    var trigger = TriggerBuilder
                        .Create()
                        .WithIdentity(id, rule.Crontab)
                        .ForJob(job)
                        .WithCronSchedule(rule.Crontab, builder => builder.InTimeZone(tzInfo))
                        .Build();
                    
                    await _scheduler.ScheduleJob(job, trigger);
                }
        }
        public async Task Subscribe(IGuild guild, IMessageChannel channel)
        {
            var existing = await _subscriptionRepository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Crontab);
            var existingList = existing?.ToList() ?? new ();

            if (!existingList.Any())
                await _subscriptionRepository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id, SubscriptionType.Crontab));
        }
        public async Task Unsubscribe(IGuild guild, IMessageChannel channel)
        {
            var existingEnumerable = await _subscriptionRepository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Crontab);
            var existing = existingEnumerable?.FirstOrDefault();

            if (existing is not null) await _subscriptionRepository.DeleteAsync(existing.Id);
        }
    }
}