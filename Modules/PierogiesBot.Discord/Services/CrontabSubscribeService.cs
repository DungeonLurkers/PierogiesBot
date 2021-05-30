using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
        private readonly IRepository<BotMessageSubscription> _subscriptionRepository;
        private readonly DiscordSocketClient _client;
        private readonly IMessageHandlerChain _messageHandlerChain;

        private ISubject<SocketUserMessage> _userMessagesSubject;

        private Dictionary<(ulong, ulong), object> _subscriptions;

        public CrontabSubscribeService(IScheduler scheduler, IRepository<BotCrontabRule> ruleRepository, IRepository<BotMessageSubscription> subscriptionRepository, DiscordSocketClient client, IMessageHandlerChain messageHandlerChain)
        {
            _scheduler = scheduler;
            _ruleRepository = ruleRepository;
            _subscriptionRepository = subscriptionRepository;
            _client = client;
            _messageHandlerChain = messageHandlerChain;

            _subscriptions = new Dictionary<(ulong, ulong), object>();
            
        }
        
        public async Task LoadSubscriptions()
        {
            var subscriptions = await _subscriptionRepository.GetAll();
            var rules = await _ruleRepository.GetAll();

            foreach (var sub in subscriptions)
            {
                var (_, guildId, channelId) = sub;
                Subscribe(guildId, channelId);
            }

            foreach (var rule in rules)
            {
                
                var job = JobBuilder.Create<SendCrontabMessageToChannelsJob>()
                    .WithIdentity(nameof(CrontabSubscribeService))
                    .SetJobData(new JobDataMap
                    {
                        {"Rule", rule}
                    }).Build();
                
                var trigger = TriggerBuilder
                    .Create()
                    .WithIdentity(nameof(CrontabSubscribeService))
                    .ForJob(job)
                    .WithCronSchedule(rule.Crontab)
                    .Build();
                
                await _scheduler.ScheduleJob(job, trigger);
            }
        }
        
        public async Task Subscribe(IGuild guild, IMessageChannel channel)
        {
            var existing = await _subscriptionRepository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id));
            var existingList = existing?.ToList() ?? new ();

            if (!existingList.Any())
                await _subscriptionRepository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id));

            Subscribe(guild.Id, channel.Id);
        }
        
        private void Subscribe(ulong guildId, ulong channelId)
        {
            if (_subscriptions.ContainsKey((guildId, channelId))) return;
            // var disposable = _userMessagesSubject
            //     .ObserveOn(TaskPoolScheduler.Default)
            //     .Where(m => !m.Author.IsBot)
            //     .Where(m => m.Channel.Id == channelId)
            //     .Select(m => new SocketCommandContext(_client, m))
            //     .Do(async ctx => await _messageHandlerChain.HandleAsync(ctx))
            //     .Subscribe();
                
            _subscriptions[(guildId, channelId)] = new {};
        }
        
    }
}