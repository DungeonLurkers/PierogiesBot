using System;
using System.Linq;
using System.Threading.Tasks;
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
            IDataSource<BotResponseRule, Guid> rulesDataSource) : base(logger, "=>addrule ")
        {
            _logger = logger;
            _rulesDataSource = rulesDataSource;
        }

        protected override Embed RuleHelp { get; set; } =
            new EmbedBuilder()
            .WithColor(Color.Purple)
            .WithDescription("```powershell\n=>addrule [TRIGGER_TEXT];[IS_REGEX];[SHOULD_TRIGGER_ON_CONTAINS];[RESPONSE]\n\n[TRIGGER_TEXT] # text to trigger bot response\n[IS_REGEX} # true if [TRIGGER_TEXT] is a Regex pattern, default is false\n[SHOULD...] # true if bot should be triggered if message contains [TRIGGER_TEXT]. If false, bot will be triggered only if message is equal to trigger\n```")
            .WithCurrentTimestamp()
            .Build();

        protected override async Task HandleRule(string triggerText, bool isRegex, bool shouldTriggerOnContains, string respondWith, IMessage message)
        {
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

            await message.Channel.SendMessageAsync($"I will respond with '{respondWith}' when {msgPart} '{triggerText}'");
        }
    }
}