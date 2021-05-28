using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PierogiesBot.Models
{
    public record BotReactRule(string Id, string Reaction, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) : BotMessageRuleBase(Id, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains)
    {
        public BotReactRule(string reaction, string triggerText, StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains)
        : this(ObjectId.GenerateNewId().ToString(), reaction, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains)
        {
            
        }

    }
}