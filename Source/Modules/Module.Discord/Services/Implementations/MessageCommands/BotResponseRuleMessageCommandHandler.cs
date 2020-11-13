using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Module.Data.Models;
using Module.Data.Storage;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public class BotResponseRuleMessageCommandHandler : IMessageCommandHandler
    {
        private readonly IDataSource<BotResponseRule, Guid> _botResponseRules;

        public BotResponseRuleMessageCommandHandler(IDataSource<BotResponseRule, Guid> botResponseRules)
        {
            _botResponseRules = botResponseRules;
        }

        public async Task Handle(IMessage message)
        {
            if (message.Author.IsBot) return;

            var rules = _botResponseRules.GetAll().ToList();

            foreach (var rule in rules)
            {
                switch ((isRegex: rule.IsTriggerTextRegex, triggerOnContains: rule.ShouldTriggerOnContains))
                {
                    case (true, true):
                        await RespondIfMessageMatchesRegex(message, rule);
                        break;
                    case (true, false):
                        await RespondIfWholeMessageMatchesRegex(message, rule);
                        break;
                    case (false, false):
                        await RespondIfMessageMatchesText(message, rule);
                        break;
                    case (false, true):
                        await RespondIfMessageContainsText(message, rule);
                        break;
                }
            }
        }

        private async Task RespondIfMessageMatchesRegex(IMessage message, BotResponseRule rule)
        {
            if (Regex.IsMatch(message.Content, rule.TriggerText))
                await message.Channel.SendMessageAsync(rule.RespondWith);
        }
        private async Task RespondIfWholeMessageMatchesRegex(IMessage message, BotResponseRule rule)
        {
            if (Regex.IsMatch(message.Content, $"^{rule.TriggerText}$"))
                await message.Channel.SendMessageAsync(rule.RespondWith);
        }
        private async Task RespondIfMessageMatchesText(IMessage message, BotResponseRule rule)
        {
            if (message.Content.Equals(rule.TriggerText, rule.StringComparison))
                await message.Channel.SendMessageAsync(rule.RespondWith);
        }
        private async Task RespondIfMessageContainsText(IMessage message, BotResponseRule rule)
        {
            if (message.Content.Contains(rule.TriggerText, rule.StringComparison))
                await message.Channel.SendMessageAsync(rule.RespondWith);
        }
    }
}