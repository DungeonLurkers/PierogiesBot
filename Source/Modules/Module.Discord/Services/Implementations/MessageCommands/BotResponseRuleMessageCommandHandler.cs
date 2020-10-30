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

        private IList<BotResponseRule> _rules;

        public BotResponseRuleMessageCommandHandler(IDataSource<BotResponseRule, Guid> botResponseRules)
        {
            _botResponseRules = botResponseRules;

            _rules = _botResponseRules.GetAll().ToList();
        }

        public void Handle(IMessage message)
        {

            foreach (var rule in _rules)
            {
                if (rule.IsRespondToRegex)
                {
                    if (Regex.IsMatch(message.Content, rule.RespondTo))
                    {
                        message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult(); // await
                    }
                }
                else
                {
                    if (rule.IsRespondOnContains)
                    {
                        if (message.Content.Contains(rule.RespondTo))
                        {
                            message.Channel.SendMessageAsync(rule.RespondWith).GetAwaiter().GetResult(); // await
                        }
                    }
                    else
                    {
                        if (message.Content.Equals(rule.RespondTo))
                        {
                            message.Channel.SendMessageAsync(rule.RespondWith);
                        }
                    }
                }
            }
        }
    }
}