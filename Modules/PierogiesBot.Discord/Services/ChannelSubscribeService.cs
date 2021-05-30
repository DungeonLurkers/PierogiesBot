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

namespace PierogiesBot.Discord.Services
{
    public class ChannelSubscribeService
    {
        private readonly IRepository<BotMessageSubscription> _repository;
        private readonly DiscordSocketClient _client;
        private readonly IMessageHandlerChain _messageHandlerChain;
        private readonly ISubject<SocketUserMessage> _userMessagesSubject;
        private readonly Dictionary<(ulong, ulong), IDisposable> _subscriptions;

        public ChannelSubscribeService(IRepository<BotMessageSubscription> repository, DiscordSocketClient client, IMessageHandlerChain messageHandlerChain)
        {
            _repository = repository;
            _client = client;
            _messageHandlerChain = messageHandlerChain;

            _subscriptions = new Dictionary<(ulong, ulong), IDisposable>();
            _userMessagesSubject = new Subject<SocketUserMessage>();

            _client.MessageReceived += async message => await Task.Run(() =>
            {
                if (message is SocketUserMessage msg) _userMessagesSubject.OnNext(msg);
            });
        }
        
        public async Task LoadSubscriptions()
        {
            var subscriptions = await _repository.GetAll();

            foreach (var sub in subscriptions)
            {
                var (_, guildId, channelId) = sub;
                Subscribe(guildId, channelId);
            }
        }
        public async Task Subscribe(IGuild guild, IMessageChannel channel)
        {
            var existing = await _repository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id));
            var existingList = existing?.ToList() ?? new ();

            if (!existingList.Any())
                await _repository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id));

            Subscribe(guild.Id, channel.Id);
        }
        
        public async Task Unsubscribe(IGuild guild, IMessageChannel channel)
        {
            var existingEnumerable = await _repository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id));
            var existing = existingEnumerable.FirstOrDefault();

            if (existing is not null)
            {
                await _repository.DeleteAsync(existing.Id);
                if (_subscriptions[(guild.Id, channel.Id)] is {} sub) sub.Dispose();
            }

        }

        private void Subscribe(ulong guildId, ulong channelId)
        {
            if (_subscriptions.ContainsKey((guildId, channelId))) return;
            var disposable = _userMessagesSubject
                .ObserveOn(TaskPoolScheduler.Default)
                .Where(m => !m.Author.IsBot)
                .Where(m => m.Channel.Id == channelId)
                .Select(m => new SocketCommandContext(_client, m))
                .Do(async ctx => await _messageHandlerChain.HandleAsync(ctx))
                .Subscribe();
                
            _subscriptions[(guildId, channelId)] = disposable;
        }
    }
}