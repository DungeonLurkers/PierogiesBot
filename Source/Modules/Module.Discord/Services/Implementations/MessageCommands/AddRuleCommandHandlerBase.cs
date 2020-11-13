using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Module.Data.Models;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public abstract class AddRuleCommandHandlerBase<TRule> : BotMessageCommandHandlerBase where TRule : BotMessageRuleBase
    {
        private readonly ILogger<AddRuleCommandHandlerBase<TRule>> _logger;
        protected abstract Embed RuleHelp { get; set; }

        protected AddRuleCommandHandlerBase(ILogger<AddRuleCommandHandlerBase<TRule>> logger, string cmdPrefix) : base(cmdPrefix)
        {
            _logger = logger;
        }
        public override async Task HandleInternal(IMessage message)
        {
            _logger.LogInformation($"Found {typeof(TRule).Name} command");
            if (message.Content.Equals($"{CmdPrefix}-h", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogDebug($"Displaying {typeof(TRule).Name} help");

                await message.Channel.SendMessageAsync(embed: RuleHelp);
            }

            var author = (SocketGuildUser) message.Author;
            _logger.LogDebug("Checking if author is administrator");

            if (!author.Roles.Any(role => role.Permissions.Administrator)) return;
            var command = message.Content!;

            var cmdEntries = command.Substring(CmdPrefix.Length - 1).Split(';', StringSplitOptions.RemoveEmptyEntries)!;
            if (cmdEntries.Length < 4) return;

            var triggerText = cmdEntries[0]!.TrimStart(' ');
            var isRegex = bool.Parse(cmdEntries[1]);
            var shouldTriggerOnContains = bool.Parse(cmdEntries[2]);
            var respondWith = cmdEntries[3];

            await HandleRule(triggerText, isRegex, shouldTriggerOnContains, respondWith, message);
        }

        protected abstract Task HandleRule(string triggerText, bool isRegex, bool shouldTriggerOnContains,
            string respondWith, IMessage message);
    }
}