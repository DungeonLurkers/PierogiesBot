using System;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using Module.Data.Models;
using Module.Data.Storage;
using Module.Discord.Services.Definitions;

namespace Module.Discord.Services.Implementations.MessageCommands
{
    public abstract class BotMessageRuleMessageCommandHandlerBase<TRule> : IMessageCommandHandler where TRule : BotMessageRuleBase
    {
        protected IDataSource<TRule, Guid> RulesDataSource { get; set; }

        public BotMessageRuleMessageCommandHandlerBase(IDataSource<TRule, Guid> rulesDataSource)
        {
            RulesDataSource = rulesDataSource;
        }
        public void Handle(IMessage message)
        {
            if (message.Author.IsBot) return;

            var rules = RulesDataSource.GetAll().ToList();

            foreach (var rule in rules)
            {
                if (rule.IsTriggerTextRegex)
                {
                    if (rule.ShouldTriggerOnContains)
                    {
                        if (Regex.IsMatch(message.Content, rule.TriggerText))
                        {
                            CommandAction(message, rule);
                        }
                    }
                    if (Regex.IsMatch(message.Content, $"^{rule.TriggerText}$"))
                    {
                        CommandAction(message, rule);
                    }
                }
                else
                {
                    if (rule.ShouldTriggerOnContains)
                    {
                        if (message.Content.Contains(rule.TriggerText, rule.StringComparison))
                        {
                            CommandAction(message, rule);
                        }
                    }
                    else
                    {
                        if (message.Content.Equals(rule.TriggerText, rule.StringComparison))
                        {
                            CommandAction(message, rule);
                        }
                    }
                }
            }
        }

        protected abstract void CommandAction(IMessage message, TRule rule);
    }
}