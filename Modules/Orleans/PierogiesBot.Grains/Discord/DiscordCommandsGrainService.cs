using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.Discord.TypeReaders;
using PierogiesBot.GrainsInterfaces;
using PierogiesBot.GrainsInterfaces.Discord;

namespace PierogiesBot.Grains.Discord
{
    [StatelessWorker(1)]
    [Reentrant]
    public class DiscordCommandsGrainService : GrainService, IDiscordMessageHandlerGrain
    {
        private IServiceProvider _services;
        private readonly ILoggerFactory _loggerFactory;
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private ILogger<CommandService> _commandLogger;

        public DiscordCommandsGrainService(IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory) : base(id, silo, loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override async Task Init(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commandService = serviceProvider.GetRequiredService<CommandService>();
            _commandLogger = _loggerFactory.CreateLogger<CommandService>();
            
            _commandService.AddTypeReader<TimeZoneInfo>(new TimeZoneInfoTypeReader());
            
            await base.Init(serviceProvider);
        }

        public override async Task Start()
        {
            await InstallCommandsAsync();

            await base.Start();
        }
        
        public override Task Stop()
        {
            _client.MessageReceived -= HandleCommandAsync;
            _commandService.CommandExecuted -= CommandServiceOnCommandExecuted;
            
            return base.Stop();
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
            await _commandService.AddModulesAsync(
                assembly: Assembly.GetAssembly(typeof(PierogiesBot.Discord.ServiceCollectionExtensions)),
                services: _services);
        }

        private Task CommandServiceOnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext ctx,
            IResult result)
        {
            if (!commandInfo.IsSpecified) return Task.CompletedTask;

            var cmd = commandInfo.Value!;

            if (result.IsSuccess)
                _commandLogger.LogTrace("<{0}|>{1}|{2}>{3}", ctx.Guild, ctx.Channel, ctx.User, cmd.Name);
            else
                _commandLogger.LogError(result.ErrorReason);

            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

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
                services: _services);
        }

        
    }
}