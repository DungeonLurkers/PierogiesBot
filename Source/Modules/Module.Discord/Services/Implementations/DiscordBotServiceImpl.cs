using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Module.Core.Enums;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations
{
    public class DiscordBotServiceImpl : IDiscordBotService
    {
        public IDiscordClient DiscordClient { get; }
        public IObservable<BotState> BotStateObservable { get; }
        public IObservable<(MessageChangeType changeType, IMessage message)> MessageObservable { get; }

        private ISubject<BotState> _botStateSubject;
        private ISubject<(MessageChangeType changeType, IMessage message)> _messageSubject;
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<DiscordBotServiceImpl> _logger;

        public DiscordBotServiceImpl(IDiscordClient discordClient, ILogger<DiscordBotServiceImpl> logger)
        {
            _discordClient = (DiscordSocketClient) discordClient;
            DiscordClient = discordClient;
            
            _logger = logger;

            _botStateSubject = new BehaviorSubject<BotState>(BotState.Unknown);
            BotStateObservable = _botStateSubject.AsObservable();

            _messageSubject = new Subject<(MessageChangeType, IMessage)>();
            MessageObservable = _messageSubject.AsObservable();
            
            _botStateSubject.OnNext(BotState.Created);

            _discordClient.LoggedIn += DiscordClientOnLoggedIn;
            _discordClient.Ready += DiscordClientOnReady;
            _discordClient.MessageReceived += DiscordClientOnMessageReceived;
            _discordClient.MessageDeleted += DiscordClientOnMessageDeleted;
            _discordClient.MessageUpdated += DiscordClientOnMessageUpdated;
            
            _botStateSubject.OnNext(BotState.Idle);
        }

        private Task DiscordClientOnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            _messageSubject.OnNext((MessageChangeType.Edited, arg2));
            return Task.CompletedTask;
        }

        private async Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            
            _messageSubject.OnNext((MessageChangeType.Removed, await arg1.GetOrDownloadAsync()));
        }

        private Task DiscordClientOnMessageReceived(SocketMessage message)
        {
            _messageSubject.OnNext((MessageChangeType.Added, message));
            return Task.CompletedTask;
        }

        private Task DiscordClientOnReady()
        {
            _botStateSubject.OnNext(BotState.Ready);
            return Task.CompletedTask;
        }

        private Task DiscordClientOnLoggedIn()
        {
            _botStateSubject.OnNext(BotState.Logged);
            return Task.CompletedTask;
        }

        public async Task LoginAsync(string token)
        {
            _botStateSubject.OnNext(BotState.Logging);
            await _discordClient.LoginAsync(TokenType.Bot, token);
        }

        public async Task StartAsync()
        {
            _botStateSubject.OnNext(BotState.Connecting);
            await _discordClient
                .StartAsync()
                .ContinueWith(task =>
                    _botStateSubject
                        .OnNext(task.IsCompletedSuccessfully ? BotState.Connected : BotState.ConnectingError));
        }

        public async Task StopAsync()
        {
            _botStateSubject.OnNext(BotState.LoggingOut);
            await _discordClient.LogoutAsync();
        }

        public Task<IEnumerable<IUser>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageChannel>> GetMessageChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IDMChannel?> GetMessageChannelAsync(ulong id) => await _discordClient.GetDMChannelAsync(id);
    }
}