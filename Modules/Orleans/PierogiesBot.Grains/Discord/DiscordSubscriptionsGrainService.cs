using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.Discord.Services;
using PierogiesBot.GrainsInterfaces;
using PierogiesBot.GrainsInterfaces.Discord;

namespace PierogiesBot.Grains.Discord
{
    [StatelessWorker(1)]
    [Reentrant]
    public class DiscordSubscriptionsGrainService : GrainService, IDiscordSubscriptionsGrainService
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerFactory _loggerFactory;
        private ChannelSubscribeService _channelSubscribeService;
        private CrontabSubscribeService _crontabSubscribeService;
        private ILogger<DiscordSubscriptionsGrainService> _logger;

        public DiscordSubscriptionsGrainService(IServiceProvider services, IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory) : base(id, silo, loggerFactory)
        {
            _services = services;
            _loggerFactory = loggerFactory;
        }

        public override async Task Init(IServiceProvider serviceProvider)
        {
            _channelSubscribeService = _services.GetService<ChannelSubscribeService>();
            _crontabSubscribeService = _services.GetService<CrontabSubscribeService>();
            _logger = _loggerFactory.CreateLogger<DiscordSubscriptionsGrainService>();
            
            await  base.Init(serviceProvider);
        }

        public override async Task Start()
        {
            _logger.LogDebug("Loading discord subscriptions");
            await _channelSubscribeService.LoadSubscriptionsAsync();
            await _crontabSubscribeService.LoadSubscriptionsAsync();
            await base.Start();
            
            _logger.LogDebug("Discord subscriptions loaded");
        }
    }
}