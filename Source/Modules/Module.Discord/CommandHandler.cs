using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Module.Data.Models;
using Module.Discord.CommandModules;
using Module.Discord.TypeReaders;

namespace Module.Discord
{
    public class CommandHandler
    {
        public static readonly char CmdPrefix = '>';
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(IDiscordClient client, CommandService commands, IServiceProvider services)
        {
            _commands = commands;
            _services = services;
            _client = (DiscordSocketClient) client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            _commands.AddTypeReader<CreateQuestionEntityDto>(_services.GetService<CreateQuestionDtoTypeReader>());

            await _commands.AddModuleAsync<CoreDiscordModule>(_services);
            await _commands.AddModuleAsync<CheckUserCommandModule>(_services);
            await _commands.AddModuleAsync<EmojiCommandModule>(_services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(CmdPrefix, ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            // if (!result.IsSuccess)
            // await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}