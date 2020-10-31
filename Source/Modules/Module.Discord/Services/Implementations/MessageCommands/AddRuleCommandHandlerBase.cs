using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Module.Data.Models;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public abstract class AddRuleCommandHandlerBase<TRule> : BotMessageCommandHandlerBase where TRule : BotMessageRuleBase
    {
        private readonly ILogger<AddRuleCommandHandlerBase<TRule>> _logger;
        protected abstract string AddRuleCmdPrefix { get; set; }
        protected abstract Embed RuleHelp { get; set; }

        public AddRuleCommandHandlerBase(ILogger<AddRuleCommandHandlerBase<TRule>> logger)
        {
            _logger = logger;
        }
        public override void HandleInternal(IMessage message)
        {
            var channel = message.Channel;
            _logger.LogInformation($"Found {typeof(TRule).Name} command");
            if (message.Content.Equals($"{AddRuleCmdPrefix}-h", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogDebug($"Displaying {typeof(TRule).Name} help");

                message.Channel.SendMessageAsync(embed: RuleHelp).GetAwaiter().GetResult();
            }

            var author = (SocketGuildUser) message.Author;
            _logger.LogDebug("Checking if author is administrator");

            if (!author.Roles.Any(role => role.Permissions.Administrator)) return;
            var command = message.Content!;

            var cmdEntries = command.Substring(AddRuleCmdPrefix.Length - 1).Split(';', StringSplitOptions.RemoveEmptyEntries)!;
            if (cmdEntries.Length < 4) return;

            var triggerText = cmdEntries[0]!.TrimStart(' ');
            var isRegex = bool.Parse(cmdEntries[1]);
            var shouldTriggerOnContains = bool.Parse(cmdEntries[2]);
            var respondWith = cmdEntries[3];

            HandleRule(triggerText, isRegex, shouldTriggerOnContains, respondWith, message);
        }

        protected abstract void HandleRule(string triggerText, bool isRegex, bool shouldTriggerOnContains,
            string respondWith, IMessage message);
    }
}