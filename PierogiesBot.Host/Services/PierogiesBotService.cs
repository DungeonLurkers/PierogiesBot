using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Host.Models;
using PierogiesBot.Host.Services.Definitions;

namespace PierogiesBot.Host.Services
{
    public class PierogiesBotService : IHostedService
    {
        private readonly ILogger<PierogiesBotService> _logger;
        private readonly IDiscordBotService _discordBotService;

        public PierogiesBotService(ILogger<PierogiesBotService> logger, IDiscordBotService discordBotService)
        {
            _logger = logger;
            _discordBotService = discordBotService;

            InitializeSubscriptions();
        }

        public void InitializeSubscriptions()
        {
            _discordBotService.BotStateObservable.Subscribe(state =>
            {
                _logger.LogInformation("Inner bot state changed! New state is {0}", state);
            });

            var commandObservable = _discordBotService.MessageObservable
                .Where(tuple => tuple.changeType == MessageChangeType.Added)
                .Where(tuple => tuple.message.Content.StartsWith("=>"))
                .Select(tuple => tuple.message);

            commandObservable
                .Where(commandMsg => commandMsg.Content.StartsWith("=>sayhi"))
                .Subscribe(async commandMsg =>
                {
                    _logger.LogInformation("Sending message...");
                    var typingDisposable = commandMsg.Channel.EnterTypingState();
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await commandMsg.Channel.SendMessageAsync("Hi!");
                    typingDisposable.Dispose();
                });
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Bot");

            await _discordBotService.LoginAsync("");
            
            await _discordBotService.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Bot");
            await Task.CompletedTask;
        }
    }
}