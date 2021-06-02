using System;
using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotReactRule
{
    public record GetBotReactRuleDto(string Id, IEnumerable<string> Reactions, string TriggerText,
        StringComparison StringComparison,
        bool IsTriggerTextRegex, bool ShouldTriggerOnContains, ResponseMode ResponseMode);
}