using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Discord.Modules
{
    public abstract class LoggingModuleBase : ModuleBase<SocketCommandContext>
    {
        private ILogger<LoggingModuleBase> _logger;

        public LoggingModuleBase(ILogger<LoggingModuleBase> logger)
        {
            _logger = logger;
        }
        protected void LogTrace(string message, SocketUser user, SocketGuild guild, ISocketMessageChannel channel)
        {
            _logger.LogTrace($"<{guild}|{channel}|{user}> {message}");
        }

        protected void LogTrace(string message) => LogTrace(message, Context.User, Context.Guild, Context.Channel);
    }
}