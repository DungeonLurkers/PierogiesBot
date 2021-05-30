using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace PierogiesBot.Data.Models
{
    public record BotReactRule(string Id, IEnumerable<string> Reactions, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) : BotMessageRuleBase(Id, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains)
    {
        public BotReactRule(IEnumerable<string> reactions, string triggerText, StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains)
        : this(ObjectId.GenerateNewId().ToString(), reactions, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains)
        {
            
        }

    }
}