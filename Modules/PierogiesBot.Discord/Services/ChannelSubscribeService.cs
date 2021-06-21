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
using PierogiesBot.Data.Enums;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Discord.Services
{
    public class ChannelSubscribeService : IChannelSubscribeService
    {
        private readonly DiscordSocketClient _client;
        private readonly IMessageHandlerChain _messageHandlerChain;
        private readonly IRepository<BotMessageSubscription> _repository;
        private readonly Dictionary<(ulong, ulong), IDisposable> _subscriptions;
        private readonly ISubject<SocketUserMessage> _userMessagesSubject;
        private readonly ILogger<ChannelSubscribeService> _logger;

        public ChannelSubscribeService(
            IRepository<BotMessageSubscription> repository,
            DiscordSocketClient client,
            IMessageHandlerChain messageHandlerChain,
            ILogger<ChannelSubscribeService> logger)
        {
            _repository = repository;
            _client = client;
            _messageHandlerChain = messageHandlerChain;
            _logger = logger;

            _subscriptions = new Dictionary<(ulong, ulong), IDisposable>();
            _userMessagesSubject = new Subject<SocketUserMessage>();

            _client.MessageReceived += async message => await Task.Run(() =>
            {
                if (message is SocketUserMessage { Author: { IsBot: false } } msg)
                {
                    _userMessagesSubject.OnNext(msg);
                }
            });
        }

        /// <inheritdoc/>
        public async Task LoadSubscriptionsAsync()
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
                await SubscribeAsync(channel);
            }
        }

        /// <inheritdoc/>
        public async Task SubscribeAsync(SocketGuildChannel channel)
        {
            var guild = channel.Guild;
            var guildS = guild.ToString();
            var channelS = channel.ToString();
            _logger.LogTrace("Subscribing to channel {0} in guild {1}", channelS, guildS);
            var existing = await _repository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id)
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Responses);
            var existingList = existing?.ToList() ?? new();

            if (!existingList.Any())
            {
                _logger.LogTrace("Not found any existing subscription. Creating new in database");
                await _repository.InsertAsync(new BotMessageSubscription(guild.Id, channel.Id, SubscriptionType.Responses));
            }

            var guildId = guild.Id;
            var channelId = channel.Id;

            if (_subscriptions.ContainsKey((guildId, channelId)))
            {
                _logger.LogTrace("Already subscribed to channel with id {0} in guild with id {1}", 
                    guildS,
                    channelS);
                return;
            }

            _logger.LogTrace("Creating new observable subscription to channel with id {0} in guild with id {1}",
                guildS,
                channelS);

            var disposable = _userMessagesSubject
                .ObserveOn(TaskPoolScheduler.Default)
                .Where(m => m is { Channel: { } c } && c.Id == channelId)
                .Select(m => new SocketCommandContext(_client, m))
                .Do(async ctx => await _messageHandlerChain.HandleAsync(ctx))
                .Subscribe();

            _subscriptions[(guildId, channelId)] = disposable;
        }

        /// <inheritdoc/>
        public async Task UnsubscribeAsync(SocketGuildChannel channel)
        {
            var guild = channel.Guild;
            var guildS = guild.ToString();
            var channelS = channel.ToString();
            _logger.LogTrace("Unsubscribing from channel {0} in guild {1}", channelS, guildS);

            var existingEnumerable = await _repository
                .GetByPredicate(s => s.GuildId.Equals(guild.Id)
                                     && s.ChannelId.Equals(channel.Id)
                                     && s.SubscriptionType == SubscriptionType.Responses);
            var existing = existingEnumerable.FirstOrDefault();

            if (existing is not null)
            {
                await _repository.DeleteAsync(existing.Id);
                if (_subscriptions[(guild.Id, channel.Id)] is { } sub) sub.Dispose();
            }
        }
    }
}