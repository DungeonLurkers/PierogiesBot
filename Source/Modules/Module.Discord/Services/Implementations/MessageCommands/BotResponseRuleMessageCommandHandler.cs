using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public void Handle(IMessage message)
        {
            if (message.Author.IsBot) return;

            var rules = _botResponseRules.GetAll().ToList();

            foreach (var rule in rules)
            {
                if (rule.IsTriggerTextRegex)
                {
                    if (rule.ShouldTriggerOnContains)
                    {
                        if (Regex.IsMatch(message.Content, rule.TriggerText))
                        {
                            message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult(); // await
                        }
                    }
                    if (Regex.IsMatch(message.Content, $"^{rule.TriggerText}$"))
                    {
                        message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult(); // await
                    }
                }
                else
                {
                    if (rule.ShouldTriggerOnContains)
                    {
                        if (message.Content.Contains(rule.TriggerText, rule.StringComparison))
                        {
                            message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult(); // await
                        }
                    }
                    else
                    {
                        if (message.Content.Equals(rule.TriggerText, rule.StringComparison))
                        {
                            message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult();
                        }
                    }
                }
            }
        }
    }
}