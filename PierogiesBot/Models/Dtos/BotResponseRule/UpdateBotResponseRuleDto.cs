using System;

namespace PierogiesBot.Models.Dtos.BotResponseRule
{
    public record UpdateBotResponseRuleDto(string RespondWith, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}