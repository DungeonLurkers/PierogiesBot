using System;
using System.Collections.Generic;

namespace PierogiesBot.Commons.Dtos.BotResponseRule
{
    public record UpdateBotResponseRuleDto(IEnumerable<string> Responses, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}