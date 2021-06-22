using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Modules
{
    public abstract class LoggingModuleBase : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<LoggingModuleBase> _logger;

        public LoggingModuleBase(ILogger<LoggingModuleBase> logger)
        {
            _logger = logger;
        }

        protected void LogTrace(string message, SocketUser user, SocketGuild guild, ISocketMessageChannel channel)
        {
            _logger.LogTrace($"<{guild}|{channel}|{user}> {message}");
        }

        protected void LogTrace(string message)
        {
            LogTrace(message, Context.User, Context.Guild, Context.Channel);
        }
        
        protected void LogError(Exception? e, string message = "")
        {
            LogError(e, Context.User, Context.Guild, Context.Channel, message);
        }
        
        protected void LogError(Exception? e, SocketUser user, SocketGuild guild, ISocketMessageChannel channel, string message = "")
        {
            _logger.LogError(e, $"<{guild}|{channel}|{user}> {message}");
        }
    }
}