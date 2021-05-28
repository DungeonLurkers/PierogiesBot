using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PierogiesBot.Models
{
    public record BotResponseRule(string Id, string RespondWith, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) : BotMessageRuleBase(Id, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains)
    {
        public BotResponseRule(string respondWith, string triggerText, StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains)
            : this(ObjectId.GenerateNewId().ToString(), respondWith, triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains)
        {
            
        }
    }
}