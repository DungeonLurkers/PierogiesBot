using System;
using System.Collections.Generic;
using MongoDB.Bson;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Data.Models
{
    public record BotResponseRule(string Id, ResponseMode ResponseMode, IEnumerable<string> Responses,
        string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains) :
        BotMessageRuleBase(Id, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains)
    {
        public BotResponseRule(ResponseMode responseMode, IEnumerable<string> responses, string triggerText,
            StringComparison stringComparison, bool isTriggerTextRegex, bool shouldTriggerOnContains)
            : this(ObjectId.GenerateNewId().ToString(), responseMode, responses, triggerText, stringComparison,
                isTriggerTextRegex, shouldTriggerOnContains)
        {
        }
    }
}