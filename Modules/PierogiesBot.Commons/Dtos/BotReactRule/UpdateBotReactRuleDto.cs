using System;
using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotReactRule
{
    public record UpdateBotReactRuleDto(IEnumerable<string> Reactions, string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains,
        ResponseMode ResponseMode) : BotReactRuleDtoBase, IUpdateEntityDto;
}