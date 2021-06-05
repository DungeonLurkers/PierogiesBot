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
using Microsoft.Extensions.Logging;
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
        private ILogger<ChannelSubscribeService> _logger;

        public ChannelSubscribeService(IRepository<BotMessageSubscription> repository, DiscordSocketClient client, IMessageHandlerChain messageHandlerChain, ILogger<ChannelSubscribeService> logger)
        {
            _repository = repository;
            _client = client;
            _messageHandlerChain = messageHandlerChain;
            _logger = logger;

            _subscriptions = new Dictionary<(ulong, ulong), IDisposable>();
            _userMessagesSubject = new Subject<SocketUserMessage>();

            _client.MessageReceived += async message => await Task.Run(() =>
            {
                if (message is SocketUserMessage msg) _userMessagesSubject.OnNext(msg);
            });
        }
        
        public async Task LoadSubscriptions()
        {
            _logger.LogInformation("Loading subscriptions from DB");
            var subscriptions = await _repository.GetAll();

            foreach (var sub in subscriptions)
            {
                var (_, guildId, channelId, _) = sub;

                var guild = _client.Guilds.SingleOrDefault(x => x.Id == guildId);
                if (guild is null)
                {
                    _logger.LogWarning("Guild with Id {0} not found!", guildId);
                    continue;
                }

                _logger.LogDebug("Found guild [{0}]", guild.Name);
                var channel = guild.Channels.SingleOrDefault(x => x.Id == channelId);

                if (channel is null)
                {
                    _logger.LogWarning("Guild with Id {0} not found!", guildId);
                    continue;
                }
                
                _logger.LogDebug("Found channel [{0}] in guild [{1}]", channel.Name, guild.Name);
                await Subscribe(guild, channel);
            }
        }

        private async Task Subscribe(SocketGuild guild, SocketGuildChannel channel) => await Subscribe((IGuild) guild, (IMessageChannel) channel);

        public async Task Subscribe(IGuild guild, IMessageChannel channel)
        {
            _logger.LogInformation("Subscribing to channel {0} in guild {1}", channel.Name, guild.Name);
            var existing = await _repository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id) 
                                     && s.ChannelId.Equals(channel.Id));
            var existingList = existing?.ToList() ?? new ();

            if (!existingList.Any())
            {
                _logger.LogInformation("Not found any existing subscription. Creating new in database");
                await _repository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id,
                    SubscriptionType.Responses));
            }

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
            if (_subscriptions.ContainsKey((guildId, channelId)))
            {
                _logger.LogInformation("Already subscribed to channel with id {0} in guild with id {1}", guildId, channelId);
                return;
            }
            
            _logger.LogInformation("Creating new observable subscription to channel with id {0} in guild with id {1}", guildId, channelId);
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