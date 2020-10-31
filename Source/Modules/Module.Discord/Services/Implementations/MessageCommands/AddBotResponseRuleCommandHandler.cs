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
    public class AddBotResponseRuleCommandHandler : AddRuleCommandHandlerBase<BotResponseRule>
    {
        private readonly ILogger<AddBotResponseRuleCommandHandler> _logger;
        private readonly IDataSource<BotResponseRule, Guid> _rulesDataSource;

        public AddBotResponseRuleCommandHandler(ILogger<AddBotResponseRuleCommandHandler> logger,
            IDataSource<BotResponseRule, Guid> rulesDataSource) : base(logger)
        {
            _logger = logger;
            _rulesDataSource = rulesDataSource;
        }

        protected override string AddRuleCmdPrefix { get; set; } = "=>addrule ";

        protected override string RuleHelp { get; set; } =
            "=>addrule [TRIGGER_TEXT];[IS_REGEX];[SHOULD_TRIGGER_ON_CONTAINS];[RESPONSE_TEXT";
        protected override void HandleRule(string triggerText, bool isRegex, bool shouldTriggerOnContains, string respondWith, IMessage message)
        {
            if (!message.Content.StartsWith(AddRuleCmdPrefix)) return;
            _logger.LogDebug($"New rule command: {nameof(BotResponseRule.TriggerText)} = '{triggerText}'; " +
                             $"{nameof(BotResponseRule.IsTriggerTextRegex)} = {isRegex}; " +
                             $"{nameof(BotResponseRule.ShouldTriggerOnContains)} = {shouldTriggerOnContains}; " +
                             $"{nameof(BotResponseRule.RespondWith)} = {respondWith}");

            if (string.IsNullOrWhiteSpace(triggerText) || string.IsNullOrWhiteSpace(respondWith)) return;
            var rule = new BotResponseRule()
            {
                RespondWith = respondWith,
                TriggerText = triggerText,
                ShouldTriggerOnContains = shouldTriggerOnContains,
                IsTriggerTextRegex = isRegex
            };

            _logger.LogInformation("Saving new rule to db");

            _rulesDataSource.AddOrUpdate(rule);

            var msgPart = shouldTriggerOnContains ? "message contains" : "message is";
            msgPart = isRegex ? "pattern matches" : msgPart;

            message.Channel.SendMessageAsync($"I will respond with '{respondWith}' when {msgPart} '{triggerText}'");
        }
    }
}