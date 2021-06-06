using System;
using System.Collections.Generic;
using PierogiesBot.Commons.Enums;

namespace PierogiesBot.Commons.Dtos.BotResponseRule
{
    public record UpdateBotResponseRuleDto(ResponseMode ResponseMode, IEnumerable<string> Responses, string TriggerText,
        StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}