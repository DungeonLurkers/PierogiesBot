using System;
using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.BotReactRule
{
    public record UpdateBotReactRuleDto(IEnumerable<string> Reactions, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}