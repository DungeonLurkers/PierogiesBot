using System;

namespace PierogiesBot.Models.Dtos.BotReactRule
{
    public record UpdateBotReactRuleDto(string Reaction, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}