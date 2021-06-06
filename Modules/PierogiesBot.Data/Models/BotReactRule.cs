using System;
using System.Collections.Generic;
using MongoDB.Bson;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Data.Models
{
    public record BotReactRule(string Id, IEnumerable<string> Reactions, string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains,
        ResponseMode ResponseMode = ResponseMode.First) : BotMessageRuleBase(Id, TriggerText, StringComparison,
        IsTriggerTextRegex,
        ShouldTriggerOnContains)

    {
        public BotReactRule(IEnumerable<string> reactions, string triggerText, StringComparison stringComparison,
            bool isTriggerTextRegex, bool shouldTriggerOnContains, ResponseMode responseMode = ResponseMode.First)
            : this(ObjectId.GenerateNewId().ToString(), reactions, triggerText, stringComparison, isTriggerTextRegex,
                shouldTriggerOnContains, responseMode)
        {
        }
    }
}