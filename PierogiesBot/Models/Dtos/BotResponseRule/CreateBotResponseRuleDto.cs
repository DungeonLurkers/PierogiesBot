using System;

namespace PierogiesBot.Models.Dtos.BotResponseRule
{
    public record CreateBotResponseRuleDto(string RespondWith, string TriggerText, StringComparison StringComparison, bool IsTriggerTextRegex, bool ShouldTriggerOnContains);
}