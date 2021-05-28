using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PierogiesBot.Models
{
    public abstract record BotMessageRuleBase(string Id, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) : EntityBase(Id)
    {
        protected BotMessageRuleBase(string triggerText, StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains) 
            : this(ObjectId.GenerateNewId().ToString(), triggerText, stringComparison, isTriggerTextRegex, shouldTriggerOnContains)
        {
            
        }
    }
}