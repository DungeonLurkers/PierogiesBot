using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Module.Data.Models;
using Module.Data.Storage;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public class AddBotReactRuleCommandHandler : IMessageCommandHandler
    {
        private const string CmdPrefix = "=>addreact ";
        private readonly IDiscordClient _discordClient;
        private readonly IDataSource<BotReactRule, Guid> _rulesDataSource;
        private readonly ILogger<AddBotReactRuleCommandHandler> _logger;

        public AddBotReactRuleCommandHandler(IDiscordClient discordClient,
            IDataSource<BotReactRule, Guid> rulesDataSource,
            ILogger<AddBotReactRuleCommandHandler> logger)
        {
            _discordClient = discordClient;
            _rulesDataSource = rulesDataSource;
            _logger = logger;
        }
        public void Handle(IMessage message)
        {
            var channel = message.Channel;

            if (!message.Content.StartsWith(CmdPrefix)) return;

            _logger.LogInformation("Found AddRule command");
            if (message.Content.Equals($"{CmdPrefix}-h", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogDebug("Displaying AddRule help");
                channel.SendMessageAsync($"{CmdPrefix}[TRIGGER_TEXT];[IS_REGEX];[SHOULD_TRIGGER_ON_CONTAINS];[REACTION_NAME]");
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
            var reactionName = cmdEntries[3];

            _logger.LogDebug($"New rule command: {nameof(BotReactRule.TriggerText)} = '{triggerText}'; " +
                             $"{nameof(BotReactRule.IsTriggerTextRegex)} = {isRegex}; " +
                             $"{nameof(BotReactRule.ShouldTriggerOnContains)} = {shouldTriggerOnContains}; " +
                             $"{nameof(BotReactRule.Reaction)} = {reactionName}");

            if (string.IsNullOrWhiteSpace(triggerText) || string.IsNullOrWhiteSpace(reactionName)) return;
            var rule = new BotReactRule
            {
                Reaction = reactionName,
                TriggerText = triggerText,
                ShouldTriggerOnContains = shouldTriggerOnContains,
                IsTriggerTextRegex = isRegex
            };

            _logger.LogInformation("Saving new rule to db");

            _rulesDataSource.AddOrUpdate(rule);

            message.Channel.SendMessageAsync($"New rules set! ({command})");
        }
    }
}