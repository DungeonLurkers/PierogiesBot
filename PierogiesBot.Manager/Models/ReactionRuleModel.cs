using System;
using System.Collections.Generic;
using System.Linq;
using PierogiesBot.Commons.Dtos.BotReactRule;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Manager.Models
{
    public record ReactionRuleModel(string Id, IEnumerable<string> Reactions, string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains,
        ResponseMode ResponseMode) : GetBotReactRuleDto(
        Id, Reactions, TriggerText, StringComparison, IsTriggerTextRegex, ShouldTriggerOnContains, ResponseMode)
    {
        
        public string ReactionsAsString =>
            ResponseMode == ResponseMode.First ? Reactions.First() : $"'{string.Join(", ", Reactions)}'";
    }
}