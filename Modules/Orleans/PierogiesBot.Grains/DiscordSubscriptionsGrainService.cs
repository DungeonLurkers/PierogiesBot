using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.Discord.Services;
using PierogiesBot.GrainsInterfaces;

namespace PierogiesBot.Grains
{
    public class DiscordSubscriptionsGrainService : GrainService, IDiscordSubscriptionsGrainService
    {
        private readonly IServiceProvider _services;
        private ILoggerFactory _loggerFactory;
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
            await _channelSubscribeService.LoadSubscriptions();
            await _crontabSubscribeService.LoadSubscriptions();
            await base.Start();
        }
    }
}