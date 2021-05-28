using System;

namespace PierogiesBot.Models.Dtos.BotReactRule
{
    public record CreateBotReactRuleDto(string Reaction, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}