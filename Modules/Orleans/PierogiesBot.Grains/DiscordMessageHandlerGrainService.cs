using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.Discord.Services;
using PierogiesBot.GrainsInterfaces;

namespace PierogiesBot.Grains
{
    [Reentrant]
    public class DiscordMessageHandlerGrainService : GrainService, IDiscordMessageHandlerGrainService
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerFactory _loggerFactory;
        private DiscordSocketClient _client;
        private ILogger<DiscordMessageHandlerGrainService> _logger;
        private IMessageHandlerChain _messageHandlerChain;

        public DiscordMessageHandlerGrainService(IServiceProvider services, IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory) : base(id, silo, loggerFactory)
        {
            _services = services;
            _loggerFactory = loggerFactory;
        }

        public override async Task Init(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<DiscordSocketClient>();
            _messageHandlerChain = serviceProvider.GetService<IMessageHandlerChain>();
            _logger = _loggerFactory.CreateLogger<DiscordMessageHandlerGrainService>();
            
            await  base.Init(serviceProvider);
        }

        public override Task Start()
        {
            _client.MessageReceived += ClientOnMessageReceived;
            _logger.LogDebug("Starting {0}", nameof(DiscordMessageHandlerGrainService));
            return base.Start();
        }

        private async Task ClientOnMessageReceived(SocketMessage arg)
        {
            if (arg is SocketUserMessage msg) await _messageHandlerChain.HandleAsync(new SocketCommandContext(_client, msg));
        }

        public override Task Stop()
        {
            _logger.LogDebug("Stopping {0}", nameof(DiscordMessageHandlerGrainService));
            _client.MessageReceived -= ClientOnMessageReceived;
            return base.Stop();
        }
    }
}