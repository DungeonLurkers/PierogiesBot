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
    public class AddBotReactRuleCommandHandler : AddRuleCommandHandlerBase<BotReactRule>
    {
        private const string CmdPrefix = "=>addreact ";
        private readonly IDiscordClient _discordClient;
        private readonly IDataSource<BotReactRule, Guid> _rulesDataSource;
        private readonly ILogger<AddBotReactRuleCommandHandler> _logger;

        public AddBotReactRuleCommandHandler(IDiscordClient discordClient,
            IDataSource<BotReactRule, Guid> rulesDataSource,
            ILogger<AddBotReactRuleCommandHandler> logger) : base(logger)
        {
            _discordClient = discordClient;
            _rulesDataSource = rulesDataSource;
            _logger = logger;
        }

        protected override string AddRuleCmdPrefix { get; set; } = "=>addreact ";
        protected override string RuleHelp { get; set; } =
            "=>addreact [TRIGGER_MSG];[IS_REGEX];[SHOULD_TRIGGER_ON_CONTAINS];[REACTION_NAME]";
        protected override void HandleRule(string triggerText, bool isRegex, bool shouldTriggerOnContains, string respondWith, IMessage message)
        {
            if (!message.Content.StartsWith(AddRuleCmdPrefix)) return;
            
            _logger.LogDebug($"New rule command: {nameof(BotReactRule.TriggerText)} = '{triggerText}'; " +
                             $"{nameof(BotReactRule.IsTriggerTextRegex)} = {isRegex}; " +
                             $"{nameof(BotReactRule.ShouldTriggerOnContains)} = {shouldTriggerOnContains}; " +
                             $"{nameof(BotReactRule.Reaction)} = {respondWith}");

            if (string.IsNullOrWhiteSpace(triggerText) || string.IsNullOrWhiteSpace(respondWith)) return;
            var rule = new BotReactRule
            {
                Reaction = respondWith,
                TriggerText = triggerText,
                ShouldTriggerOnContains = shouldTriggerOnContains,
                IsTriggerTextRegex = isRegex
            };

            _logger.LogInformation("Saving new rule to db");

            _rulesDataSource.AddOrUpdate(rule);
            var msgPart = shouldTriggerOnContains ? "message contains" : "message is";
             msgPart = isRegex ? "pattern matches" : msgPart;

            message.Channel.SendMessageAsync($"I will react with '{respondWith}' when {msgPart} '{triggerText}'");
        }
    }
}