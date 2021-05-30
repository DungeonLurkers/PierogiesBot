using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PierogiesBot.Commons.Enums;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Jobs;
using PierogiesBot.Discord.Settings;
using PierogiesBot.Discord.TypeReaders;
using Quartz;
using Quartz.Impl;

namespace PierogiesBot.Discord.Services
{
    public class DiscordClientHostedService : IHostedService
    {
        private readonly ILogger<DiscordClientHostedService> _logger;
        private readonly ILogger _discordLogger;
        private readonly ILogger _commandLogger;
        private readonly IScheduler _scheduler;
        private readonly ILoggerFactory _loggerfactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandService _commandService;
        private DiscordSocketClient _client;
        private readonly IRepository<BotCrontabRule> _repository;
        private readonly ChannelSubscribeService _channelSubscribeService;
        private readonly CrontabSubscribeService _crontabSubscribeService;
        private DiscordSettings _settings;

        public DiscordClientHostedService(IScheduler scheduler, ILoggerFactory loggerfactory, IServiceProvider serviceProvider, 
            CommandService commandService, IOptions<DiscordSettings> discordOptions, DiscordSocketClient client,
            IRepository<BotCrontabRule> repository, ChannelSubscribeService channelSubscribeService, CrontabSubscribeService crontabSubscribeService)
        {
            _scheduler = scheduler;
            _loggerfactory = loggerfactory;
            _serviceProvider = serviceProvider;
            _commandService = commandService;
            _client = client;
            _repository = repository;
            _channelSubscribeService = channelSubscribeService;
            _crontabSubscribeService = crontabSubscribeService;

            _logger = _loggerfactory.CreateLogger<DiscordClientHostedService>();
            _discordLogger = _loggerfactory.CreateLogger("Discord");
            _commandLogger = _loggerfactory.CreateLogger("Discord.Commands");

            _settings = discordOptions.Value;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Discord services");
            var eventAwaiter = new TaskCompletionSource<bool>(false);
            void ClientOnReady() => eventAwaiter.SetResult(true);

            _client.Log += message => Task.Run(() =>
            {
                var logLevel = message.Severity switch
                {
                    LogSeverity.Critical => LogLevel.Critical,
                    LogSeverity.Error => LogLevel.Error,
                    LogSeverity.Warning => LogLevel.Warning,
                    LogSeverity.Info => LogLevel.Information,
                    LogSeverity.Verbose => LogLevel.Debug,
                    LogSeverity.Debug => LogLevel.Trace,
                    _ => throw new ArgumentOutOfRangeException(nameof(message), "has wrong LogSeverity!")
                };
                if (message.Exception is not null) _discordLogger.LogError(message.Exception, message.Message);
                else _discordLogger.Log(logLevel, message.Message);
            });
            
            _commandService.AddTypeReader<TimeZoneInfo>(new TimeZoneInfoTypeReader());
            
            await _client.LoginAsync(TokenType.Bot, _settings.Token);
            await _client.StartAsync();

            _client.Ready += () => Task.Run(ClientOnReady);

            await eventAwaiter.Task;

            await InstallCommandsAsync();

            await InitializeSubscriptions();
            
            await _scheduler.Start();

        }
        
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
            
        }
        
        private async Task InitializeSubscriptions()
        {
            await _channelSubscribeService.LoadSubscriptions();
            await _crontabSubscribeService.LoadSubscriptions();
        }
        
        private async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _commandService.CommandExecuted += CommandServiceOnCommandExecuted;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commandService.AddModulesAsync(assembly: Assembly.GetAssembly(GetType()), 
                services: _serviceProvider);
        }

        private Task CommandServiceOnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext ctx, IResult result)
        {
            if (!commandInfo.IsSpecified) return Task.CompletedTask;

            var cmd = commandInfo.Value!;

            if (result.IsSuccess)
            {
                _commandLogger.LogTrace("Command {0} execution was succesful", cmd.Name);
            }
            else
            {
                _commandLogger.LogError(result.ErrorReason);
            }

            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('>', ref argPos) || 
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);
            
            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commandService.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: _serviceProvider);
        }
    }
}